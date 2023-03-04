using System.IO;
using System.Linq;
using System.Security.AccessControl;
using Common.Logging;

namespace Ntech.NetStandard.Utilities.IO
{
    /// <summary>
    /// File permission checker.
    /// </summary>
    public static class FilePermissionChecker
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(FilePermissionChecker));

        /// <summary>
        /// Check write permission.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <returns>True: If we have write permission. Otherwise, return false.</returns>
        public static bool HasWritePermission(string path, string fullPath)
        {
            Logger.Info($"Start HasWritePermission: {path}");
            Preconditions.CheckNotNull(path, "path");
            Preconditions.CheckNotBlank(path, "path");
            var hasPermissionToWrite = false;
            var result = FileUtils.SafeAccess(
                () =>
                {
                    // check file or folder exist
                    FileAttributes location = File.GetAttributes(path);
                    if (location.HasFlag(FileAttributes.Directory))
                    {
                        Preconditions.CheckFilePathCorrect(path, fullPath);
                    }
                    else
                    {
                        Preconditions.CheckFileExist(path, fullPath);
                    }
                    // Get file security information.
                    var fileInfo = new FileInfo(path);
                    var accessControlList = fileInfo.GetAccessControl();
                    var accessRules = accessControlList.GetAccessRules(true, true,
                        typeof(System.Security.Principal.SecurityIdentifier));

                    foreach (var rule in accessRules.Cast<FileSystemAccessRule>().Where(
                        rule => (FileSystemRights.Write & rule.FileSystemRights) == FileSystemRights.Write))
                    {
                        // Check if write permission is deny.
                        if (rule.AccessControlType == AccessControlType.Deny || fileInfo.IsReadOnly == true)
                        {
                            hasPermissionToWrite = false;
                            break;
                        }

                        // Check if write permission is allowed.
                        if (rule.AccessControlType == AccessControlType.Allow && fileInfo.IsReadOnly == false)
                        {
                            hasPermissionToWrite = true;
                        }
                    }
                    Logger.Info($"Has Write Permission: {hasPermissionToWrite}");
                    Logger.Info($"End HasReadPermission: {path}");
                    return hasPermissionToWrite;
                }, fullPath);
            return result;
        }

        /// <summary>
        /// Check read permission.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <returns>True: If we have read permission. Othewise, return false.</returns>
        public static bool HasReadPermission(string path, string fullPath)
        {
            Logger.Info($"Start HasReadPermission: {path}");
            Preconditions.CheckNotNull(path, "path");
            Preconditions.CheckNotBlank(path, "path");
            var hasPermissionToRead = false;
            var result = FileUtils.SafeAccess(
                () =>
                {
                    // check file or folder exist
                    FileAttributes location = File.GetAttributes(path);
                    if (location.HasFlag(FileAttributes.Directory))
                    {
                        Preconditions.CheckFilePathCorrect(path, fullPath);
                    }
                    else
                    {
                        Preconditions.CheckFileExist(path, fullPath);
                    }
                    // Get file security information.
                    var fileInfo = new FileInfo(path);
                    var accessControlList = fileInfo.GetAccessControl();
                    var accessRules = accessControlList.GetAccessRules(true, true,
                        typeof(System.Security.Principal.SecurityIdentifier));
                    foreach (var rule in accessRules.Cast<FileSystemAccessRule>().Where(
                        rule => (FileSystemRights.Read & rule.FileSystemRights) == FileSystemRights.Read))
                    {
                        // Check if read permission is Deny.
                        if (rule.AccessControlType == AccessControlType.Deny)
                        {
                            hasPermissionToRead = false;
                            break;
                        }
                        // Check if read permission is allowed.
                        if (rule.AccessControlType == AccessControlType.Allow)
                        {
                            hasPermissionToRead = true;
                        }
                    }
                    Logger.Info($"Has Read Permission: {hasPermissionToRead}");
                    Logger.Info($"End HasReadPermission: {path}");
                    return hasPermissionToRead;
                }, fullPath);
            return result;
        }
    }
}
