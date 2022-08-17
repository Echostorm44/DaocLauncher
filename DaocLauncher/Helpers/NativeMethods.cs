using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace DaocLauncher.Helpers
{
    internal static class NativeMethods
    {
        const string Advapi32 = "advapi32.dll";
        const string Kernel32 = "kernel32.dll";
        const string Wtsapi32 = "wtsapi32.dll";
        const string Userenv = "userenv.dll";

        #region constants

        public const uint JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 0x00002000;
        public const uint SE_PRIVILEGE_DISABLED = 0x00000000;
        public const uint SE_PRIVILEGE_ENABLED = 0x00000002;
        public const int ERROR_SUCCESS = 0x0;
        public const int ERROR_ACCESS_DENIED = 0x5;
        public const int ERROR_NOT_ENOUGH_MEMORY = 0x8;
        public const int ERROR_NO_TOKEN = 0x3f0;
        public const int ERROR_NOT_ALL_ASSIGNED = 0x514;
        public const int ERROR_NO_SUCH_PRIVILEGE = 0x521;
        public const int ERROR_CANT_OPEN_ANONYMOUS = 0x543;
        public const uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        public const uint STANDARD_RIGHTS_READ = 0x00020000;
        public const uint NORMAL_PRIORITY_CLASS = 0x0020;
        public const uint CREATE_UNICODE_ENVIRONMENT = 0x00000400;
        public const uint MAX_PATH = 260;
        public const uint CREATE_NO_WINDOW = 0x08000000;
        public const uint INFINITE = 0xFFFFFFFF;

        #endregion

        #region Advapi32

        [DllImport(Advapi32, ExactSpelling = true, SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static extern bool AdjustTokenPrivileges(SafeTokenHandle TokenHandle, bool DisableAllPrivileges,
            IntPtr NewState, uint BufferLength, IntPtr PreviousState, out uint ReturnLength);

        [DllImport(Advapi32, ExactSpelling = true, SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static extern bool RevertToSelf();

        [DllImport(Advapi32, CharSet = CharSet.Unicode, SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, out LUID Luid);

        [DllImport(Advapi32, ExactSpelling = true, SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static extern bool OpenProcessToken(IntPtr ProcessToken, TokenAccessLevels DesiredAccess, out SafeTokenHandle TokenHandle);

        [DllImport(Advapi32, ExactSpelling = true, SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static extern bool OpenThreadToken(IntPtr ThreadToken, TokenAccessLevels DesiredAccess, bool OpenAsSelf, out SafeTokenHandle TokenHandle);

        [DllImport(Advapi32, ExactSpelling = true, SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static extern bool DuplicateTokenEx(SafeTokenHandle ExistingToken, TokenAccessLevels DesiredAccess,
            IntPtr TokenAttributes, SecurityImpersonationLevel ImpersonationLevel, TokenType TokenType, out SafeTokenHandle NewToken);

        [DllImport(Advapi32, ExactSpelling = true, SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static extern bool SetThreadToken(IntPtr Thread, SafeTokenHandle Token);

        [DllImport(Advapi32, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CreateProcessAsUser(SafeTokenHandle hToken,
            StringBuilder appExeName, StringBuilder commandLine, IntPtr processAttributes,
            IntPtr threadAttributes, bool inheritHandles, uint dwCreationFlags,
            EnvironmentBlockSafeHandle environment, string currentDirectory, ref STARTUPINFO startupInfo,
            out PROCESS_INFORMATION startupInformation);

        [DllImport(Advapi32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetTokenInformation(IntPtr TokenHandle,
            TokenInformationClass TokenInformationClass, out int TokenInformation,
            uint TokenInformationLength, out uint ReturnLength);

        #endregion

        #region Kernel32

        [DllImport(Kernel32, ExactSpelling = true, SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport(Kernel32, ExactSpelling = true, SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static extern IntPtr GetCurrentProcess();

        [DllImport(Kernel32, ExactSpelling = true, SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static extern IntPtr GetCurrentThread();

        [DllImport(Kernel32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern SafeJobHandle CreateJobObject(IntPtr lpJobAttributes, string lpName);

        [DllImport(Kernel32, SetLastError = true)]
        public static extern bool SetInformationJobObject(SafeJobHandle hJob, JobObjectInfoType infoType,
            ref JOBOBJECT_EXTENDED_LIMIT_INFORMATION lpJobObjectInfo, int cbJobObjectInfoLength);

        [DllImport(Kernel32, SetLastError = true)]
        public static extern bool AssignProcessToJobObject(SafeJobHandle job, IntPtr process);

        #endregion

        #region Wtsapi

        [DllImport(Wtsapi32, ExactSpelling = true, SetLastError = true)]
        public static extern bool WTSQueryUserToken(int sessionid, out SafeTokenHandle handle);

        #endregion

        #region Userenv

        [DllImport(Userenv, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CreateEnvironmentBlock(out EnvironmentBlockSafeHandle lpEnvironment, SafeTokenHandle hToken, bool bInherit);

        [DllImport(Userenv, ExactSpelling = true, SetLastError = true)]
        public extern static bool DestroyEnvironmentBlock(IntPtr hEnvironment);

        #endregion

        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        public struct IO_COUNTERS
        {
            public ulong ReadOperationCount;
            public ulong WriteOperationCount;
            public ulong OtherOperationCount;
            public ulong ReadTransferCount;
            public ulong WriteTransferCount;
            public ulong OtherTransferCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct JOBOBJECT_BASIC_LIMIT_INFORMATION
        {
            public long PerProcessUserTimeLimit;
            public long PerJobUserTimeLimit;
            public uint LimitFlags;
            public UIntPtr MinimumWorkingSetSize;
            public UIntPtr MaximumWorkingSetSize;
            public uint ActiveProcessLimit;
            public UIntPtr Affinity;
            public uint PriorityClass;
            public uint SchedulingClass;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public uint nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
        {
            public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
            public IO_COUNTERS IoInfo;
            public UIntPtr ProcessMemoryLimit;
            public UIntPtr JobMemoryLimit;
            public UIntPtr PeakProcessMemoryUsed;
            public UIntPtr PeakJobMemoryUsed;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwYSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID
        {
            public uint LowPart;
            public uint HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID_AND_ATTRIBUTES
        {
            public LUID Luid;
            public uint Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TOKEN_PRIVILEGE
        {
            public uint PrivilegeCount;
            public LUID_AND_ATTRIBUTES Privilege;
        }

        #endregion

        #region Enums

        public enum JobObjectInfoType
        {
            AssociateCompletionPortInformation = 7,
            BasicLimitInformation = 2,
            BasicUIRestrictions = 4,
            EndOfJobTimeInformation = 6,
            ExtendedLimitInformation = 9,
            SecurityLimitInformation = 5,
            GroupInformation = 11
        }

        public enum SecurityImpersonationLevel
        {
            Anonymous = 0,
            Identification = 1,
            Impersonation = 2,
            Delegation = 3,
        }

        public enum TokenType
        {
            Primary = 1,
            Impersonation = 2,
        }

        public enum TokenInformationClass
        {
            TokenUser = 1,
            TokenGroups,
            TokenPrivileges,
            TokenOwner,
            TokenPrimaryGroup,
            TokenDefaultDacl,
            TokenSource,
            TokenType,
            TokenImpersonationLevel,
            TokenStatistics,
            TokenRestrictedSids,
            TokenSessionId,
            TokenGroupsAndPrivileges,
            TokenSessionReference,
            TokenSandBoxInert,
            TokenAuditPolicy,
            TokenOrigin,
            TokenElevationType,
            TokenLinkedToken,
            TokenElevation,
            TokenHasRestrictions,
            TokenAccessInformation,
            TokenVirtualizationAllowed,
            TokenVirtualizationEnabled,
            TokenIntegrityLevel,
            TokenUIAccess,
            TokenMandatoryPolicy,
            TokenLogonSid,
            TokenIsAppContainer,
            TokenCapabilities,
            TokenAppContainerSid,
            TokenAppContainerNumber,
            TokenUserClaimAttributes,
            TokenDeviceClaimAttributes,
            TokenRestrictedUserClaimAttributes,
            TokenRestrictedDeviceClaimAttributes,
            TokenDeviceGroups,
            TokenRestrictedDeviceGroups,
            // MaxTokenInfoClass should always be the last enum
            MaxTokenInfoClass
        }

        #endregion

        #region SafeHandles

        public sealed class EnvironmentBlockSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public EnvironmentBlockSafeHandle()
                : base(true)
            {
            }

            protected override bool ReleaseHandle()
            {
                return DestroyEnvironmentBlock(handle);
            }
        }

        public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public SafeTokenHandle()
                : base(true)
            {
            }

            override protected bool ReleaseHandle()
            {
                return CloseHandle(handle);
            }
        }

        internal sealed class SafeJobHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public SafeJobHandle()
                : base(true)
            {
            }

            protected override bool ReleaseHandle()
            {
                return CloseHandle(this.handle);
            }
        }

        #endregion

        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);
        [DllImport("user32.dll")]
        public static extern bool EnumDisplayDevices(string deviceName, int modeNum, ref DISPLAY_DEVICE displayDevice, int flags);

        const int ENUM_CURRENT_SETTINGS = -1;

        const int ENUM_REGISTRY_SETTINGS = -2;

        [Flags()]
        public enum DisplayDeviceStateFlags : int
        {
            /// <summary>The device is part of the desktop.</summary>
            AttachedToDesktop = 0x1,
            MultiDriver = 0x2,
            /// <summary>The device is part of the desktop.</summary>
            PrimaryDevice = 0x4,
            /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
            MirroringDriver = 0x8,
            /// <summary>The device is VGA compatible.</summary>
            VGACompatible = 0x16,
            /// <summary>The device is removable; it cannot be the primary display.</summary>
            Removable = 0x20,
            /// <summary>The device has more display modes than its output devices support.</summary>
            ModesPruned = 0x8000000,
            Remote = 0x4000000,
            Disconnect = 0x2000000
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public System.Windows.Forms.ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAY_DEVICE
        {
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            [MarshalAs(UnmanagedType.U4)]
            public DisplayDeviceStateFlags StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        public static List<DISPLAY_DEVICE> GetGraphicsAdapters()
        {
            int i = 0;
            DISPLAY_DEVICE displayDevice = new DISPLAY_DEVICE();
            List<DISPLAY_DEVICE> result = new List<DISPLAY_DEVICE>();
            displayDevice.cb = Marshal.SizeOf(displayDevice);
            while (NativeMethods.EnumDisplayDevices(null, i, ref displayDevice, 1))
            {
                result.Add(displayDevice);
                i++;
            }

            return result;
        }

        public static List<DISPLAY_DEVICE> GetMonitors(string graphicsAdapter)
        {

            DISPLAY_DEVICE displayDevice = new DISPLAY_DEVICE();
            List<DISPLAY_DEVICE> result = new List<DISPLAY_DEVICE>();
            int i = 0;
            displayDevice.cb = Marshal.SizeOf(displayDevice);
            while (NativeMethods.EnumDisplayDevices(graphicsAdapter, i, ref displayDevice, 0))
            {
                result.Add(displayDevice);
                i++;
            }

            return result;
        }

        public static List<DEVMODE> GetDeviceModes(string graphicsAdapter)
        {
            int i = 0;
            DEVMODE devMode = new DEVMODE();
            List<DEVMODE> result = new List<DEVMODE>();
            while (NativeMethods.EnumDisplaySettings(graphicsAdapter, i, ref devMode))
            {
                result.Add(devMode);
                i++;
            }
            return result;
        }

        public static void GetScreenResolutions()
        {
            DEVMODE vDevMode = new DEVMODE();
            int i = 0;
            while (EnumDisplaySettings(null, i, ref vDevMode))
            {
                var foo = vDevMode;
                Console.WriteLine("Width:{0} Height:{1} Color:{2} Frequency:{3}",
                                        vDevMode.dmPelsWidth,
                                        vDevMode.dmPelsHeight,
                                        1 << vDevMode.dmBitsPerPel, vDevMode.dmDisplayFrequency
                                    );
                i++;
            }
        }
    }
}