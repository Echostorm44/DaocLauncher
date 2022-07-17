using DaocLauncher.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DaocLauncher
{    
    /// <summary>
    /// Interaction logic for UpsertHotkey.xaml
    /// </summary>
    public partial class UpsertHotkey : Window
    {
        public HotKey TheHotKey { get; set; }
        public ObservableCollection<HotKeyAction> AllActions { get; set; }
        public ICommand RemoveActionCommand { get; private set; }
        public List<string> ActionTypeNames { get; set; }
        public List<string> GroupCategories { get; set; }

        public UpsertHotkey(HotKey key, List<HotKeyAction> actions, List<string> groupCategories)
        {
            GroupCategories = groupCategories;
            ActionTypeNames = Enum.GetNames(typeof(HotkeyActionType)).ToList();
            AllActions = new ObservableCollection<HotKeyAction>();
            actions.ForEach(a => AllActions.Add(a));
            TheHotKey = key;
            RemoveActionCommand = new RelayCommand(a => RemoveAction(a));
            InitializeComponent();
            this.DataContext = this;
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnAddAction_Click(object sender, RoutedEventArgs e)
        {
        }

        void RemoveAction(object choice)
        {
            if (choice == null)
            {
                return;
            }
            var targetAction = (HotKeyAction)choice;
            AllActions.Remove(targetAction);
        }

        private void ddlAddActionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ddlAddActionType.SelectedValue == null || !Enum.TryParse(ddlAddActionType.SelectedValue.ToString(), out HotkeyActionType selectedActionType))
            {
                return;
            }

            switch (selectedActionType)
            {
                case HotkeyActionType.AllKeyCommand:
                    { 
                        var moo = 1;                    
                    }
                    break;
                case HotkeyActionType.AssistActiveWindow:
                    {
                        var moo = 1;
                    }
                    break;
                case HotkeyActionType.DisableAllHotkeys:
                    {
                        var moo = 1;
                    }
                    break;
                case HotkeyActionType.EchoSay:
                    {
                        var moo = 1;
                    }
                    break;
                case HotkeyActionType.EnableAllHotkeys:
                    {
                        var moo = 1;
                    }
                    break;
                case HotkeyActionType.GroupKeyCommand:
                    {
                        var moo = 1;
                    }
                    break;
                case HotkeyActionType.InviteAllWindowsToGroup:
                    {
                        var moo = 1;
                    }
                    break;
                case HotkeyActionType.PauseScript:
                    {
                        var moo = 1;
                    }
                    break;
                case HotkeyActionType.SlashCommand:
                    {
                        var moo = 1;
                    }
                    break;
                case HotkeyActionType.SlashPrompt:
                    {
                        var moo = 1;
                    }
                    break;
                case HotkeyActionType.TargetActiveWindow:
                    {
                        var moo = 1;
                    }
                    break;
                case HotkeyActionType.ToggleAllHotkeysOnOff:
                    {
                        var moo = 1;
                    }
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
