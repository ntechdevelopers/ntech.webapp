using Common.Logging;
using System;
using System.IO;
using System.Linq;

namespace Ntech.NetStandard.Utilities.IO
{
    public static class FileUtils
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(FileUtils));

        /// <summary>
        /// Execute read & write IO files
        /// </summary>
        /// <param name="action"></param>
        /// <param name="filePath"></param>
        public static void SafeAccess(Action action, string filePath)
        {
            try
            {
                action.Invoke();
            }
            catch (UnauthorizedAccessException ex)
            {
                var message = $"Access to the path '{filePath}' is denied";
                Logger.Error(message, ex);
                new Exception(message);
            }
            catch (PathTooLongException ex)
            {
                var message = $"Path or file [{filePath}] is longer than supported";
                Logger.Error(message, ex);
                new Exception(message);
            }
            catch (DirectoryNotFoundException ex)
            {
                var message = $"Part of a file or directory [{filePath}] cannot be found";
                Logger.Error(message, ex);
                new Exception(message);
            }
            catch (FileNotFoundException)
            {
                var message = $"Path or file [{filePath}] does not exist";
                Logger.Error(message);
                new Exception(message);
            }
            catch (IOException ex)
            {
                var message = $"General IO exception [{ex.Message}]";
                Logger.Error(message, ex);
                throw new Exception(message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Copies the directory.
        /// </summary>
        /// <param name="sourceDirPath">The source path.</param>
        /// <param name="destinationDirPath">The destination path.</param>
        /// <param name="copySubDirs">if set to <c>true</c> [copy sub dirs].</param>
        /// <param name="copyCurrentDir">if set to <c>true</c> [copy current dir].</param>
        /// <exception cref="System.IO.DirectoryNotFoundException">Source directory does not exist or could not be found: " + sourcePath</exception>
        public static void CopyDirectory(string sourceDirPath, string destinationDirPath, bool copySubDirs = true,
                                                                                          bool copyCurrentDir = false,
                                                                                          bool overwritte = false)
        {
            Logger.Debug($"CopyDirectory - sourcePath={sourceDirPath}, destinationPath={destinationDirPath}, copySubDirs={copySubDirs}, copyCurrentDir={copyCurrentDir}, overwrite ={overwritte}");

            // Get the subdirectories for the specified directory.
            var sourceDirInfo = new DirectoryInfo(sourceDirPath);
            if (!sourceDirInfo.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirPath);
            }

            if (copyCurrentDir) // Also copy current source directory
            {
                var sourceDirName = sourceDirInfo.Name;
                destinationDirPath = Path.Combine(destinationDirPath, sourceDirName);
            }

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destinationDirPath))
            {
                Directory.CreateDirectory(destinationDirPath);
            }

            // Get the files in the directory and copy them to the new location.
            Logger.Debug("CopyDirectory - Copy files...");
            var files = sourceDirInfo.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destinationDirPath, file.Name);
                file.CopyTo(temppath, overwritte);
            }
            Logger.Debug("CopyDirectory - Copy files...DONE");

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                Logger.Debug("CopyDirectory - Copy Sub-Dirs...");
                var sourceSubDirs = sourceDirInfo.GetDirectories();
                foreach (DirectoryInfo subdir in sourceSubDirs)
                {
                    string tempPath = Path.Combine(destinationDirPath, subdir.Name);
                    CopyDirectory(subdir.FullName, tempPath, copySubDirs, false, true);
                }
                Logger.Debug("CopyDirectory - Copy Sub-Dirs...DONE");
            }

            Logger.Debug("CopyDirectory...DONE");
        }

        /// <summary>
        /// Prepares the empty folder.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        public static void PrepareEmptyFolder(string folderName)
        {
            // 3. Clear backup folder
            if (Directory.Exists(folderName))
            {
                FileUtils.ClearFolder(folderName, false);
            }
            else
            {
                PathUtils.EnsureCreateFolder(folderName);
            }
        }

        /// <summary>
        /// Determines whether [is file path] [the specified file path].
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns><c>true</c> if [is file path] [the specified file path]; otherwise, <c>false</c>.</returns>
        public static bool IsFilePath(string filePath)
        {
            var isFilePath = true;

            // Get the file attributes for file or directory
            var attr = File.GetAttributes(filePath);

            // Detect whether its a directory or file
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                isFilePath = false;
            }

            return isFilePath;
        }

        /// <summary>
        /// Checks the ability to create and write to a file in the supplied directory.
        /// </summary>
        /// <param name="directory">String representing the directory path to check.</param>
        /// <returns>True if successful; otherwise false.</returns>
        public static bool CheckDirectoryAccess(string directory)
        {
            var success = false;
            var fullPath = Path.Combine(directory, "file.lock");
            try
            {
                using (var fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write))
                {
                    fs.WriteByte(0xff);
                }

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Clear folder
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="deleteParent">if set to <c>true</c> [delete parent].</param>
        public static void ClearFolder(string folderName, bool deleteParent = true)
        {
            SafeAccess(() =>
            {
                Logger.DebugFormat("Enter method clean folder. Parameter : folderName = [{0}], Delete parent = [{0}]", folderName, deleteParent);
                if (!Directory.Exists(folderName))
                {
                    return;
                }

                // Delete all file contain in folder
                var dir = new DirectoryInfo(folderName);
                foreach (var file in dir.GetFiles())
                {
                    File.SetAttributes(file.FullName, FileAttributes.Normal);
                    file.Delete();
                }

                // Delete all sub folders in root folder
                foreach (var di in dir.GetDirectories())
                {
                    Logger.DebugFormat("Delete sub directory : {0}", dir.FullName);
                    ClearFolder(di.FullName);
                }

                if (deleteParent)
                {
                    Logger.DebugFormat("Delete root directory : {0}", dir.FullName);
                    if (dir.GetFiles().Any() || dir.GetDirectories().Any())
                    // back up delete incase can not delete all file or folder in root folder
                    {
                        dir.Delete(true);
                    }
                    else
                    {
                        dir.Delete();
                    }
                }

                Logger.DebugFormat("Leave method clean folder");
            }, folderName);
        }


        /// <summary>
        /// Execute read & write IO files
        /// </summary>
        /// <param name="action"></param>
        /// <param name="filePath"></param>
        public static T SafeAccess<T>(Func<T> action, string filePath)
        {
            T value;
            try
            {
                value = action.Invoke();
            }
            catch (UnauthorizedAccessException ex)
            {
                var message = $"Access to the path '{filePath}' is denied";
                Logger.Error(message, ex);
                throw new Exception(message, ex);
            }
            catch (PathTooLongException ex)
            {
                var message = $"Path or file [{filePath}] is longer than supported";
                Logger.Error(message, ex);
                throw new Exception(message, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                var message = $"Part of a file or directory [{filePath}] cannot be found";
                Logger.Error(message, ex);
                throw new Exception(message, ex);
            }
            catch (FileNotFoundException ex)
            {
                var message = $"Path or file [{filePath}] does not exist";
                Logger.Error(message);
                throw new Exception(message, ex);
            }
            catch (IOException ex)
            {
                var message = $"General IO exception [{ex.Message}]";
                Logger.Error(message, ex);
                throw new Exception(message, ex);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw new Exception(ex.Message, ex);
            }

            return value;
        }

        /// <summary>
        /// Deletes the path.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void DeletePath(string path)
        {
            SafeAccess(() =>
            {
                if (IsFilePath(path))
                {
                    DeleteFile(path);
                }
                else
                {
                    ClearFolder(path, true);
                }
            }, path);
        }

        /// <summary>
        /// Move file from source to destination
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="isOverride"></param>
        public static void MoveFile(string source, string dest, bool isOverride = true)
        {
            SafeAccess(() =>
            {
                // Delete Xml backup file
                if (File.Exists(source))
                {
                    if (isOverride && File.Exists(dest))
                    {
                        File.Delete(dest);
                    }

                    File.Move(source, dest);
                }
            }, source);
        }

        /// <summary>
        /// Move file from source to destination
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="isOverride"></param>
        public static void MoveDir(string source, string dest, bool isOverride = true)
        {
            SafeAccess(() =>
            {
                // Delete Xml backup file
                if (Directory.Exists(source))
                {
                    if (isOverride && Directory.Exists(dest))
                    {
                        Directory.Delete(dest, true);
                    }

                    PathUtils.EnsureCreateFolder(Path.GetDirectoryName(dest));
                    Directory.Move(source, dest);
                }
            }, source);
        }

        /// <summary>
        /// Create new hidden folder with specified path
        /// </summary>
        /// <param name="folderPath">Path of folder</param>
        public static void CreateHiddenFolder(string folderPath)
        {
            SafeAccess(() =>
            {
                // New folder if it's not exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                var dir = new DirectoryInfo(folderPath);
                dir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }, folderPath);
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public static void DeleteFile(string filePath)
        {
            Logger.Debug($"[DeleteFile] - filePath=[{filePath}]...");
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"DeleteFile - Failed to delete file [{filePath}].", ex);
                return;
            }
            Logger.Debug($"[DeleteFile] - filePath=[{filePath}]...DONE");
        }

        /// <summary>
        /// Copies the file.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="dest">The dest.</param>
        /// <param name="isOverride">if set to <c>true</c> [is override].</param>
        public static void CopyFile(string source, string dest, bool isOverride = true)
        {
            SafeAccess(() =>
            {
                // Delete Xml backup file
                if (File.Exists(source))
                {
                    if (isOverride && File.Exists(dest))
                    {
                        File.Delete(dest);
                    }

                    File.Copy(source, dest);
                }
            }, source);
        }
    }
}
