using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace DaocLauncher.Helpers
{
    [Flags]
    public enum KeyModifier
    {
        None = 0x0000,
        Alt = 0x0001,
        Ctrl = 0x0002,
        NoRepeat = 0x4000,
        Shift = 0x0004,
        Win = 0x0008
    }

    public enum HotkeyActionType
    {
        AssistActiveWindow, // (group
        TargetActiveWindow,
        Pause,         // pause script for x milliseconds
        SlashCommand, // text
        GroupCommand, // (group, key, modifier
        AllKeyCommand, // send a key to all (like sprint f
        EchoText, // pop an input text box and echo the results to all windows with a /say
        InviteGroup,  //Invites all windows to a group
        SlashPrompt, // Will echo the slash command given to all windows
        Disable, // Toggle, Disable and Enable everything so the user can type normally.  
        Enable,  // Main use should be to toggle on Enter and /
        ToggleAllHotkeys,
    }

    public class HotKeyAction
    {
        public string? GroupName { get; set; } // Casters, healers
        public int? Count { get; set; } // for use with things like pause
        public VirtualKeyCode? KeyToSend { get; set; } // 1 2
        public VirtualKeyCode? ModifierKeyToSend { get; set; } // shift alt
        public string? Text { get; set; } // For things like slash command
        public HotkeyActionType ActionType { get; set; }

        /// <summary>
        /// HotKeyAction
        /// </summary>
        /// <param name="groupName">Who does this target? eg Casters, Healers, PBAOE</param>
        /// <param name="count">For things like pause</param>
        /// <param name="keyToSend">The keyboard key being sent eg 1, 2, F</param>
        /// <param name="modifierKeyToSend">A modifier key so we can send something like Shift-1 or Alt-f</param>
        /// <param name="text">For use with things like slash command</param>
        /// <param name="actionType">From the enum</param>
        public HotKeyAction(string? groupName, int? count, VirtualKeyCode? keyToSend, VirtualKeyCode? modifierKeyToSend, string? text, HotkeyActionType actionType)
        {
            GroupName = groupName;
            Count = count;
            KeyToSend = keyToSend;
            ModifierKeyToSend = modifierKeyToSend;
            Text = text;
            ActionType = actionType;
        }

        public HotKeyAction()
        {
            
        }
    }

    public class HotKey : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, UInt32 fsModifiers, UInt32 vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public const int WmHotKey = 0x0312;

        private bool AmIDisposed = false;
        public Key Key { get; private set; }
        public KeyModifier KeyModifiers { get; private set; }
        public Action<HotKey>? HotKeyAction { get; private set; }
        public int Id { get; set; }

        public HotKey(Key k, KeyModifier keyModifiers)
        {
            Key = k;
            KeyModifiers = keyModifiers;            
        }

        public bool Register(Action<HotKey> action)
        {
            HotKeyAction = action;
            int virtualKeyCode = KeyInterop.VirtualKeyFromKey(Key);
            Id = virtualKeyCode + ((int)KeyModifiers * 0x10000);
            bool result = RegisterHotKey(IntPtr.Zero, Id, (UInt32)KeyModifiers, (UInt32)virtualKeyCode);
            ComponentDispatcher.ThreadFilterMessage += new ThreadMessageEventHandler(ComponentDispatcherThreadFilterMessage);
            return result;
        }

        public void Unregister()
        {
            UnregisterHotKey(IntPtr.Zero, Id);
        }

        private void ComponentDispatcherThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (!handled)
            {
                if (msg.message == WmHotKey)
                {
                    if (HotKeyAction != null && (int)msg.wParam == Id)
                    {
                        HotKeyAction.Invoke(this);
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
            if (!this.AmIDisposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
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
