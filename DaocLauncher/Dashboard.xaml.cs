using DaocLauncher.Helpers;
using DaocLauncher.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DaocLauncher
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl, INotifyPropertyChanged
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public Dictionary<string, IntPtr> LoadedWindows { get; set; }
        public MacroSet ActiveMacroSet { get; set; }
        private object macroLock = new object();
        private bool macrosAreSleeping = false;
        public bool TextPromptIsOpen
        {
            get; set;
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool MacrosAreRunning { get; set; }
        string macroStateText;
        public string MacroStateText
        {
            get => macroStateText;
            set
            {
                if (macroStateText == value)
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
        public Dashboard()
        {
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
            foreach (var process in Process.GetProcesses())
            {
                if (process.ProcessName.Contains("game.dll"))
                {
                    if (!LoadedWindows.ContainsKey(process.MainWindowTitle))
                    {
                        LoadedWindows.Add(process.MainWindowTitle, process.MainWindowHandle);
                    }
                }
            }

            this.DataContext = this;    
        }

        void LaunchCharClicked(object choice)
        {            
            if (choice == null)
            {
                return;
            }
            var selectedChar = (DaocCharacter)choice;
            if (string.IsNullOrEmpty(selectedChar.ParentAccountName) || string.IsNullOrEmpty(selectedChar.Name) || 
                string.IsNullOrEmpty(selectedChar.Server) || string.IsNullOrEmpty(selectedChar.Class))
            {
                return;
            }
            var accountData = Accounts.FirstOrDefault(a => a.Name == selectedChar.ParentAccountName);
            if (accountData == null)
            {
                return;
            }
            var serverData = Servers.FirstOrDefault(a => a.Name == selectedChar.Server);
            if (serverData == null)
            {
                return;
            }
            var classLookup = GeneralHelpers.GetAllCharacterClasses();
            if (!classLookup.ContainsKey(selectedChar.Class))
            {
                return;
            }
            var realmNumber = classLookup[selectedChar.Class];
            var windowHandle = GeneralHelpers.LaunchDaoc(selectedChar.Name, accountData.Name, accountData.Password, serverData.IP, serverData.ID, selectedChar.Name, realmNumber);
            if (!LoadedWindows.ContainsKey(selectedChar.Name))
            {
                LoadedWindows.Add(selectedChar.Name, windowHandle);
            }            
        }

        void LaunchMyAccountClicked(object choice)
        {
            if (choice == null)
            {
                return;
            }
            var selectedAccount = (DaocAccount)choice;
            if (string.IsNullOrEmpty(selectedAccount.DefaultServer))
            {
                return;
            }
            var serverData = Servers.FirstOrDefault(a => a.Name == selectedAccount.DefaultServer);
            if (serverData == null)
            {
                return;
            }
            GeneralHelpers.SaveAccountListToDisk(Accounts);// Save the default server if it changed
            var windowHandle = GeneralHelpers.LaunchDaoc(selectedAccount.DefaultTag, selectedAccount.Name, selectedAccount.Password, serverData.IP, serverData.ID, "", "");
            if (!LoadedWindows.ContainsKey("selectedAccount.DefaultTag"))
            {
                LoadedWindows.Add(selectedAccount.DefaultTag, windowHandle);
            }            
        }

        private void btnToggleMacroSet_Click(object sender, RoutedEventArgs e)
        {
            if (MacrosAreRunning) // They are already running, stop them
            {
                MacrosAreRunning = false;
                macrosAreSleeping = true;
                ActiveMacroSet.ListeningForHotkeys = false;
                foreach (var hotkey in ActiveMacroSet.HotKeyCollection)
                {
                    hotkey.Dispose();
                }                
                ddlMacroSets.IsEnabled = true;
                MacroStateText = "Start Macro Set";
            }
            else
            {
                if (ddlMacroSets.SelectedItem == null)
                {
                    return;
                }
                ActiveMacroSet = ((MacroSet)ddlMacroSets.SelectedItem).DeepCopyMe();// Need a deep copy here since it will get disposed                
                foreach (var hotkey in ActiveMacroSet.HotKeyCollection)
                {
                    hotkey.Register(OnHotKeyHandler);
                }
                ActiveMacroSet.ListeningForHotkeys = true;
                MacrosAreRunning = true;
                macrosAreSleeping = false;
                ddlMacroSets.IsEnabled = false;
                MacroStateText = "Stop Macro Set";
            }
        }

        private void OnHotKeyHandler(HotKey hotKey)
        {
            // See if the text prompt is up so we're not listening
            if (TextPromptIsOpen == true)
            {
                return;
            }

            // We need to check and see if the key is Enter, /, ', r, or escape to deal with the chat window being open
            // escape always closes it, /, ' and r (reply) only open it and enter toggles it
            if (hotKey.TriggeredActions.Any(a => a.ActionType == HotkeyActionType.DisableAllHotkeys || a.ActionType == HotkeyActionType.EnableAllHotkeys || a.ActionType == HotkeyActionType.ToggleAllHotkeysOnOff))
            {
                foreach (var act in hotKey.TriggeredActions)
                {
                    switch (act.ActionType)
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
                }
                return;
            }

            if (macrosAreSleeping)
            {
                return;
            }
            Task.Factory.StartNew(() =>
            {
                lock (macroLock)// Prevent things from getting too crazy
                {
                    IntPtr windowToReturnTo = GetActiveWindow();
                    var activeWindowName = "";
                    if (LoadedWindows.ContainsValue(windowToReturnTo))
                    {
                        activeWindowName = LoadedWindows.First(a => a.Value == windowToReturnTo).Key;
                    }
                    foreach (var act in hotKey.TriggeredActions)
                    {
                        switch (act.ActionType)
                        {
                            case HotkeyActionType.AssistActiveWindow:
                                {
                                    // First find all the windows we need to send the command to based on the group                                
                                    if (string.IsNullOrEmpty(act.GroupName))// No group to use command specified, so we send to everyone except main
                                    {
                                        foreach (var win in LoadedWindows)
                                        {
                                            if (win.Key != activeWindowName)
                                            {
                                                keySender.SendChatCommand(win.Value, "/assist " + activeWindowName, windowToReturnTo);
                                            }
                                        }
                                    }
                                    else if (ActiveMacroSet.CategoryGroups.TryGetValue(act.GroupName, out var windowNamesToCheckFor))
                                    {
                                        foreach (var name in windowNamesToCheckFor)
                                        {
                                            if (LoadedWindows.TryGetValue(name, out var winPtr))
                                            {
                                                keySender.SendChatCommand(winPtr, "/assist " + activeWindowName, windowToReturnTo);
                                            }
                                        }
                                    }
                                }
                                break;
                            case HotkeyActionType.TargetActiveWindow:
                                {
                                    if (string.IsNullOrEmpty(act.GroupName))// No group to use command specified, so we send to everyone except main
                                    {
                                        foreach (var win in LoadedWindows)
                                        {
                                            if (win.Key != activeWindowName)
                                            {
                                                keySender.SendChatCommand(win.Value, "/target " + activeWindowName, windowToReturnTo);
                                            }
                                        }
                                    }
                                    else if (ActiveMacroSet.CategoryGroups.TryGetValue(act.GroupName, out var windowNamesToCheckFor))
                                    {
                                        foreach (var name in windowNamesToCheckFor)
                                        {
                                            if (LoadedWindows.TryGetValue(name, out var winPtr))
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
                                    if (ActiveMacroSet.CategoryGroups.TryGetValue(act.GroupName, out var windowNamesToCheckFor))
                                    {
                                        foreach (var name in windowNamesToCheckFor)
                                        {
                                            if (LoadedWindows.TryGetValue(name, out var winPtr))
                                            {
                                                keySender.SendThoseKeysSucka(winPtr, act.KeyToSend.Value, act.ModifierKeyToSend, windowToReturnTo);
                                            }
                                        }
                                    }
                                }
                                break;
                            case HotkeyActionType.SlashCommand:
                                {
                                    if (ActiveMacroSet.CategoryGroups.TryGetValue(act.GroupName, out var windowNamesToCheckFor))
                                    {
                                        foreach (var name in windowNamesToCheckFor)
                                        {
                                            if (LoadedWindows.TryGetValue(name, out var winPtr))
                                            {
                                                keySender.SendChatCommand(winPtr, act.Text, windowToReturnTo);
                                            }
                                        }
                                    }
                                }
                                break;
                            case HotkeyActionType.AllKeyCommand:
                                {
                                    foreach (var win in LoadedWindows)
                                    {
                                        keySender.SendThoseKeysSucka(win.Value, act.KeyToSend.Value, act.ModifierKeyToSend, windowToReturnTo);
                                    }
                                }
                                break;
                            case HotkeyActionType.EchoSay:
                                {
                                    TextPromptIsOpen = true;
                                    var dialog = new TextPrompt();
                                    if (dialog.ShowDialog() == true && !string.IsNullOrEmpty(dialog.ResponseText))
                                    {
                                        TextPromptIsOpen = false;
                                        foreach (var win in LoadedWindows)
                                        {
                                            keySender.SendChatCommand(win.Value, "/say " + dialog.ResponseText, windowToReturnTo);
                                        }
                                    }
                                }
                                break;
                            case HotkeyActionType.InviteAllWindowsToGroup:
                                {
                                    foreach (var win in LoadedWindows)
                                    {
                                        if (win.Key != activeWindowName)
                                        {
                                            keySender.SendChatCommand(win.Value, "/invite " + activeWindowName, windowToReturnTo);
                                        }
                                    }
                                }
                                break;
                            case HotkeyActionType.SlashPrompt:
                                {
                                    TextPromptIsOpen = true;
                                    var dialog = new TextPrompt();
                                    if (dialog.ShowDialog() == true && !string.IsNullOrEmpty(dialog.ResponseText))
                                    {
                                        TextPromptIsOpen = false;
                                        foreach (var win in LoadedWindows)
                                        {
                                            keySender.SendChatCommand(win.Value, "/" + dialog.ResponseText, windowToReturnTo);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }, System.Threading.CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ddlMacroSets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        //IntPtr windowToFind = FindWindow("DAoCMWC", "Buddyblocker");
        //IntPtr windowToReturnTo = GetActiveWindow();            
        //SendKeysTo keySender = new SendKeysTo();
        //keySender.SendThoseKeysSucka(windowToFind, VirtualKeyCode.VK_1, null, windowToReturnTo);
        //keySender.SendThoseKeysSucka(windowToFind, VirtualKeyCode.VK_3, VirtualKeyCode.SHIFT, windowToReturnTo);
    }
}
