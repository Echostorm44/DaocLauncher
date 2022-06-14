using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DaocLauncher.Helpers
{
    public enum Keys : short
    {
        Null = 0x00,
        Esc = 0x01,
        One = 0x02,
        Exclamation = 0x02,
        Two = 0x03,
        AtSign = 0x03,
        Three = 0x04,
        HashTag = 0x04,
        Four = 0x05,
        Dollar = 0x05,
        Five = 0x06,
        Percent = 0x06,
        Six = 0x07,
        Caret = 0x07,
        Seven = 0x08,
        Ampersand = 0x08,
        Eight = 0x09,
        Asterisk = 0x09,
        Nine = 0x0A,
        LeftParen = 0x0A,
        Zero = 0x0B,
        RightParen = 0x0B,
        Minus = 0x0C,
        Underscore = 0x0C,
        Equals = 0x0D,
        Plus = 0x0D,
        Backspace = 0x0E,
        Tab = 0x0F,
        Q = 0x10,
        W = 0x11,
        E = 0x12,
        R = 0x13,
        T = 0x14,
        Y = 0x15,
        U = 0x16,
        I = 0x17,
        O = 0x18,
        P = 0x19,
        LeftBracket = 0x1A,
        LeftBrace = 0x1A,
        RightBracket = 0x1B,
        RightBrace = 0x1B,
        Enter = 0x1C,         //
        LeftControl = 0x1D,
        RightControl = 0x1D,    //
        A = 0x1E,
        S = 0x1F,
        D = 0x20,
        F = 0x21,
        G = 0x22,
        H = 0x23,
        J = 0x24,
        K = 0x25,
        L = 0x26,
        Colon = 0x27,
        SemiColon = 0x27,
        SingleQuote = 0x28,
        DoubleQuote = 0x28,
        BackTick = 0x29,
        Tilde = 0x29,
        LeftShift = 0x2A,
        BackSlash = 0x2B,
        Pipe = 0x2B,
        Z = 0x2C,
        X = 0x2D,
        C = 0x2E,
        V = 0x2F,
        B = 0x30,
        N = 0x31,
        M = 0x32,
        Comma = 0x33,
        LessThan = 0x33,
        Period = 0x34,
        GreaterThan = 0x34,
        ForwardSlash = 0x35,
        Question = 0x35,
        RightShift = 0x36,
        PrintScreen = 0x37,
        LeftAlt = 0x38,
        RightAlt = 0x38,
        Space = 0x39,
        Capslock = 0x3A,
        F1 = 0x3B,
        F2 = 0x3C,
        F3 = 0x3D,
        F4 = 0x3E,
        F5 = 0x3F,
        F6 = 0x40,
        F7 = 0x41,
        F8 = 0x42,
        F9 = 0x43,
        F10 = 0x44,
        F11 = 0x57,
        F12 = 0x58,
        NumLock = 0x45,
        ScrollLock = 0x46,
        Home = 0x47,
        Up = 0x48,
        PageUp = 0x49,
        Left = 0x4B,
        Center = 0x4C,
        Right = 0x4D,
        End = 0x4F,
        Down = 0x50,
        PageDown = 0x51,
        Insert = 0x52,
        Delete = 0x53
    }

    public enum KeyboardFlag : int
    {
        KEYEVENTF_KEYDOWN = 0x0000,
        KEYEVENTF_EXTENDEDKEY = 0x0001,
        KEYEVENTF_KEYUP = 0x0002,
        KEYEVENTF_UNICODE = 0x0004,
        KEYEVENTF_SCANCODE = 0x0008
    }
    public static class SendKeys
    {
        [DllImport("user32.dll")]
        static extern UInt32 SendInput(UInt32 nInputs, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] INPUT[] pInputs, Int32 cbSize);

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        // If 32 bit, FieldOffset = 4; else FieldOffset = 8
        [StructLayout(LayoutKind.Explicit)]
        struct INPUT
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(8)]
            public MOUSEINPUT mi;
            [FieldOffset(8)]
            public KEYBDINPUT ki;
            [FieldOffset(8)]
            public HARDWAREINPUT hi;
        }

        public static void SendKey(short Keycode, bool KeyDown)
        {
            int flag;

            if (KeyDown)
                flag = (int)KeyboardFlag.KEYEVENTF_KEYDOWN;
            else
                flag = (int)KeyboardFlag.KEYEVENTF_KEYUP;

            INPUT[] InputData = new INPUT[1];

            InputData[0].type = 1;
            InputData[0].ki.wScan = Keycode;
            InputData[0].ki.dwFlags = flag;
            InputData[0].ki.time = 0;
            InputData[0].ki.dwExtraInfo = IntPtr.Zero;

            SendInput(1, InputData, Marshal.SizeOf(typeof(INPUT)));
        }
    }
}
