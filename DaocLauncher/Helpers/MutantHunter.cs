using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DaocLauncher.Helpers
{
    internal class MutantHunter
    {
        #region Enums
        public enum OBJECT_INFORMATION_CLASS
        {
            ObjectBasicInformation,
            ObjectNameInformation,
            ObjectTypeInformation,
            ObjectTypesInformation,
            ObjectHandleFlagInformation,
            ObjectSessionInformation,
        }

        public enum SYSTEM_INFORMATION_CLASS
        {
            SystemBasicInformation,
            SystemProcessorInformation,
            SystemPerformanceInformation,
            SystemTimeOfDayInformation,
            SystemPathInformation,
            SystemProcessInformation,
            SystemCallCountInformation,
            SystemDeviceInformation,
            SystemProcessorPerformanceInformation,
            SystemFlagsInformation,
            SystemCallTimeInformation,
            SystemModuleInformation,
            SystemLocksInformation,
            SystemStackTraceInformation,
            SystemPagedPoolInformation,
            SystemNonPagedPoolInformation,
            SystemHandleInformation,
            SystemObjectInformation,
            SystemPageFileInformation,
            SystemVdmInstemulInformation,
            SystemVdmBopInformation,
            SystemFileCacheInformation,
            SystemPoolTagInformation,
            SystemInterruptInformation,
            SystemDpcBehaviorInformation,
            SystemFullMemoryInformation,
            SystemLoadGdiDriverInformation,
            SystemUnloadGdiDriverInformation,
            SystemTimeAdjustmentInformation,
            SystemSummaryMemoryInformation,
            SystemMirrorMemoryInformation,
            SystemPerformanceTraceInformation,
            SystemObsolete0,
            SystemExceptionInformation,
            SystemCrashDumpStateInformation,
            SystemKernelDebuggerInformation,
            SystemContextSwitchInformation,
            SystemRegistryQuotaInformation,
            SystemExtendServiceTableInformation,
            SystemPrioritySeperation,
            SystemVerifierAddDriverInformation,
            SystemVerifierRemoveDriverInformation,
            SystemProcessorIdleInformation,
            SystemLegacyDriverInformation,
            SystemCurrentTimeZoneInformation,
            SystemLookasideInformation,
            SystemTimeSlipNotification,
            SystemSessionCreate,
            SystemSessionDetach,
            SystemSessionInformation,
            SystemRangeStartInformation,
            SystemVerifierInformation,
            SystemVerifierThunkExtend,
            SystemSessionProcessInformation,
            SystemLoadGdiDriverInSystemSpace,
            SystemNumaProcessorMap,
            SystemPrefetcherInformation,
            SystemExtendedProcessInformation,
            SystemRecommendedSharedDataAlignment,
            SystemComPlusPackage,
            SystemNumaAvailableMemory,
            SystemProcessorPowerInformation,
            SystemEmulationBasicInformation,
            SystemEmulationProcessorInformation,
            SystemExtendedHandleInformation,
            SystemLostDelayedWriteInformation,
            SystemBigPoolInformation,
            SystemSessionPoolTagInformation,
            SystemSessionMappedViewInformation,
            SystemHotpatchInformation,
            SystemObjectSecurityMode,
            SystemWatchdogTimerHandler,
            SystemWatchdogTimerInformation,
            SystemLogicalProcessorInformation,
            SystemWow64SharedInformationObsolete,
            SystemRegisterFirmwareTableInformationHandler,
            SystemFirmwareTableInformation,
            SystemModuleInformationEx,
            SystemVerifierTriageInformation,
            SystemSuperfetchInformation,
            SystemMemoryListInformation,
            SystemFileCacheInformationEx,
            SystemThreadPriorityClientIdInformation,
            SystemProcessorIdleCycleTimeInformation,
            SystemVerifierCancellationInformation,
            SystemProcessorPowerInformationEx,
            SystemRefTraceInformation,
            SystemSpecialPoolInformation,
            SystemProcessIdInformation,
            SystemErrorPortInformation,
            SystemBootEnvironmentInformation,
            SystemHypervisorInformation,
            SystemVerifierInformationEx,
            SystemTimeZoneInformation,
            SystemImageFileExecutionOptionsInformation,
            SystemCoverageInformation,
            SystemPrefetchPatchInformation,
            SystemVerifierFaultsInformation,
            SystemSystemPartitionInformation,
            SystemSystemDiskInformation,
            SystemProcessorPerformanceDistribution,
            SystemNumaProximityNodeInformation,
            SystemDynamicTimeZoneInformation,
            SystemCodeIntegrityInformation,
            SystemProcessorMicrocodeUpdateInformation,
            SystemProcessorBrandString,
            SystemVirtualAddressInformation,
            SystemLogicalProcessorAndGroupInformation,
            SystemProcessorCycleTimeInformation,
            SystemStoreInformation,
            SystemRegistryAppendString,
            SystemAitSamplingValue,
            SystemVhdBootInformation,
            SystemCpuQuotaInformation,
            SystemNativeBasicInformation,
            SystemErrorPortTimeouts,
            SystemLowPriorityIoInformation,
            SystemBootEntropyInformation,
            SystemVerifierCountersInformation,
            SystemPagedPoolInformationEx,
            SystemSystemPtesInformationEx,
            SystemNodeDistanceInformation,
            SystemAcpiAuditInformation,
            SystemBasicPerformanceInformation,
            SystemQueryPerformanceCounterInformation,
            SystemSessionBigPoolInformation,
            SystemBootGraphicsInformation,
            SystemScrubPhysicalMemoryInformation,
            SystemBadPageInformation,
            SystemProcessorProfileControlArea,
            SystemCombinePhysicalMemoryInformation,
            SystemEntropyInterruptTimingInformation,
            SystemConsoleInformation,
            SystemPlatformBinaryInformation,
            SystemThrottleNotificationInformation,
            SystemHypervisorProcessorCountInformation,
            SystemDeviceDataInformation,
            SystemDeviceDataEnumerationInformation,
            SystemMemoryTopologyInformation,
            SystemMemoryChannelInformation,
            SystemBootLogoInformation,
            SystemProcessorPerformanceInformationEx,
            SystemSpare0,
            SystemSecureBootPolicyInformation,
            SystemPageFileInformationEx,
            SystemSecureBootInformation,
            SystemEntropyInterruptTimingRawInformation,
            SystemPortableWorkspaceEfiLauncherInformation,
            SystemFullProcessInformation,
            MaxSystemInfoClass,
        }

        public enum PROCESSINFOCLASS
        {
            ProcessBasicInformation,
            ProcessQuotaLimits,
            ProcessIoCounters,
            ProcessVmCounters,
            ProcessTimes,
            ProcessBasePriority,
            ProcessRaisePriority,
            ProcessDebugPort,
            ProcessExceptionPort,
            ProcessAccessToken,
            ProcessLdtInformation,
            ProcessLdtSize,
            ProcessDefaultHardErrorMode,
            ProcessIoPortHandlers,
            ProcessPooledUsageAndLimits,
            ProcessWorkingSetWatch,
            ProcessUserModeIOPL,
            ProcessEnableAlignmentFaultFixup,
            ProcessPriorityClass,
            ProcessWx86Information,
            ProcessHandleCount,
            ProcessAffinityMask,
            ProcessPriorityBoost,
            ProcessDeviceMap,
            ProcessSessionInformation,
            ProcessForegroundInformation,
            ProcessWow64Information,
            ProcessImageFileName,
            ProcessLUIDDeviceMapsEnabled,
            ProcessBreakOnTermination,
            ProcessDebugObjectHandle,
            ProcessDebugFlags,
            ProcessHandleTracing,
            ProcessUnusedSpare1,
            ProcessExecuteFlags,
            ProcessResourceManagement,
            ProcessCookie,
            ProcessImageInformation,
            MaxProcessInfoClass,
        }
        #endregion

        #region Classes
        [StructLayout(LayoutKind.Sequential)]
        public class PROCESS_BASIC_INFORMATION
        {
            public int ExitStatus;
            public IntPtr PebBaseAddress = (IntPtr)0;
            public IntPtr AffinityMask = (IntPtr)0;
            public int BasePriority;
            public IntPtr UniqueProcessId = (IntPtr)0;
            public IntPtr InheritedFromUniqueProcessId = (IntPtr)0;
        }
        #endregion

        #region Structs
        internal struct TOKEN_PRIVILEGES
        {
            internal int PrivilegeCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            internal int[] Privileges;
        }
        public struct LUID
        {
            public int LowPart;
            public int HighPart;
        }

        public struct HTT
        {
            public IntPtr handle;
            public string txt;
        }

        public struct GENERIC_MAPPING
        {
            public int GenericRead;
            public int GenericWrite;
            public int GenericExecute;
            public int GenericAll;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct UNICODE_STRING
        {
            public ushort Length;
            public ushort MaximumLength;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string Buffer;
        }
        public struct OBJECT_TYPE_INFORMATION
        {
            public UNICODE_STRING TypeName;
            public int TotalNumberOfObjects;
            public int TotalNumberOfHandles;
            public int TotalPagedPoolUsage;
            public int TotalNonPagedPoolUsage;
            public int TotalNamePoolUsage;
            public int TotalHandleTableUsage;
            public int HighWaterNumberOfObjects;
            public int HighWaterNumberOfHandles;
            public int HighWaterPagedPoolUsage;
            public int HighWaterNonPagedPoolUsage;
            public int HighWaterNamePoolUsage;
            public int HighWaterHandleTableUsage;
            public int InvalidAttributes;
            public GENERIC_MAPPING GenericMapping;
            public int ValidAccessMask;
            public byte SecurityRequired;
            public byte MaintainHandleCount;
            public int PoolType;
            public int DefaultPagedPoolCharge;
            public int DefaultNonPagedPoolCharge;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SYSTEM_HANDLE_INFORMATION_EX
        {
            public IntPtr Object;
            public uint UniqueProcessId;
            public uint HandleValue;
            public uint GrantedAccess;
            public ushort CreatorBackTraceIndex;
            public ushort ObjectTypeIndex;
            public uint HandleAttributes;
            public uint Reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct OBJECT_NAME_INFORMATION
        {
            public UNICODE_STRING Name;
        }
        #endregion

        #region Imports
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool QueryFullProcessImageName(IntPtr hProcess, int dwFlags, StringBuilder lpExeName, ref uint lpdwSize);
        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool IsWow64Process(IntPtr hProcess, out bool Wow64Process);

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool Wow64DisableWow64FsRedirection(out IntPtr OldValue);

        [DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool LookupPrivilegeValue([In] string lpSystemName, [In] string lpName, out LUID Luid);

        [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, bool DisableAllPrivileges, IntPtr NewState, int BufferLength, IntPtr PreviousState, ref int ReturnLength);

        [DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool OpenProcessToken(IntPtr hProcess, uint desiredAccess, out IntPtr hToken);

        [DllImport("NtDll.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int NtQuerySystemInformation(SYSTEM_INFORMATION_CLASS SystemInformationClass, IntPtr SystemInformation, int SystemInformationLength, ref int ReturnLength);

        [DllImport("NtDll.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int NtQueryInformationProcess(IntPtr ProcessHandle, PROCESSINFOCLASS ProcessInformationClass, PROCESS_BASIC_INFORMATION info, int ProcessInformationLength, int[] ReturnLength);

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateThread(IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, int dwCreationFlags, out int lpThreadId);

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool TerminateThread(IntPtr hThread, int dwExitCode);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("NtDll.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int NtQueryObject(IntPtr Handle, OBJECT_INFORMATION_CLASS ObjectInformationClass, IntPtr ObjectInformation, uint Length, ref uint ReturnLength);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, IntPtr hSourceHandle, IntPtr hTargetProcessHandle, ref IntPtr lpTargetHandle,
          int dwDesiredAccess, bool bInheritHandle, int dwOptions);
        #endregion

        private delegate int PTHREAD_START_ROUTINE(IntPtr lpParameter);
        private delegate void VoidDelegate();
        private static readonly PTHREAD_START_ROUTINE pThreadStartRoutine = new PTHREAD_START_ROUTINE(ThreadProc);
        private static readonly IntPtr pThreadProc = Marshal.GetFunctionPointerForDelegate<PTHREAD_START_ROUTINE>(pThreadStartRoutine);

        private static int ThreadProc(IntPtr httPointer)
        {
            HTT handleInfo = (HTT)Marshal.PtrToStructure(httPointer, typeof(HTT));
            uint returnLength = 4096;
            IntPtr objectInfoPointer;
            
            for (objectInfoPointer = Marshal.AllocHGlobal((int)returnLength); NtQueryObject(handleInfo.handle,
                OBJECT_INFORMATION_CLASS.ObjectNameInformation, objectInfoPointer, returnLength, ref returnLength) == -1073741820; objectInfoPointer = Marshal.AllocHGlobal((int)returnLength))
            {
                Marshal.FreeHGlobal(objectInfoPointer);
            }

            OBJECT_NAME_INFORMATION objectNameInfo = (OBJECT_NAME_INFORMATION)Marshal.PtrToStructure(objectInfoPointer, typeof(OBJECT_NAME_INFORMATION));
            Marshal.FreeHGlobal(objectInfoPointer);
            handleInfo.txt = objectNameInfo.Name.Buffer;
            Marshal.StructureToPtr<HTT>(handleInfo, httPointer, false);
            return 0;
        }

        public List<string> KillMutants()
        {
            var results = new List<string>();
            bool wow64Process = false;
            IsWow64Process(Process.GetCurrentProcess().Handle, out wow64Process);
            if (wow64Process)
            {
                IntPtr oldValue = IntPtr.Zero;
                Wow64DisableWow64FsRedirection(out oldValue);
            }
            
            EnablePrivilege("SeDebugPrivilege", true);

            foreach (var process in Process.GetProcesses())
            {                
                if (process.ProcessName.Contains("game.dll"))
                {
                    var handlesFromPid = CheckProcessForMutants(process.Id);
                    results.AddRange(handlesFromPid);
                }
                #region HardWay
                //string processName = (string)null;
                //IntPtr targetProcess = OpenProcess(4096, true, (uint)process.Id);
                //if (targetProcess != IntPtr.Zero)
                //{
                //    uint processNameBufferSize = 260;
                //    StringBuilder processNameDump = new StringBuilder((int)processNameBufferSize);
                //    QueryFullProcessImageName(targetProcess, 0, processNameDump, ref processNameBufferSize);
                //    processName = processNameDump.ToString();
                //    CloseHandle(targetProcess);
                //    if (processName.Contains("game.dll") || process.ProcessName.Contains("game"))
                //    {
                //        var handlesFromPid = GetHandlesFromPID(process.Id);
                //    }
                //}
                //else
                //{
                //    Marshal.GetLastWin32Error();
                //}
                #endregion
            }
            return results;
        }


        private List<string> CheckProcessForMutants(int targetPid)
        {
            var results = new List<string>();
            int systemInfoLength = 65536;
            int lengthOfInfoReturn = 0;
            IntPtr systemHandleInfoPointer;
            while (true)
            {
                systemHandleInfoPointer = Marshal.AllocHGlobal(systemInfoLength);
                switch (NtQuerySystemInformation(SYSTEM_INFORMATION_CLASS.SystemExtendedHandleInformation, systemHandleInfoPointer, systemInfoLength, ref lengthOfInfoReturn))
                {
                    case -1073741820:
                        Marshal.FreeHGlobal(systemHandleInfoPointer);
                        systemInfoLength += lengthOfInfoReturn;
                        continue;
                    case 0:
                        return DiveOnIn(targetPid, results, systemHandleInfoPointer);
                    default:
                        Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                        continue;
                }
            }
            //DiveOnIn:
            //return DiveOnIn(targetPid, results, systemHandleInfoPointer);
        }

        private static List<string> DiveOnIn(int targetPid, List<string> results, IntPtr systemHandleInfoPointer)
        {
            int targetHandleCount = Convert.ToInt32(Marshal.PtrToStructure(systemHandleInfoPointer, typeof(int)));
            IntPtr targetHandlePointer = systemHandleInfoPointer + Marshal.SizeOf(typeof(int)) * 2;
            SYSTEM_HANDLE_INFORMATION_EX extraInfo = (SYSTEM_HANDLE_INFORMATION_EX)Marshal.PtrToStructure(targetHandlePointer, typeof(SYSTEM_HANDLE_INFORMATION_EX));
            for (int index = 0; index <= targetHandleCount; ++index)
            {
                targetHandlePointer += Marshal.SizeOf(typeof(SYSTEM_HANDLE_INFORMATION_EX));
                SYSTEM_HANDLE_INFORMATION_EX targetHandleExInfo = (SYSTEM_HANDLE_INFORMATION_EX)Marshal.PtrToStructure(targetHandlePointer, typeof(SYSTEM_HANDLE_INFORMATION_EX));
                if (targetPid != -1 && (long)targetHandleExInfo.UniqueProcessId == (long)targetPid)
                {
                    string handleTypeName = "";
                    string handleValue = "";
                    IntPtr targetHandleCopy = IntPtr.Zero;
                    IntPtr handleProcessPointer = OpenProcess(64, false, targetHandleExInfo.UniqueProcessId);
                    if (handleProcessPointer != IntPtr.Zero)
                    {
                        DuplicateHandle(handleProcessPointer, (IntPtr)(long)targetHandleExInfo.HandleValue, GetCurrentProcess(), ref targetHandleCopy, 0, false, 2);
                        if (targetHandleCopy != IntPtr.Zero)
                        {
                            uint ReturnLength2 = 4096;
                            IntPtr num4;
                            for (num4 = Marshal.AllocHGlobal((int)ReturnLength2); NtQueryObject(targetHandleCopy, OBJECT_INFORMATION_CLASS.ObjectTypeInformation, num4,
                                ReturnLength2, ref ReturnLength2) == -1073741820; num4 = Marshal.AllocHGlobal((int)ReturnLength2))
                            {
                                Marshal.FreeHGlobal(num4);
                            }
                            handleTypeName = ((OBJECT_TYPE_INFORMATION)Marshal.PtrToStructure(num4, typeof(OBJECT_TYPE_INFORMATION))).TypeName.Buffer;
                            Marshal.FreeHGlobal(num4);
                            if (handleTypeName == "Directory" || handleTypeName == "File" || handleTypeName == "Section" || handleTypeName == "Key" ||
                                handleTypeName == "WindowStation" || handleTypeName == "Desktop" || handleTypeName == "Semaphore" || handleTypeName == "Mutant")
                            {
                                HTT structure3 = new HTT();
                                structure3.handle = targetHandleCopy;
                                IntPtr num5 = Marshal.AllocHGlobal(Marshal.SizeOf<HTT>(structure3));
                                Marshal.StructureToPtr<HTT>(structure3, num5, false);
                                IntPtr thread = CreateThread(IntPtr.Zero, 0U, pThreadProc, num5, 0, out int _);
                                if (WaitForSingleObject(thread, 500) == 258)
                                {
                                    TerminateThread(thread, 0);
                                    handleValue = "<Locked>";
                                }
                                else
                                {
                                    handleValue = ((HTT)Marshal.PtrToStructure(num5, typeof(HTT))).txt;
                                }
                                if (!string.IsNullOrEmpty(handleValue) && handleValue.Contains("BaseNamedObjects\\DAoCi"))
                                {
                                    IntPtr tempHand = IntPtr.Zero;
                                    DuplicateHandle(handleProcessPointer, (IntPtr)(long)targetHandleExInfo.HandleValue, GetCurrentProcess(), ref tempHand, 0, false, 1);
                                    CloseHandle(tempHand);
                                    results.Add($"Found:{handleTypeName} - {handleValue} - {(string.Format("0x{0:x2}", (object)targetHandleExInfo.HandleValue))}");
                                }
                                Marshal.FreeHGlobal(num5);
                            }
                            CloseHandle(targetHandleCopy);
                        }
                        CloseHandle(handleProcessPointer);
                    }
                }
            }
            Marshal.FreeHGlobal(systemHandleInfoPointer);
            return results;
        }

        private bool EnablePrivilege(string privilegeName, bool enablePriv)
        {
            bool flag = false;
            int ReturnLength = 0;
            IntPtr tokenHandlePointer = IntPtr.Zero;
            TOKEN_PRIVILEGES tokenPriv = new TOKEN_PRIVILEGES();
            tokenPriv.Privileges = new int[3];
            LUID Luid = new LUID();
            tokenPriv.PrivilegeCount = 1;
            tokenPriv.Privileges[2] = !enablePriv ? 0 : 2;
            if (LookupPrivilegeValue((string)null, privilegeName, out Luid))
            {
                using (Process currentProcess = Process.GetCurrentProcess())
                {
                    if (currentProcess.Handle != IntPtr.Zero && OpenProcessToken(currentProcess.Handle, 40U, out tokenHandlePointer))
                    {
                        tokenPriv.PrivilegeCount = 1;
                        tokenPriv.Privileges[2] = 2;
                        tokenPriv.Privileges[1] = Luid.HighPart;
                        tokenPriv.Privileges[0] = Luid.LowPart;
                        IntPtr tokenPointer = Marshal.AllocHGlobal(256);
                        Marshal.StructureToPtr<TOKEN_PRIVILEGES>(tokenPriv, tokenPointer, true);
                        if (AdjustTokenPrivileges(tokenHandlePointer, false, tokenPointer, 256, IntPtr.Zero, ref ReturnLength) && Marshal.GetLastWin32Error() == 0)
                            flag = true;
                        TOKEN_PRIVILEGES foo = (TOKEN_PRIVILEGES)Marshal.PtrToStructure(tokenPointer, typeof(TOKEN_PRIVILEGES));
                        Marshal.FreeHGlobal(tokenPointer);
                    }
                }
            }
            if (tokenHandlePointer != IntPtr.Zero)
            {
                CloseHandle(tokenHandlePointer);
            }

            return flag;
        }

    }
}
