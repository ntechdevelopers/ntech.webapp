using System;
using System.Runtime.InteropServices;

namespace Ntech.NetStandard.Utilities.IO
{
    /// <summary>
    /// The IoHelper class
    /// </summary>
    public static class IoHelper
    {
        /// <summary>
        /// Check if disk full or not from exception.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool IsDiskFull(Exception ex)
        {
            const int ERROR_HANDLE_DISK_FULL = 0x27;
            const int ERROR_DISK_FULL = 0x70;

            int win32ErrorCode = Marshal.GetHRForException(ex) & 0xFFFF;
            return win32ErrorCode == ERROR_HANDLE_DISK_FULL || win32ErrorCode == ERROR_DISK_FULL;
        }
    }
}
