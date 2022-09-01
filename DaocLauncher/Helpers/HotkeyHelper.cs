using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Xml.Linq;

namespace DaocLauncher.Helpers
{
    [Flags]
    public enum KeyModifier
    {
        None = 0x0000,
        Alt = 0x0001,
        Ctrl = 0x0002,
        Shift = 0x0004,
    }

    public enum HotkeyActionType
    {
        AssistActiveWindow, // (group
        TargetActiveWindow,
        PauseScript,         // pause script for x milliseconds
        GroupKeyCommand, // (group, key, modifier
        SlashCommand,//face
        AllKeyCommand, // send a key to all (like sprint f
        EchoSay, // pop an input text box and echo the results to all windows with a /say
        InviteAllWindowsToGroup,  //Invites all windows to a group
        SlashPrompt, // Will echo the slash command given to all windows
        DisableAllHotkeys, // Toggle, Disable and Enable everything so the user can type normally.  
        EnableAllHotkeys,  // Main use should be to toggle on Enter and /
        ToggleAllHotkeysOnOff,
    }

    public class HotKeyAction : IDragAndDropListBoxItem, ICloneable
    {
        public string? GroupName { get; set; } // Casters, healers
        public int? Count { get; set; } // for use with things like pause
        public VirtualKeyCode? KeyToSend { get; set; } // 1 2
        public VirtualKeyCode? ModifierKeyToSend { get; set; } // shift alt
        public string? Text { get; set; } // For things like slash command
        public HotkeyActionType ActionType { get; set; }
        public int SortOrderID { get; set; }

        /// <summary>
        /// HotKeyAction
        /// </summary>
        /// <param name="groupName">Who does this target? eg Casters, Healers, PBAOE</param>
        /// <param name="count">For things like pause</param>
        /// <param name="keyToSend">The keyboard key being sent eg 1, 2, F</param>
        /// <param name="modifierKeyToSend">A modifier key so we can send something like Shift-1 or Alt-f</param>
        /// <param name="text">For use with things like slash command</param>
        /// <param name="actionType">From the enum</param>
        public HotKeyAction(string? groupName, int? count, VirtualKeyCode? keyToSend, VirtualKeyCode? modifierKeyToSend, string? text, HotkeyActionType actionType, int sortID)
        {
            GroupName = groupName;
            Count = count;
            KeyToSend = keyToSend;
            ModifierKeyToSend = modifierKeyToSend;
            Text = text;
            ActionType = actionType;
            SortOrderID = sortID;
        }

        public override string ToString()
        {
            string result = "";
            switch(ActionType)
            {
                case HotkeyActionType.AssistActiveWindow:
                    result += $"{GroupName} Assist The Active Window";
                    break;
                case HotkeyActionType.TargetActiveWindow:
                    result += $"{GroupName} Target The Active Window";
                    break;
                case HotkeyActionType.PauseScript:
                    result += $"Pause Script For {Count} ms";
                    break;
                case HotkeyActionType.GroupKeyCommand:
                    result += $"Send { (ModifierKeyToSend.HasValue ? ModifierKeyToSend.Value + " + " : "") +  KeyToSend?.ToString()?.Replace("VK_","") } To {GroupName}";
                    break;
                case HotkeyActionType.SlashCommand:
                    result += $"Send {Text} Command To {GroupName}";
                    break;
                case HotkeyActionType.AllKeyCommand:
                    result += $"Send {(ModifierKeyToSend.HasValue ? ModifierKeyToSend.Value + " + " : "") + KeyToSend?.ToString()?.Replace("VK_", "") } To All Windows";
                    break;
                case HotkeyActionType.EchoSay:
                    result += $"Prompt And Send Say";
                    break;
                case HotkeyActionType.InviteAllWindowsToGroup:
                    result += $"Invite Other Windows To Group";
                    break;
                case HotkeyActionType.SlashPrompt:
                    result += $"Prompt And Send Slash Command";
                    break;
                case HotkeyActionType.DisableAllHotkeys:
                    result += "Disable Hotkeys";
                    break;
                case HotkeyActionType.EnableAllHotkeys:
                    result += "Enable Hotkeys";
                    break;
                case HotkeyActionType.ToggleAllHotkeysOnOff:
                    result += "Toggle Hotkeys";
                    break;
            }
            return result;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public HotKeyAction()
        {
        }
    }

