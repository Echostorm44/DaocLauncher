using DaocLauncher.Helpers;
using DaocLauncher.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace DaocLauncher
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public Dictionary<string, IntPtr> LoadedWindows { get; set; }
        public MacroSet ActiveMacroSet { get; set; }
        private object macroLock = new object();
        private bool macrosAreSleeping = false;
        public bool TextPromptIsOpen { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool MacrosAreRunning { get; set; }
        string macroStateText;
        public string MacroStateText
        {
            get => macroStateText;
            set
            {
                if(macroStateText == value)
                {
                    return;
                }

                macroStateText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MacroStateText)));
            }
        }
        // TODO google code cave method for C# dll injection

        public List<MacroSet> MacroSets { get; set; }
        public List<DaocCharacter> AllCharacters { get; set; }
        public List<Server> Servers { get; set; }
        public List<string> ServerNames { get; set; }
        public List<DaocAccount> Accounts { get; set; }
        public ICommand LaunchChar { get; private set; }
        public ICommand LaunchMyAcct { get; private set; }
        public SendKeysTo keySender { get; private set; }

        private Menu TopMenu { get; set; }
        public Window ParentWindowYay { get; set; }

        public Dashboard(Menu topMenu, Window parent)
        {
            ParentWindowYay = parent;
            TopMenu = topMenu;
            MacroStateText = "Start Macro Set";
            MacrosAreRunning = false;
            LoadedWindows = new Dictionary<string, IntPtr>();
            AllCharacters = GeneralHelpers.LoadCharactersFromDisk() ?? new List<DaocCharacter>();
            Servers = GeneralHelpers.LoadServerListFromDisk()?.Servers ?? new List<Server>();
            ServerNames = Servers.Select(a => a.Name).ToList();
            Accounts = GeneralHelpers.LoadAccountListFromDisk()?.MyAccounts ?? new List<DaocAccount>();
            LaunchChar = new RelayCommand(a => LaunchCharClicked(a));
            LaunchMyAcct = new RelayCommand(a => LaunchMyAccountClicked(a));
            MacroSets = GeneralHelpers.LoadMacroSetsFromDisk();
            InitializeComponent();
            keySender = new SendKeysTo();
            TextPromptIsOpen = false;
            // Check for previously loaded chars.  In case program shutdown
            foreach(var process in Process.GetProcesses())
            {
                if(process.ProcessName.Contains("game.dll"))
                {
                    if(!LoadedWindows.ContainsKey(process.MainWindowTitle))
                    {
                        LoadedWindows.Add(process.MainWindowTitle, process.MainWindowHandle);
                    }
                }
            }
            this.DataContext = this;
        }

        void LaunchCharClicked(object choice)
        {
            if(choice == null)
            {
                return;
            }
            var selectedChar = (DaocCharacter)choice;
            if(string.IsNullOrEmpty(selectedChar.ParentAccountName) || string.IsNullOrEmpty(selectedChar.Name) || 
                string.IsNullOrEmpty(selectedChar.Server) || string.IsNullOrEmpty(selectedChar.Class))
            {
                return;
            }
            var accountData = Accounts.FirstOrDefault(a => a.Name == selectedChar.ParentAccountName);
            if(accountData == null)
            {
                return;
            }
            var serverData = Servers.FirstOrDefault(a => a.Name == selectedChar.Server);
            if(serverData == null)
            {
                return;
            }
            var classLookup = GeneralHelpers.GetAllCharacterClasses();
            if(!classLookup.ContainsKey(selectedChar.Class))
            {
                return;
            }
            var realmNumber = classLookup[selectedChar.Class];
            var windowHandle = GeneralHelpers.LaunchDaoc(selectedChar.Name, accountData.Name, accountData.Password, serverData.IP, serverData.ID, selectedChar.Name, realmNumber);
            if(!LoadedWindows.ContainsKey(selectedChar.Name))
            {
                LoadedWindows.Add(selectedChar.Name, windowHandle);
            }
        }

        void LaunchMyAccountClicked(object choice)
        {
            if(choice == null)
            {
                return;
            }
            var selectedAccount = (DaocAccount)choice;
            if(string.IsNullOrEmpty(selectedAccount.DefaultServer))
            {
                return;
            }
            var serverData = Servers.FirstOrDefault(a => a.Name == selectedAccount.DefaultServer);
            if(serverData == null)
            {
                return;
            }
            GeneralHelpers.SaveAccountListToDisk(Accounts);// Save the default server if it changed
            var windowHandle = GeneralHelpers.LaunchDaoc(selectedAccount.DefaultTag, selectedAccount.Name, selectedAccount.Password, serverData.IP, serverData.ID, "", "");
            if(!LoadedWindows.ContainsKey("selectedAccount.DefaultTag"))
            {
                LoadedWindows.Add(selectedAccount.DefaultTag, windowHandle);
            }
        }

        private HwndSource _source;

        private void btnToggleMacroSet_Click(object sender, RoutedEventArgs e)
        {
            if(MacrosAreRunning) // They are already running, stop them
            {
                waitForHotkeys = false;
                TopMenu.IsEnabled = true;
                MacrosAreRunning = false;
                macrosAreSleeping = true;
                ActiveMacroSet.ListeningForHotkeys = false;
                ActiveMacroSet.Dispose();
                ddlMacroSets.IsEnabled = true;
                MacroStateText = "Start Macro Set";
            }
            else
            {
                if(ddlMacroSets.SelectedItem == null)
                {
                    return;
                }
                TopMenu.IsEnabled = false;
                waitForHotkeys = true;
                //Task.Run(() => DoTheHotkeyStuff());// FF
                ActiveMacroSet = ((MacroSet)ddlMacroSets.SelectedItem).DeepCopyMe();// Need a deep copy here since it will get disposed

                foreach(var hotkey in ActiveMacroSet.HotKeyCollection)
                {
                    hotkey.Register(OnHotKeyHandler);
                }
                ActiveMacroSet.ListeningForHotkeys = true;
                ActiveMacroSet.StartMeUp();
                MacrosAreRunning = true;
                macrosAreSleeping = false;
                ddlMacroSets.IsEnabled = false;
                MacroStateText = "Stop Macro Set";
            }
        }


        public enum MapType : uint
        {
            MAPVK_VK_TO_VSC = 0x0,
            MAPVK_VSC_TO_VK = 0x1,
            MAPVK_VK_TO_CHAR = 0x2,
            MAPVK_VSC_TO_VK_EX = 0x3,
        }

        [DllImport("user32.dll")]
        public static extern int ToUnicode(
            uint wVirtKey,
            uint wScanCode,
            byte[] lpKeyState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)]
            StringBuilder pwszBuff,
            int cchBuff,
            uint wFlags);

        [DllImport("user32.dll")]
        public static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, MapType uMapType);


        public static char GetCharFromKey(Key key)
        {
            char ch = ' ';

            int virtualKey = KeyInterop.VirtualKeyFromKey(key);
            byte[] keyboardState = new byte[256];
            GetKeyboardState(keyboardState);

            uint scanCode = MapVirtualKey((uint)virtualKey, MapType.MAPVK_VK_TO_VSC);
            StringBuilder stringBuilder = new StringBuilder(2);

            int result = ToUnicode((uint)virtualKey, scanCode, keyboardState, stringBuilder, stringBuilder.Capacity, 0);
            switch(result)
            {
                case -1:
                    break;
                case 0:
                    break;
                case 1:
                {
                    ch = stringBuilder[0];
                    break;
                }
                default:
                {
                    ch = stringBuilder[0];
                    break;
                }
            }
            return ch;
        }


        ConcurrentQueue<HotKey> hotKeyQueue = new ConcurrentQueue<HotKey>();

        private void OnHotKeyHandler(HotKey hotKey)
        {
            IntPtr activeWindow = GetForegroundWindow();
            // See if the text prompt is up so we're not listening
            if(TextPromptIsOpen == true)
            {
                // We need to allow for hotkeys to still be used for typing.                
                var translation = (VirtualKeyCode)KeyInterop.VirtualKeyFromKey(hotKey.Key);
                keySender.JustSendKey(activeWindow, translation);
                return;
            }

            // We need to check and see if the key is Enter, /, ', r, or escape to deal with the chat window being open
            // escape always closes it, /, ' and r (reply) only open it and enter toggles it
            if(hotKey.TriggeredActions.Any(a => a.ActionType == HotkeyActionType.DisableAllHotkeys || a.ActionType == HotkeyActionType.EnableAllHotkeys || a.ActionType == HotkeyActionType.ToggleAllHotkeysOnOff))
            {
                foreach(var act in hotKey.TriggeredActions)
                {
                    switch(act.ActionType)
                    {
                        case HotkeyActionType.DisableAllHotkeys:
                        {
                            macrosAreSleeping = true;
                        }
                            break;
                        case HotkeyActionType.EnableAllHotkeys:
                        {
                            macrosAreSleeping = false;
                        }
                            break;
                        case HotkeyActionType.ToggleAllHotkeysOnOff:
                        {
                            macrosAreSleeping = !macrosAreSleeping;
                        }
                            break;
                    }
                    if(hotKey.Key == Key.Enter || hotKey.Key == Key.Return)
                    {
                        var translation = (VirtualKeyCode)KeyInterop.VirtualKeyFromKey(hotKey.Key);
                        keySender.SendThoseKeysSucka(activeWindow, translation, null, activeWindow);
                    }
                    else
                    {
                        var myChar = GetCharFromKey(hotKey.Key);
                        keySender.JustSendKey(activeWindow, myChar);
                    }
                }
                return;
            }

            if(macrosAreSleeping)
            {// Let the keys still get through in case we're in chat || something
                var myChar = GetCharFromKey(hotKey.Key);
                keySender.JustSendKey(activeWindow, myChar);
                return;
            }

            //hotKeyQueue.Enqueue(hotKey);
            #region something different
            Task.Factory.StartNew(() =>
            {
                lock(macroLock)// Prevent things from getting too crazy
                {
                    IntPtr windowToReturnTo = GetForegroundWindow();
                    var activeWindowName = "";
                    if(LoadedWindows.ContainsValue(windowToReturnTo))
                    {
                        activeWindowName = LoadedWindows.First(a => a.Value == windowToReturnTo).Key;
                    }
                    foreach(var act in hotKey.TriggeredActions)
                    {
                        switch(act.ActionType)
                        {
                            case HotkeyActionType.AssistActiveWindow:
                            {
                                // First find all the windows we need to send the command to based on the group                                
                                if(string.IsNullOrEmpty(act.GroupName))// No group to use command specified, so we send to everyone except main
                                {
                                    foreach(var win in LoadedWindows)
                                    {
                                        if(win.Key != activeWindowName)
                                        {
                                            keySender.SendChatCommand(win.Value, "/assist " + activeWindowName, windowToReturnTo);
                                        }
                                    }
                                }
                                else if(ActiveMacroSet.CategoryGroups.TryGetValue(act.GroupName, out var windowNamesToCheckFor))
                                {
                                    foreach(var name in windowNamesToCheckFor)
                                    {
                                        if(LoadedWindows.TryGetValue(name, out var winPtr))
                                        {
                                            keySender.SendChatCommand(winPtr, "/assist " + activeWindowName, windowToReturnTo);
                                        }
                                    }
                                }
                            }
                                break;
                            case HotkeyActionType.TargetActiveWindow:
                            {
                                if(string.IsNullOrEmpty(act.GroupName))// No group to use command specified, so we send to everyone except main
                                {
                                    foreach(var win in LoadedWindows)
                                    {
                                        if(win.Key != activeWindowName)
                                        {
                                            keySender.SendChatCommand(win.Value, "/target " + activeWindowName, windowToReturnTo);
                                        }
                                    }
                                }
                                else if(ActiveMacroSet.CategoryGroups.TryGetValue(act.GroupName, out var windowNamesToCheckFor))
                                {
                                    foreach(var name in windowNamesToCheckFor)
                                    {
                                        if(LoadedWindows.TryGetValue(name, out var winPtr))
                                        {
                                            keySender.SendChatCommand(winPtr, "/target " + activeWindowName, windowToReturnTo);
                                        }
                                    }
                                }
                            }
                                break;
                            case HotkeyActionType.PauseScript:
                            {
                                System.Threading.Thread.Sleep(act.Count ?? 10);
                            }
                                break;
                            case HotkeyActionType.GroupKeyCommand:
                            {
                                if(ActiveMacroSet.CategoryGroups.TryGetValue(act.GroupName, out var windowNamesToCheckFor))
                                {
                                    foreach(var name in windowNamesToCheckFor)
                                    {
                                        if(LoadedWindows.TryGetValue(name, out var winPtr))
                                        {
                                            keySender.SendThoseKeysSucka(winPtr, act.KeyToSend.Value, act.ModifierKeyToSend, windowToReturnTo);
                                        }
                                    }
                                }
                            }
                                break;
                            case HotkeyActionType.SlashCommand:
                            {
                                if(ActiveMacroSet.CategoryGroups.TryGetValue(act.GroupName, out var windowNamesToCheckFor))
                                {
                                    foreach(var name in windowNamesToCheckFor)
                                    {
                                        if(LoadedWindows.TryGetValue(name, out var winPtr))
                                        {
                                            keySender.SendChatCommand(winPtr, act.Text, windowToReturnTo);
                                        }
                                    }
                                }
                                else
                                {
                                    foreach(var win in LoadedWindows)
                                    {
                                        keySender.SendChatCommand(win.Value, act.Text, windowToReturnTo);
                                    }
                                }
                            }
                                break;
                            case HotkeyActionType.AllKeyCommand:
                            {
                                foreach(var win in LoadedWindows)
                                {
                                    keySender.SendThoseKeysSucka(win.Value, act.KeyToSend.Value, act.ModifierKeyToSend, windowToReturnTo);
                                }
                            }
                                break;
                            case HotkeyActionType.EchoSay:
                            {
                                TextPromptIsOpen = true;
                                var dialog = new TextPrompt();
                                if(dialog.ShowDialog() == true && !string.IsNullOrEmpty(dialog.ResponseText))
                                {
                                    foreach(var win in LoadedWindows)
                                    {
                                        keySender.SendChatCommand(win.Value, "/say " + dialog.ResponseText, windowToReturnTo);
                                    }
                                }
                                TextPromptIsOpen = false;
                            }
                                break;
                            case HotkeyActionType.InviteAllWindowsToGroup:
                            {
                                //Get active window pointer
                                if(!LoadedWindows.ContainsKey(activeWindowName))
                                {
                                    return;
                                }
                                var activeWindowPointer = LoadedWindows[activeWindowName];
                                foreach(var win in LoadedWindows)
                                {
                                    if(win.Key != activeWindowName)
                                    {
                                        keySender.SendChatCommand(activeWindowPointer, "/invite " + win.Key, windowToReturnTo);
                                    }
                                }
                            }
                                break;
                            case HotkeyActionType.SlashPrompt:
                            {
                                TextPromptIsOpen = true;
                                var dialog = new TextPrompt();
                                if(dialog.ShowDialog() == true && !string.IsNullOrEmpty(dialog.ResponseText))
                                {
                                    foreach(var win in LoadedWindows)
                                    {
                                        keySender.SendChatCommand(win.Value, "/" + dialog.ResponseText, windowToReturnTo);
                                    }
                                }
                                TextPromptIsOpen = false;
                            }
                                break;
                        }
                    }
                }
            }, System.Threading.CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
            #endregion
        }

        private void ddlMacroSets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        bool waitForHotkeys = true;

        void DoTheHotkeyStuff()
        {
            try
            {
                while(waitForHotkeys)// Prevent things from getting too crazy
                {
                    if(hotKeyQueue.Count == 0)
                    {
                        System.Threading.Thread.Sleep(10);
                        continue;
                    }

                    if(!hotKeyQueue.TryDequeue(out var hotKey))
                    {
                        System.Threading.Thread.Sleep(10);
                        continue;
                    }

                    IntPtr windowToReturnTo = GetForegroundWindow();
                    var activeWindowName = "";
                    if(LoadedWindows.ContainsValue(windowToReturnTo))
                    {
                        activeWindowName = LoadedWindows.First(a => a.Value == windowToReturnTo).Key;
                    }
                    foreach(var act in hotKey.TriggeredActions)
                    {
                        switch(act.ActionType)
                        {
                            case HotkeyActionType.AssistActiveWindow:
                            {
                                // First find all the windows we need to send the command to based on the group                                
                                if(string.IsNullOrEmpty(act.GroupName))// No group to use command specified, so we send to everyone except main
                                {
                                    foreach(var win in LoadedWindows)
                                    {
                                        if(win.Key != activeWindowName)
                                        {
                                            keySender.SendChatCommand(win.Value, "/assist " + activeWindowName, windowToReturnTo);
                                        }
                                    }
                                }
                                else if(ActiveMacroSet.CategoryGroups.TryGetValue(act.GroupName, out var windowNamesToCheckFor))
                                {
                                    foreach(var name in windowNamesToCheckFor)
                                    {
                                        if(LoadedWindows.TryGetValue(name, out var winPtr))
                                        {
                                            keySender.SendChatCommand(winPtr, "/assist " + activeWindowName, windowToReturnTo);
                                        }
                                    }
                                }
                            }
                                break;
                            case HotkeyActionType.TargetActiveWindow:
                            {
                                if(string.IsNullOrEmpty(act.GroupName))// No group to use command specified, so we send to everyone except main
                                {
                                    foreach(var win in LoadedWindows)
                                    {
                                        if(win.Key != activeWindowName)
                                        {
                                            keySender.SendChatCommand(win.Value, "/target " + activeWindowName, windowToReturnTo);
                                        }
                                    }
                                }
                                else if(ActiveMacroSet.CategoryGroups.TryGetValue(act.GroupName, out var windowNamesToCheckFor))
                                {
                                    foreach(var name in windowNamesToCheckFor)
                                    {
                                        if(LoadedWindows.TryGetValue(name, out var winPtr))
                                        {
                                            keySender.SendChatCommand(winPtr, "/target " + activeWindowName, windowToReturnTo);
                                        }
                                    }
                                }
                            }
                                break;
                            case HotkeyActionType.PauseScript:
                            {
                                System.Threading.Thread.Sleep(act.Count ?? 10);
                            }
                                break;
                            case HotkeyActionType.GroupKeyCommand:
                            {
                                if(ActiveMacroSet.CategoryGroups.TryGetValue(act.GroupName, out var windowNamesToCheckFor))
                                {
                                    foreach(var name in windowNamesToCheckFor)
                                    {
                                        if(LoadedWindows.TryGetValue(name, out var winPtr))
                                        {
                                            keySender.SendThoseKeysSucka(winPtr, act.KeyToSend.Value, act.ModifierKeyToSend, windowToReturnTo);
                                        }
                                    }
                                }
                            }
                                break;
                            case HotkeyActionType.SlashCommand:
                            {
                                if(ActiveMacroSet.CategoryGroups.TryGetValue(act.GroupName, out var windowNamesToCheckFor))
                                {
                                    foreach(var name in windowNamesToCheckFor)
                                    {
                                        if(LoadedWindows.TryGetValue(name, out var winPtr))
                                        {
                                            keySender.SendChatCommand(winPtr, act.Text, windowToReturnTo);
                                        }
                                    }
                                }
                                else
                                {
                                    foreach(var win in LoadedWindows)
                                    {
                                        keySender.SendChatCommand(win.Value, act.Text, windowToReturnTo);
                                    }
                                }
                            }
                                break;
                            case HotkeyActionType.AllKeyCommand:
                            {
                                foreach(var win in LoadedWindows)
                                {
                                    keySender.SendThoseKeysSucka(win.Value, act.KeyToSend.Value, act.ModifierKeyToSend, windowToReturnTo);
                                }
                            }
                                break;
                            case HotkeyActionType.EchoSay:
                            {
                                TextPromptIsOpen = true;
                                var dialog = new TextPrompt();
                                if(dialog.ShowDialog() == true && !string.IsNullOrEmpty(dialog.ResponseText))
                                {
                                    foreach(var win in LoadedWindows)
                                    {
                                        keySender.SendChatCommand(win.Value, "/say " + dialog.ResponseText, windowToReturnTo);
                                    }
                                }
                                TextPromptIsOpen = false;
                            }
                                break;
                            case HotkeyActionType.InviteAllWindowsToGroup:
                            {
                                //Get active window pointer
                                if(!LoadedWindows.ContainsKey(activeWindowName))
                                {
                                    return;
                                }
                                var activeWindowPointer = LoadedWindows[activeWindowName];
                                foreach(var win in LoadedWindows)
                                {
                                    if(win.Key != activeWindowName)
                                    {
                                        keySender.SendChatCommand(activeWindowPointer, "/invite " + win.Key, windowToReturnTo);
                                    }
                                }
                            }
                                break;
                            case HotkeyActionType.SlashPrompt:
                            {
                                TextPromptIsOpen = true;
                                var dialog = new TextPrompt();
                                if(dialog.ShowDialog() == true && !string.IsNullOrEmpty(dialog.ResponseText))
                                {
                                    foreach(var win in LoadedWindows)
                                    {
                                        keySender.SendChatCommand(win.Value, "/" + dialog.ResponseText, windowToReturnTo);
                                    }
                                }
                                TextPromptIsOpen = false;
                            }
                                break;
                        }
                    }
                }
                var woo = 1;
            }
            catch(Exception ex)
            {
                var testt = ex;
            }
        }

        ~Dashboard()
        {
            MacroSets.Clear();
            ActiveMacroSet = null;
        }
    }
}
