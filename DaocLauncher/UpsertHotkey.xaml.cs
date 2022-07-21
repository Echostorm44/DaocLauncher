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
        public List<VirtualKeyCode> PossibleVirtualKeyCodes { get; set; }
        public List<string> HotKeyMods { get; set; }
        
        public UpsertHotkey(HotKey key, ObservableCollection<HotKeyAction> actions, List<string> groupCategories)
        {
            GroupCategories = groupCategories;
            ActionTypeNames = Enum.GetNames(typeof(HotkeyActionType)).ToList();
            AllActions = new ObservableCollection<HotKeyAction>();
            AllActions = actions;
            TheHotKey = key;
            RemoveActionCommand = new RelayCommand(a => RemoveAction(a));
            HotKeyMods = new List<string>() { "None", "Alt", "Shift", "Control" };
            PossibleVirtualKeyCodes = new List<VirtualKeyCode>() { VirtualKeyCode.VK_0, VirtualKeyCode.VK_1, VirtualKeyCode.VK_2, VirtualKeyCode.VK_3, VirtualKeyCode.VK_4, 
                VirtualKeyCode.VK_5, VirtualKeyCode.VK_6, VirtualKeyCode.VK_7, VirtualKeyCode.VK_8, VirtualKeyCode.VK_9, VirtualKeyCode.VK_A, VirtualKeyCode.VK_B,
                VirtualKeyCode.VK_C, VirtualKeyCode.VK_D, VirtualKeyCode.VK_E, VirtualKeyCode.VK_F, VirtualKeyCode.VK_G, VirtualKeyCode.VK_H, VirtualKeyCode.VK_I,
                VirtualKeyCode.VK_J, VirtualKeyCode.VK_K, VirtualKeyCode.VK_L, VirtualKeyCode.VK_M, VirtualKeyCode.VK_N, VirtualKeyCode.VK_O, VirtualKeyCode.VK_P,
                VirtualKeyCode.VK_Q, VirtualKeyCode.VK_R, VirtualKeyCode.VK_S, VirtualKeyCode.VK_T, VirtualKeyCode.VK_U, VirtualKeyCode.VK_V, VirtualKeyCode.VK_W,
                VirtualKeyCode.VK_X, VirtualKeyCode.VK_Y, VirtualKeyCode.VK_Z
            };

            InitializeComponent();
            txtHotKey.Text = key.Key.ToString();
            switch (key.KeyModifiers)
            {
                case KeyModifier.None:
                    { 
                        ddlKeyMod.SelectedValue = "None";
                    }
                    break;
                case KeyModifier.Alt:
                    {
                        ddlKeyMod.SelectedValue = "Alt";
                    }
                    break;
                case KeyModifier.Shift:
                    {
                        ddlKeyMod.SelectedValue = "Shift";
                    }
                    break;
                case KeyModifier.Ctrl:
                    {
                        ddlKeyMod.SelectedValue = "Control";
                    }
                    break;
                default:
                    {
                        ddlKeyMod.SelectedValue = "None";
                    }
                    break;
            }
            txtDescription.Text = key.Description.ToString();
            this.DataContext = this;
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAddAction_Click(object sender, RoutedEventArgs e)
        {
            if (TheHotKey == null)
            {
                MessageBox.Show("Set your hotkey first");
                return;
            }

            if (ddlAddActionType.SelectedValue == null || !Enum.TryParse(ddlAddActionType.SelectedValue.ToString(), out HotkeyActionType selectedActionType))
            {
                return;
            }

            switch (selectedActionType)
            {
                case HotkeyActionType.AllKeyCommand:
                    {
                        if (ddlAddActionKeyToSend.SelectedValue == null || ddlAddActionModifierToSend.SelectedValue == null)
                        {
                            MessageBox.Show("Please make all selections before adding");
                            return;
                        }
                        VirtualKeyCode? modKey = null;
                        switch (((ComboBoxItem)ddlAddActionModifierToSend.SelectedValue).Content)
                        {
                            case "Shift":
                                {
                                    modKey = VirtualKeyCode.SHIFT;
                                }                                
                                break;
                            case "Alt":
                                {
                                    modKey = VirtualKeyCode.ALT;
                                }
                                break;
                            case "Control":
                                {
                                    modKey = VirtualKeyCode.CONTROL;
                                }
                                break;
                           default:
                                { 
                                    modKey = null;
                                }
                                break;
                        }

                        var act = new HotKeyAction(null, null, (VirtualKeyCode)ddlAddActionKeyToSend.SelectedValue,
                            modKey, null, HotkeyActionType.AllKeyCommand);
                        AllActions.Add(act);
                    }
                    break;
                case HotkeyActionType.AssistActiveWindow:
                    {
                        ddlAddActionGroupName.Visibility = Visibility.Visible;
                        var act = new HotKeyAction(ddlAddActionGroupName.SelectedValue.ToString(), null, null,
                            null, null, HotkeyActionType.AssistActiveWindow);
                    }
                    break;
                case HotkeyActionType.DisableAllHotkeys:
                    {

                    }
                    break;
                case HotkeyActionType.EchoSay:
                    {
                        txtAddActionText.Visibility = Visibility.Visible;
                    }
                    break;
                case HotkeyActionType.EnableAllHotkeys:
                    {

                    }
                    break;
                case HotkeyActionType.GroupKeyCommand:
                    {
                        ddlAddActionGroupName.Visibility = Visibility.Visible;
                        ddlAddActionKeyToSend.Visibility = Visibility.Visible;
                        ddlAddActionModifierToSend.Visibility = Visibility.Visible;
                    }
                    break;
                case HotkeyActionType.InviteAllWindowsToGroup:
                    {

                    }
                    break;
                case HotkeyActionType.PauseScript:
                    {
                        txtAddActionCount.Visibility = Visibility.Visible;
                    }
                    break;
                case HotkeyActionType.SlashCommand:
                    {
                        ddlAddActionGroupName.Visibility = Visibility.Visible;
                        txtAddActionText.Visibility = Visibility.Visible;
                        ddlAddActionGroupName.Visibility = Visibility.Visible;
                    }
                    break;
                case HotkeyActionType.SlashPrompt:
                    {

                    }
                    break;
                case HotkeyActionType.TargetActiveWindow:
                    {
                        ddlAddActionGroupName.Visibility = Visibility.Visible;
                    }
                    break;
                case HotkeyActionType.ToggleAllHotkeysOnOff:
                    {

                    }
                    break;
            }
            ddlAddActionType.SelectedValue = null;
            ResetAllActionDropdowns(); 
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

        void ResetAllActionDropdowns()
        {
            ddlAddActionGroupName.Visibility = Visibility.Collapsed;
            ddlAddActionGroupName.SelectedValue = null;
            ddlAddActionKeyToSend.Visibility = Visibility.Collapsed;
            ddlAddActionKeyToSend.SelectedValue = null;
            ddlAddActionModifierToSend.Visibility = Visibility.Collapsed;
            ddlAddActionModifierToSend.SelectedValue = null;
            txtAddActionCount.Visibility = Visibility.Collapsed;
            txtAddActionCount.Text = "";
            txtAddActionText.Visibility = Visibility.Collapsed;
            txtAddActionText.Text = "";
        }

        private void ddlAddActionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResetAllActionDropdowns();
            if (ddlAddActionType.SelectedValue == null || !Enum.TryParse(ddlAddActionType.SelectedValue.ToString(), out HotkeyActionType selectedActionType))
            {
                return;
            }

            switch (selectedActionType)
            {
                case HotkeyActionType.AllKeyCommand:
                    { 
                        ddlAddActionKeyToSend.Visibility = Visibility.Visible;
                        ddlAddActionModifierToSend.Visibility = Visibility.Visible;
                    }
                    break;
                case HotkeyActionType.AssistActiveWindow:
                    {
                        ddlAddActionGroupName.Visibility = Visibility.Visible;
                    }
                    break;
                case HotkeyActionType.DisableAllHotkeys:
                    {
                        
                    }
                    break;
                case HotkeyActionType.EchoSay:
                    {
                        txtAddActionText.Visibility = Visibility.Visible;
                    }
                    break;
                case HotkeyActionType.EnableAllHotkeys:
                    {
                        
                    }
                    break;
                case HotkeyActionType.GroupKeyCommand:
                    {
                        ddlAddActionGroupName.Visibility = Visibility.Visible;
                        ddlAddActionKeyToSend.Visibility = Visibility.Visible;
                        ddlAddActionModifierToSend.Visibility = Visibility.Visible;
                    }
                    break;
                case HotkeyActionType.InviteAllWindowsToGroup:
                    {
                        
                    }
                    break;
                case HotkeyActionType.PauseScript:
                    {
                        txtAddActionCount.Visibility = Visibility.Visible;
                    }
                    break;
                case HotkeyActionType.SlashCommand:
                    {
                        ddlAddActionGroupName.Visibility = Visibility.Visible;
                        txtAddActionText.Visibility = Visibility.Visible;
                        ddlAddActionGroupName.Visibility = Visibility.Visible;
                    }
                    break;
                case HotkeyActionType.SlashPrompt:
                    {
                        
                    }
                    break;
                case HotkeyActionType.TargetActiveWindow:
                    {
                        ddlAddActionGroupName.Visibility = Visibility.Visible;
                    }
                    break;
                case HotkeyActionType.ToggleAllHotkeysOnOff:
                    {
                        
                    }
                    break;
            }
        }

        private void txtHotKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtHotKey.Text))
            {
                txtHotKey.Text = TheHotKey.Key.ToString();
                return;
            }
            TheHotKey.Key = e.Key;
            txtHotKey.Text = TheHotKey.Key.ToString();
            ddlKeyMod.Focus();
        }

        private void ddlKeyMod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ddlKeyMod.SelectedValue)
            {
                case "Alt":
                    {
                        TheHotKey.KeyModifiers = KeyModifier.Alt;
                    }
                    break;
                case "Control":
                    {
                        TheHotKey.KeyModifiers = KeyModifier.Ctrl;
                    }
                    break;
                case "Shift":
                    {
                        TheHotKey.KeyModifiers = KeyModifier.Shift;
                    }
                    break;
                default:
                    {
                        TheHotKey.KeyModifiers = KeyModifier.None;
                    }
                    break;
            }
        }

        private void txtDescription_LostFocus(object sender, RoutedEventArgs e)
        {
            TheHotKey.Description = txtDescription.Text;
        }
    }
}
