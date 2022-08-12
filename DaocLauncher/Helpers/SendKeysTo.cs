using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DaocLauncher.Helpers
{
    public class SendKeysTo
    {
        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, uint msg, uint wParam, uint lParam);

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);
        
        const uint WM_KEYDOWN = 0x0100;
        const uint WM_KEYUP = 0x0101;
        const uint WM_CHAR = 0x0102;
        const uint WM_SETFOCUS = 0x0007;
        const uint WM_KILLFOCUS = 0x0008;

        object sendLock = 1;


        /// <summary>
        /// Send keys to target window
        /// </summary>
        /// <param name="targetWindow">Result of FindWindow or similar</param>
        /// <param name="key"></param>
        /// <param name="modiferKey">VirtualKeyCode like Shift, Alt or Control</param>
        public void SendThoseKeysSucka(IntPtr targetWindow, VirtualKeyCode key, VirtualKeyCode? modiferKey, IntPtr returnFocusWindow)
        {            
            lock (sendLock)
            {
                SendMessage(targetWindow, WM_SETFOCUS, returnFocusWindow, IntPtr.Zero);
                uint scanCode = MapVirtualKey((uint)key, 0);
                uint lParam = (0x00000001 | (scanCode << 16));
                //if (extended)
                //{
                //    lParam |= 0x01000000;
                //}
                if (modiferKey != null)
                {
                    keybd_event((byte)modiferKey, 0, WM_KEYDOWN, 0); ;
                    Thread.Sleep(1);
                }
                SendMessage(targetWindow, WM_KEYDOWN, (uint)key, lParam);
                Thread.Sleep(1);
                SendMessage(targetWindow, WM_CHAR, (uint)key, lParam);
                Thread.Sleep(1);
                SendMessage(targetWindow, WM_KEYUP, (uint)key, lParam);
                if (modiferKey != null)
                {
                    Thread.Sleep(1);
                    keybd_event((byte)modiferKey, 0, WM_KEYUP, 0);
                }
                SendMessage(targetWindow, WM_KILLFOCUS, returnFocusWindow, IntPtr.Zero);
                SendMessage(returnFocusWindow, WM_SETFOCUS, targetWindow, IntPtr.Zero);
            }
        }

        /// <summary>
        /// Send keys to target window, this one is for putting keys into an already open chat box
        /// </summary>
        /// <param name="targetWindow">Result of FindWindow or similar</param>
        /// <param name="keysToSend"></param>
        public void SendThoseChatKeysSucka(IntPtr targetWindow, string keysToSend, IntPtr returnFocusWindow)
        {
            lock (sendLock)
            {
                SendMessage(targetWindow, WM_SETFOCUS, returnFocusWindow, IntPtr.Zero);
                foreach (var c in keysToSend.ToCharArray())
                {                    
                    uint scanCode = MapVirtualKey((uint)c, 0);
                    uint lParam = (0x00000001 | (scanCode << 16));              
                    SendMessage(targetWindow, WM_CHAR, (uint)c, lParam);
                }
                SendMessage(targetWindow, WM_KILLFOCUS, returnFocusWindow, IntPtr.Zero);
                SendMessage(returnFocusWindow, WM_SETFOCUS, targetWindow, IntPtr.Zero);
            }
        }


        public void JustSendKey(IntPtr targetWindow, VirtualKeyCode key)
        {
            uint scanCode = MapVirtualKey((uint)key, 0);
            uint lParam = (0x00000001 | (scanCode << 16));
            SendMessage(targetWindow, WM_KEYDOWN, (uint)key, lParam);
            Thread.Sleep(1);
            SendMessage(targetWindow, WM_CHAR, (uint)key, lParam);
            Thread.Sleep(1);
            SendMessage(targetWindow, WM_KEYUP, (uint)key, lParam);
        }

        /// <summary>
        /// Optimized version to prevent using more focus calls than needed to do a chat / command
        /// </summary>
        /// <param name="targetWindow">Result of FindWindow or similar</param>
        /// <param name="keysToSend">eg /stick </param>
        public void SendChatCommand(IntPtr targetWindow, string keysToSend, IntPtr returnFocusWindow)
        {
            lock (sendLock)
            {
                SendMessage(targetWindow, WM_SETFOCUS, returnFocusWindow, IntPtr.Zero);
                JustSendKey(targetWindow, VirtualKeyCode.RETURN);
                foreach (var c in keysToSend.ToCharArray())
                {
                    uint scanCode = MapVirtualKey((uint)c, 0);
                    uint lParam = (0x00000001 | (scanCode << 16));
                    SendMessage(targetWindow, WM_CHAR, (uint)c, lParam);
                }
                JustSendKey(targetWindow, VirtualKeyCode.RETURN);
                SendMessage(targetWindow, WM_KILLFOCUS, returnFocusWindow, IntPtr.Zero);
                SendMessage(returnFocusWindow, WM_SETFOCUS, targetWindow, IntPtr.Zero);
            }
        }

    }
}
