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

namespace DaocLauncher;

/// <summary>
/// Interaction logic for UpsertHotkey.xaml
/// </summary>
public partial class UpsertHotkey : Window
{
    public HotKey TheHotKey { get; set; }
    public ICommand RemoveActionCommand { get; private set; }
    public List<string> ActionTypeNames { get; set; }
    public List<string> GroupCategories { get; set; }
    public List<VirtualKeyCode> PossibleVirtualKeyCodes { get; set; }
    public List<string> HotKeyMods { get; set; }
    public bool DeleteMe { get; set; }

    public UpsertHotkey(HotKey key, List<string> groupCategories)
    {
        DeleteMe = false;
        GroupCategories = groupCategories;
        if(!GroupCategories.Any(a => a == "All"))
        {
            GroupCategories.Add("All");
        }
        ActionTypeNames = Enum.GetNames(typeof(HotkeyActionType)).ToList();
        TheHotKey = key;
        RemoveActionCommand = new RelayCommand(a => RemoveAction(a));
        HotKeyMods = new List<string>() { "None", "Alt", "Shift", "Control" };
        PossibleVirtualKeyCodes = new List<VirtualKeyCode>() { VirtualKeyCode.VK_0, VirtualKeyCode.VK_1, VirtualKeyCode.VK_2, VirtualKeyCode.VK_3, VirtualKeyCode.VK_4, 
            VirtualKeyCode.VK_5, VirtualKeyCode.VK_6, VirtualKeyCode.VK_7, VirtualKeyCode.VK_8, VirtualKeyCode.VK_9, VirtualKeyCode.VK_A, VirtualKeyCode.VK_B,
            VirtualKeyCode.VK_C, VirtualKeyCode.VK_D, VirtualKeyCode.VK_E, VirtualKeyCode.VK_F, VirtualKeyCode.VK_G, VirtualKeyCode.VK_H, VirtualKeyCode.VK_I,
            VirtualKeyCode.VK_J, VirtualKeyCode.VK_K, VirtualKeyCode.VK_L, VirtualKeyCode.VK_M, VirtualKeyCode.VK_N, VirtualKeyCode.VK_O, VirtualKeyCode.VK_P,
            VirtualKeyCode.VK_Q, VirtualKeyCode.VK_R, VirtualKeyCode.VK_S, VirtualKeyCode.VK_T, VirtualKeyCode.VK_U, VirtualKeyCode.VK_V, VirtualKeyCode.VK_W,
            VirtualKeyCode.VK_X, VirtualKeyCode.VK_Y, VirtualKeyCode.VK_Z, VirtualKeyCode.SPACE
        };

        InitializeComponent();
        txtHotKey.Text = key.Key.ToString();
        switch(key.KeyModifiers)
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
        if(TheHotKey == null)
        {
            MessageBox.Show("Set your hotkey first");
            return;
        }

        if(ddlAddActionType.SelectedValue == null || !Enum.TryParse(ddlAddActionType.SelectedValue.ToString(), out HotkeyActionType selectedActionType))
        {
            return;
        }
        int nextSortID = 0;
        if(TheHotKey.TriggeredActions.Count > 0)
        {
            nextSortID = TheHotKey.TriggeredActions.Max(a => a.SortOrderID);
        }
        switch(selectedActionType)
        {
            case HotkeyActionType.AllKeyCommand:
            {
                if(ddlAddActionKeyToSend.SelectedValue == null || ddlAddActionModifierToSend.SelectedValue == null)
                {
                    MessageBox.Show("Please make all selections before adding");
                    return;
                }
                VirtualKeyCode? modKey = null;
                switch(((ComboBoxItem)ddlAddActionModifierToSend.SelectedValue).Content)
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
                        modKey, null, HotkeyActionType.AllKeyCommand, nextSortID);
                TheHotKey.TriggeredActions.Add(act);
            }
                break;
            case HotkeyActionType.AssistActiveWindow:
            {
                if(ddlAddActionGroupName.SelectedValue == null)
                {
                    MessageBox.Show("Please make all selections before adding");
                    return;
                }
                var act = new HotKeyAction(ddlAddActionGroupName.SelectedValue.ToString(), null, null,
                        null, null, HotkeyActionType.AssistActiveWindow, nextSortID);
                TheHotKey.TriggeredActions.Add(act);
            }
                break;
            case HotkeyActionType.DisableAllHotkeys:
            {
                var act = new HotKeyAction(null, null, null,
                        null, null, HotkeyActionType.DisableAllHotkeys, nextSortID);
                TheHotKey.TriggeredActions.Add(act);
            }
                break;
            case HotkeyActionType.EchoSay:
            {
                var act = new HotKeyAction(null, null, null,
                        null, null, HotkeyActionType.EchoSay, nextSortID);
                TheHotKey.TriggeredActions.Add(act);
            }
                break;
            case HotkeyActionType.EnableAllHotkeys:
            {
                var act = new HotKeyAction(null, null, null,
                        null, null, HotkeyActionType.EnableAllHotkeys, nextSortID);
                TheHotKey.TriggeredActions.Add(act);
            }
                break;
            case HotkeyActionType.GroupKeyCommand:
            {
                if(ddlAddActionKeyToSend.SelectedValue == null || 
                        ddlAddActionModifierToSend.SelectedValue == null ||
                        ddlAddActionGroupName.SelectedValue == null)
                {
                    MessageBox.Show("Please make all selections before adding");
                    return;
                }
                VirtualKeyCode? modKey = null;
                switch(((ComboBoxItem)ddlAddActionModifierToSend.SelectedValue).Content)
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

                var act = new HotKeyAction(ddlAddActionGroupName.SelectedValue.ToString(), null, 
                        (VirtualKeyCode)ddlAddActionKeyToSend.SelectedValue,
                        modKey, null, HotkeyActionType.GroupKeyCommand, nextSortID);
                TheHotKey.TriggeredActions.Add(act);
            }
                break;
            case HotkeyActionType.InviteAllWindowsToGroup:
            {
                var act = new HotKeyAction(null, null, null,
                        null, null, HotkeyActionType.InviteAllWindowsToGroup, nextSortID);
                TheHotKey.TriggeredActions.Add(act);
            }
                break;
            case HotkeyActionType.PauseScript:
            {
                if(!int.TryParse(txtAddActionCount.Text, out var count))
                {
                    MessageBox.Show("Please make all selections before adding");
                    return;
                }
                var act = new HotKeyAction(null, count, null,
                        null, null, HotkeyActionType.PauseScript, nextSortID);
                TheHotKey.TriggeredActions.Add(act);
            }
                break;
            case HotkeyActionType.SlashCommand:
            {
                if(ddlAddActionGroupName.SelectedValue == null || string.IsNullOrEmpty(txtAddActionText.Text))
                {
                    MessageBox.Show("Please make all selections before adding");
                    return;
                }
                var act = new HotKeyAction(ddlAddActionGroupName.SelectedValue.ToString(), null, null,
                        null, txtAddActionText.Text, HotkeyActionType.SlashCommand, nextSortID);
                TheHotKey.TriggeredActions.Add(act);
            }
                break;
            case HotkeyActionType.SlashPrompt:
            {
                var act = new HotKeyAction(null, null, null,
                        null, null, HotkeyActionType.SlashPrompt, nextSortID);
                TheHotKey.TriggeredActions.Add(act);
            }
                break;
            case HotkeyActionType.TargetActiveWindow:
            {
                if(ddlAddActionGroupName.SelectedValue == null)
                {
                    MessageBox.Show("Please make all selections before adding");
                    return;
                }
                var act = new HotKeyAction(ddlAddActionGroupName.SelectedValue.ToString(), null, null,
                        null, null, HotkeyActionType.TargetActiveWindow, nextSortID);
                TheHotKey.TriggeredActions.Add(act);
            }
                break;
            case HotkeyActionType.ToggleAllHotkeysOnOff:
            {
                var act = new HotKeyAction(null, null, null,
                        null, null, HotkeyActionType.ToggleAllHotkeysOnOff, nextSortID);
                TheHotKey.TriggeredActions.Add(act);
            }
                break;
        }
        ddlAddActionType.SelectedValue = null;
        ResetAllActionDropdowns();
    }

    void RemoveAction(object choice)
    {
        if(choice == null)
        {
            return;
        }
        var targetAction = (HotKeyAction)choice;
        TheHotKey.TriggeredActions.Remove(targetAction);
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
        if(ddlAddActionType.SelectedValue == null || !Enum.TryParse(ddlAddActionType.SelectedValue.ToString(), out HotkeyActionType selectedActionType))
        {
            return;
        }

        switch(selectedActionType)
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
        if(string.IsNullOrEmpty(txtHotKey.Text))
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
        switch(ddlKeyMod.SelectedValue)
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

    private string previousActionCountText = "0";

    private void txtAddActionCount_TextChanged(object sender, TextChangedEventArgs e)
    {
        int count = 0;
        if(string.IsNullOrEmpty(txtAddActionCount.Text) || int.TryParse(txtAddActionCount.Text, out count))
        {
            // Empty || numeric is fine
            previousActionCountText = count.ToString();
        }
        else
        {
            txtAddActionCount.Text = previousActionCountText;
            txtAddActionCount.CaretIndex = txtAddActionCount.Text.Length;
        }
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Are you sure? This will return the hotkey to the void from which we all come and ultimately return to. Do not weep for this hotkey for its suffering is at an end, yours is only begining. ", "Delete Hotkey", MessageBoxButton.OKCancel);
        if(result == MessageBoxResult.OK)
        {
            DeleteMe = true;
            this.Close();
        }
    }
}