    public class HotKey : IDisposable, INotifyPropertyChanged
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, UInt32 fsModifiers, UInt32 vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("kernel32", SetLastError = true)]
        public static extern short GlobalAddAtom(string lpString);

        [DllImport("kernel32", SetLastError = true)]
        public static extern short GlobalDeleteAtom(short nAtom);

        public const int WmHotKey = 0x0312;

        private bool AmIDisposed = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public List<HotKey> AllHotKeys { get; set; }

        Key key;
        public Key Key
        {
            get => key;
            set
            {
                if(key == value)
                {
                    return;
                }

                key = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Key)));
            }
        }
        KeyModifier keyModifiers;
        public KeyModifier KeyModifiers
        {
            get => keyModifiers;
            set
            {
                if(keyModifiers == value)
                {
                    return;
                }

                keyModifiers = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(KeyModifiers)));
            }
        }
        public Action<HotKey>? HotKeyActionEvent { get; set; }
        public int Id { get; set; }// AMM TODO might need a better unique id, this is more for the system call
        string description;
        public string Description
        {
            get => description;
            set
            {
                if(description == value)
                {
                    return;
                }

                description = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Description)));
            }
        }
        public ObservableCollection<HotKeyAction> TriggeredActions { get; set; }

        public HotKey()
        {
        }

        public HotKey(Key k, KeyModifier keyModifiers, string description, ObservableCollection<HotKeyAction> actions)
        {
            TriggeredActions = actions;
            Key = k;
            KeyModifiers = keyModifiers;
            Description = description;
            int virtualKeyCode = KeyInterop.VirtualKeyFromKey(k);
            Id = virtualKeyCode + ((int)keyModifiers * 0x10000);
        }

        public override string ToString()
        {
            var result = "";
            result = Key.ToString() + (KeyModifiers == KeyModifier.None ? "" : " + " + KeyModifiers.ToString()) + " : " + Description;
            return result;
        }

        public bool Register(Action<HotKey> action)
        {
            HotKeyActionEvent = action;
            int virtualKeyCode = KeyInterop.VirtualKeyFromKey(Key);
            Id = virtualKeyCode + ((int)KeyModifiers * 0x10000);
            bool result = RegisterHotKey(IntPtr.Zero, Id, (UInt32)KeyModifiers, (UInt32)virtualKeyCode);
            if(!result)
            {
                var hr = Marshal.GetHRForLastWin32Error();
                var ex = Marshal.GetExceptionForHR(hr);
                var whatNow = "";
            }
            //ComponentDispatcher.ThreadFilterMessage += new ThreadMessageEventHandler(ComponentDispatcherThreadFilterMessage);
            return result;
        }

        public void Unregister()
        {
            //ComponentDispatcher.ThreadFilterMessage -= new ThreadMessageEventHandler(ComponentDispatcherThreadFilterMessage);
            UnregisterHotKey(IntPtr.Zero, Id);
        }

        private void ComponentDispatcherThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if(!handled)
            {
                if(msg.message == WmHotKey)
                {
                    var id = (int)msg.wParam;
                    var targetKey = AllHotKeys.SingleOrDefault(a => a.Id == id);
                    if(targetKey != null)
                    {
                        if(targetKey.HotKeyActionEvent != null)
                        {
                            handled = true;
                            targetKey.HotKeyActionEvent.Invoke(targetKey);
                        }
                    }
                    handled = true;
                }
            }
        }

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be _disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be _disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if(!this.AmIDisposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if(disposing)
                {
                    // Dispose managed resources.
                    Unregister();
                }

                // Note disposing has been done.
                AmIDisposed = true;
            }
        }
    }
}
