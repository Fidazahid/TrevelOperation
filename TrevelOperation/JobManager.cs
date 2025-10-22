using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace TrevelOperation
{
    public static class JobManager
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr CreateJobObject(IntPtr lpJobAttributes, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AssignProcessToJobObject(IntPtr hJob, IntPtr hProcess);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetInformationJobObject(IntPtr hJob, JobObjectInfoType jobObjectInfoType, IntPtr lpJobObjectInfo, uint cbJobObjectInfoLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        public enum JobObjectInfoType
        {
            JobObjectExtendedLimitInformation = 9
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct JOBOBJECT_BASIC_LIMIT_INFORMATION
        {
            public Int64 PerProcessUserTimeLimit;
            public Int64 PerJobUserTimeLimit;
            public UInt32 LimitFlags;
            public UIntPtr MinimumWorkingSetSize;
            public UIntPtr MaximumWorkingSetSize;
            public UInt32 ActiveProcessLimit;
            public Int64 Affinity; 
            public UInt32 PriorityClass;
            public UInt32 SchedulingClass;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IO_COUNTERS
        {
            public UInt64 ReadOperationCount;
            public UInt64 WriteOperationCount;
            public UInt64 OtherOperationCount;
            public UInt64 ReadTransferCount;
            public UInt64 WriteTransferCount;
            public UInt64 OtherTransferCount;
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

        private const uint JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 0x00002000;

        private static IntPtr _jobHandle = IntPtr.Zero;

        public static bool CreateJob()
        {
            _jobHandle = CreateJobObject(IntPtr.Zero, null); 
            if (_jobHandle == IntPtr.Zero)
            {
                Console.WriteLine($"Failed to create job object. Error Code: {Marshal.GetLastWin32Error()}");
                return false;
            }

            var extendedInfo = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION
            {
                BasicLimitInformation = new JOBOBJECT_BASIC_LIMIT_INFORMATION
                {
                    LimitFlags = JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE
                }
            };

            int length = Marshal.SizeOf(typeof(JOBOBJECT_EXTENDED_LIMIT_INFORMATION));
            IntPtr extendedInfoPtr = Marshal.AllocHGlobal(length);
            Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);

            try
            {
                if (!SetInformationJobObject(_jobHandle, JobObjectInfoType.JobObjectExtendedLimitInformation, extendedInfoPtr, (uint)length))
                {
                    Console.WriteLine($"Failed to set information for job object. Error Code: {Marshal.GetLastWin32Error()}");
                    CloseHandle(_jobHandle); 
                    _jobHandle = IntPtr.Zero;
                    return false;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(extendedInfoPtr);
            }

            if (!AssignProcessToJobObject(_jobHandle, Process.GetCurrentProcess().Handle))
            {
                Console.WriteLine($"Warning: Failed to assign current process to the job. Error Code: {Marshal.GetLastWin32Error()}. Auto-cleanup of child processes might not occur if this app is killed.");
            }

            return true;
        }

        public static bool AssignProcess(Process process)
        {
            if (_jobHandle == IntPtr.Zero)
            {
                Console.WriteLine("Job object has not been created.");
                return false;
            }
            if (process == null || process.HasExited)
            {
                Console.WriteLine("Process is null or has already exited.");
                return false;
            }
            try
            {
                if (!AssignProcessToJobObject(_jobHandle, process.Handle))
                {
                    if (!process.HasExited)
                    {
                        Console.WriteLine($"Failed to assign process {process.Id} to job object. Error Code: {Marshal.GetLastWin32Error()}");
                    }
                    return false;
                }
            }
            catch (Win32Exception ex) 
            {
                Console.WriteLine($"Exception assigning process {process.Id} to job: {ex.Message}. Error Code: {ex.NativeErrorCode}");
                return false;
            }
            return true;
        }

        public static void CloseJob()
        {
            if (_jobHandle != IntPtr.Zero)
            {
                CloseHandle(_jobHandle);
                _jobHandle = IntPtr.Zero;
            }
        }
    }
}
