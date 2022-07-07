using DaocLauncher.Helpers;
using DaocLauncher.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class Dashboard : UserControl
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public Dictionary<string, IntPtr> LoadedWindows { get; set; }
        public MacroSet ActiveMacroSet { get; set; }
        private object macroLock = new object();
        private bool macrosAreSleeping = false;
        // TODO google code cave method for C# dll injection

        public List<string> MacroSets { get; set; }
        public List<DaocCharacter> AllCharacters { get; set; }
        public List<Server> Servers { get; set; }
        public List<string> ServerNames { get; set; }
        public List<DaocAccount> Accounts { get; set; }
        public ICommand LaunchChar { get; private set; }
        public ICommand LaunchMyAcct { get; private set; }

        public SendKeysTo keySender { get; private set; }
        public Dashboard()
        {
            LoadedWindows = new Dictionary<string, IntPtr>();
            // TODO Load MacroSets
            AllCharacters = GeneralHelpers.LoadCharactersFromDisk() ?? new List<DaocCharacter>();
            Servers = GeneralHelpers.LoadServerListFromDisk()?.Servers ?? new List<Server>();
            ServerNames = Servers.Select(a => a.Name).ToList();
            Accounts = GeneralHelpers.LoadAccountListFromDisk()?.MyAccounts ?? new List<DaocAccount>();
            LaunchChar = new RelayCommand(a => LaunchCharClicked(a));
            LaunchMyAcct = new RelayCommand(a => LaunchMyAccountClicked(a));
            InitializeComponent();
            keySender = new SendKeysTo();
            this.DataContext = this;    
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    Button button = sender as Button;
        //    Game game = button.DataContext as Game;
        //    int id = game.ID;
        //    // ...
        //}

        void Test(object param)
        {
            MessageBox.Show("HA!");
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
            LoadedWindows.Add(selectedChar.Name, windowHandle);
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
            LoadedWindows.Add(selectedAccount.DefaultTag, windowHandle);
        }

        private void btnToggleMacroSet_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void OnHotKeyHandler(HotKey hotKey)
        {
            // Throw this into it's own thread


            if (!ActiveMacroSet.HotKeyCollection.TryGetValue(hotKey, out var targetActions))
            {
                return;
            }
            // We need to check and see if the key is Enter, /, ', r, or escape to deal with the chat window being open
            // escape always closes it, /, ' and r (reply) only open it and enter toggles it
            if (targetActions.Any(a => a.ActionType == HotkeyActionType.Disable || a.ActionType == HotkeyActionType.Enable || a.ActionType == HotkeyActionType.ToggleAllHotkeys))
            {
                foreach (var act in targetActions)
                {
                    switch (act.ActionType)
                    {                        
                        case HotkeyActionType.Disable:
                            {
                                macrosAreSleeping = true;
                            }
                            break;
                        case HotkeyActionType.Enable:
                            {
                                macrosAreSleeping = false;
                            }
                            break;
                        case HotkeyActionType.ToggleAllHotkeys:
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

            lock (macroLock)// Prevent things from getting too crazy
            {
                
                IntPtr windowToReturnTo = GetActiveWindow();
                var activeWindowName = "";
                if (LoadedWindows.ContainsValue(windowToReturnTo))
                {
                    activeWindowName = LoadedWindows.First(a => a.Value == windowToReturnTo).Key;
                }
                //keySender.SendThoseKeysSucka(windowToFind, VirtualKeyCode.VK_1, null, windowToReturnTo);
                //keySender.SendThoseKeysSucka(windowToFind, VirtualKeyCode.VK_3, VirtualKeyCode.SHIFT, windowToReturnTo);
                foreach (var act in targetActions)
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
                        case HotkeyActionType.Pause:
                            {
                                // thread sleep or task.delay here
                            }
                            break;
                        case HotkeyActionType.SlashCommand:
                            {
                                //var dialog = new TextPrompt();
                                //if (dialog.ShowDialog() == true)
                                //{
                                //    keySender.SendChatCommand(WindowToFind, dialog.ResponseText, WindowToReturnTo);
                                //}
                            }
                            break;
                        case HotkeyActionType.GroupCommand:
                            {

                            }
                            break;
                        case HotkeyActionType.AllKeyCommand:
                            {

                            }
                            break;
                        case HotkeyActionType.EchoText:
                            {

                            }
                            break;
                        case HotkeyActionType.InviteGroup:
                            {

                            }
                            break;
                        case HotkeyActionType.SlashPrompt:
                            {

                            }
                            break;
                    }
                }
            }


            //IntPtr windowToFind = FindWindow("DAoCMWC", "Buddyblocker");
            //IntPtr windowToReturnTo = GetActiveWindow();            
            //SendKeysTo keySender = new SendKeysTo();
            //keySender.SendThoseKeysSucka(windowToFind, VirtualKeyCode.VK_1, null, windowToReturnTo);
            //keySender.SendThoseKeysSucka(windowToFind, VirtualKeyCode.VK_3, VirtualKeyCode.SHIFT, windowToReturnTo);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var roo = e.Key;
            var yoo = e.Key.ToString();
            var loo = sender;
        }
    }
}
