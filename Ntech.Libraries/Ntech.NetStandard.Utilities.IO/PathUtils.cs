using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Ntech.NetStandard.Utilities.IO
{
    /// <summary>
    /// NADAE.Utilities to handle path manipulation
    /// </summary>
    public static class PathUtils
    {
        #region Constant

        /// <summary>
        /// The nadae ServiceSimulator folder 
        /// </summary>
        private const string ServiceSimulatorTempFolderName = "ServiceSimulator";

        /// <summary>
        /// The right slash
        /// </summary>
        public const string RightSlash = @"\";

        #endregion

        #region Variable

        /// <summary>
        /// The ntech temporary folder name
        /// </summary>
        private static string NADAETempFolderName = "Ntech";

        /// <summary>
        /// The plugin temporary folder name
        /// </summary>
        private static string PluginTempFolderName = "Plugin";

        /// <summary>
        /// The plugin information temporary folder
        /// </summary>
        private static string PluginInfoTempFolderName = "PluginInfoTemp";

        /// <summary>
        /// The plugin information temporary file name
        /// </summary>
        private static string PluginInfoTempFileName = "PluginInfoTemp.xml";

        #endregion

        #region Public Methods

        /// <summary>
        /// Get directory path of lib
        /// </summary>
        /// <param name="installationTempFolder">The installation temp folder path</param>
        /// <param name="newFilesPath">The new files path.</param>
        /// <returns>
        /// The directory path
        /// </returns>
        /// <exception cref="System.Exception">
        /// </exception>
        public static string GetDirectoryPath(string installationTempFolder, string newFilesPath)
        {
            var directoryPath = string.Empty;

            // Check directory path
            if (Path.HasExtension(installationTempFolder))
            {
                throw new Exception($"{installationTempFolder} is not a directory path");
            }

            // Check file path
            if (!Path.HasExtension(newFilesPath))
            {
                throw new Exception($"{newFilesPath} is not a file path");
            }

            var directoryContainFile = newFilesPath.Split('\\');
            var tempDirectory = installationTempFolder.Split('\\');

            for (int i = 0; i < directoryContainFile.Length; i++)
            {
                if (directoryContainFile[i].Equals(tempDirectory[tempDirectory.Length - 1]))
                {
                    for (int j = i + 1; j < directoryContainFile.Length - 1; j++)
                    {
                        directoryPath = Path.Combine(directoryPath, directoryContainFile[j]);
                    }
                    break;
                }
            }

            return directoryPath;
        }

        /// <summary>
        /// Get plugin information temporary folder path
        /// </summary>
        /// <returns>The plugin information temporary folder path</returns>
        public static string GetPluginInfoTempFolderPath()
        {
            var tmpFolder = GetNADAETempFolder();
            // Check folder whether or not exist
            if (Directory.Exists(Path.Combine(tmpFolder, PluginTempFolderName, PluginInfoTempFolderName)))
            {
                Directory.CreateDirectory(Path.Combine(tmpFolder, PluginTempFolderName, PluginInfoTempFolderName));
            }
            return Path.Combine(Path.Combine(tmpFolder, PluginTempFolderName, PluginInfoTempFolderName));
        }

        /// <summary>
        /// Get plugin information temporary folder name
        /// </summary>
        /// <returns>the plugin information temporary folder name</returns>
        public static string GetPluginInfoTempFileName()
        {
            return PluginInfoTempFileName;
        }

        /// <summary>
        /// Sets the nadae temporary folder.
        /// </summary>
        /// <param name="markingCode">The marking code.</param>
        public static void SetNADAETempFolder(string markingCode)
        {
            if (string.IsNullOrWhiteSpace(markingCode))
            {
                return;
            }

            NADAETempFolderName = $"NADAE{markingCode}";
        }

        /// <summary>
        /// Get absolute path by passing a relative path to application folder 
        /// </summary>
        /// <param name="relativePath">The relative path to application folder</param>
        /// <returns>The absolute path</returns>
        public static string GetExecutingPath(string relativePath)
        {
            var asmPath = Assembly.GetEntryAssembly().Location;
            var path = Directory.GetParent(asmPath).FullName;
            var absolutePath = Path.Combine(path, relativePath);
            return absolutePath;
        }

        /// <summary>
        /// Get absolute path by passing a relative path to application folder 
        /// </summary>
        /// <returns>The absolute path</returns>
        public static string GetExecutingPath()
        {
            var asmPath = Assembly.GetEntryAssembly().Location;
            return Directory.GetParent(asmPath).FullName;
        }

        /// <summary>
        /// Get absolute path by passing a relative path to application folder
        /// </summary>
        /// <param name="relativePath">The relative path to application folder</param>
        /// <returns>The absolute path</returns>
        public static string GetAppPath(string relativePath)
        {
            return GetRelativeAppDomainBasePath(relativePath);
        }

        /// <summary>
        /// Get absolute path by passing a relative path to app domain base folder
        /// </summary>
        /// <param name="relativePath">The relative path to app domain base folder</param>
        /// <returns>The absolute path</returns>
        /// <exception cref="ArgumentNullException">
        /// Input relativePath is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// relativePath contains special character(s)
        /// </exception>
        public static string GetRelativeAppDomainBasePath(string relativePath)
        {
            var basedir = AppDomain.CurrentDomain.BaseDirectory;
            var absolutePath = Path.Combine(basedir, relativePath);
            return absolutePath;
        }

        /// <summary>
        /// Ensure create folder
        /// </summary>
        /// <param name="path">The path to create</param>
        public static void EnsureCreateFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            while (!Directory.Exists(path))
            {
                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// The get file name from path.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The System.String.
        /// </returns>
        public static string GetFileNameFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            var lastDirectorySeparatorIndex = Math.Max(path.LastIndexOf('\\'), path.LastIndexOf('/'));
            var startIndexOfFileName = lastDirectorySeparatorIndex > -1 ? lastDirectorySeparatorIndex + 1 : 0;
            return path.Substring(startIndexOfFileName);
        }

        ///// <summary>
        ///// Creates the unc path.
        ///// </summary>
        ///// <param name="ipAddress">The ip address.</param>
        ///// <param name="absolutePath">The absolute path.</param>
        ///// <returns></returns>
        //public static string CreateUNCPath(string ipAddress, string absolutePath)
        //{
        //    NetworkHelper.CheckValidIpAddress(ipAddress);
        //    if (NetworkHelper.IsLocalHostName(ipAddress))
        //    {
        //        return absolutePath; // No need to create UNC for local path
        //    }

        //    // Build UNC path
        //    absolutePath = absolutePath.Replace(":", @"$");
        //    return $@"\\{ipAddress}\{absolutePath}";
        //}

        /// <summary>
        /// Get List Directories by Path,searchPattern and SearchOption
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <param name="searchOption"></param>
        /// <returns>List directories</returns>
        public static List<string> GetDirectories(string path, string searchPattern = "*",
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (searchOption == SearchOption.TopDirectoryOnly)
            {
                return Directory.GetDirectories(path, searchPattern).ToList();
            }

            var directories = new List<string>(GetDirectories(path, searchPattern));
            for (var i = 0; i < directories.Count; i++)
            {
                directories.AddRange(GetDirectories(directories[i], searchPattern));
            }

            return directories;
        }

        /// <summary>
        /// Gets the temporary folder.
        /// </summary>
        /// <returns>The NADAE temporary folder. Ex: Use\AppData\NADAE\GUID</returns>
        public static string GetNADAETempFolder()
        {
            var subFolder = Guid.NewGuid().ToString();
            return Path.Combine(Path.GetTempPath(), NADAETempFolderName, subFolder);
        }

        /// <summary>
        /// Gets the temporary folder for service simulator.
        /// </summary>
        /// <returns>The NADAE temporary folder. Ex: Use\AppData\NADAE\GUID</returns>
        public static string GetNADAETempFolderForSimulator()
        {
            var subFolder = Guid.NewGuid().ToString();
            return Path.Combine(Path.GetTempPath(), NADAETempFolderName, ServiceSimulatorTempFolderName, subFolder);
        }

        /// <summary>
        /// Get Plugin Temp Folder for extract ZipFile
        /// </summary>
        /// <returns>The Plugin temporary folder. Ex: Use\AppData\Plugin\GUID</returns>
        public static string GetPluginTempFolder()
        {
            var subFolder = Guid.NewGuid().ToString();
            return Path.Combine(Path.GetTempPath(), NADAETempFolderName, PluginTempFolderName, subFolder);
        }
        /// <summary>
        /// Gets the Root temporary folder
        /// </summary>
        /// <returns>The NADAE temporary folder. Ex: Use\AppData\NADAE\GUID</returns>
        public static string GetRootNADAETempFolder()
        {
            return Path.Combine(Path.GetTempPath(), NADAETempFolderName, ServiceSimulatorTempFolderName);
        }

        /// <summary>
        /// Gets common app data NADAE folder by a station
        /// </summary>
        /// <param name="stationName">Name of the station.</param>
        /// <returns>The app data NADAE folder. Ex: C:\ProgramData\NADAE\SimSimulator</returns>
        public static string GetCommonAppDataNADAEFolder(string stationName)
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            appDataFolder = Path.Combine(appDataFolder, NADAETempFolderName, ServiceSimulatorTempFolderName);

            // Append with station name
            if (!string.IsNullOrWhiteSpace(stationName))
            {
                appDataFolder = Path.Combine(appDataFolder, stationName);
            }

            if (!File.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            return appDataFolder;
        }

        /// <summary>
        /// Gets common app data NADAE folder.
        /// </summary>
        /// <returns>The common app data NADAE folder</returns>
        public static string GetCommonAppDataNADAEFolder()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            appDataFolder = Path.Combine(appDataFolder, NADAETempFolderName);
            return appDataFolder;
        }
        /// <summary>
        /// Get temp path
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetTempPath(string fileName)
        {
            var realPath = Path.Combine(Path.GetTempPath(), fileName);
            EnsureCreateFolder(Path.GetDirectoryName(realPath));
            return realPath;
        }

        /// <summary>
        /// Get the PlugAndPlay xml file for perform install/upgrade/uninstall plugin/SharedLibDeployment
        /// </summary>
        /// <returns>Full path of temp file. Ex:C:\user\NADAE\PlugAndPlay\PlugAndPlay.xml </returns>
        public static string GetExtractTempXmlFile()
        {
            var path = Path.Combine(Path.GetTempPath(), NADAETempFolderName, "PlugAndPlay", "PlugAndPlay.xml");
            return path;
        }

        /// <summary>
        /// Get the Plugin folder for perform install/upgrade/uninstall plugin/SharedLibDeployment
        /// </summary>
        /// <returns>Full path of folder. Ex:C:\user\NADAE\Plugin </returns>
        public static string GetPluginFolder()
        {
            var path = Path.Combine(Path.GetTempPath(), NADAETempFolderName, PluginTempFolderName);
            return path;
        }

        /// <summary>
        /// Gets all file in dirctory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public static List<string> GetAllFileInDirectory(string path)
        {
            var listFiles = new List<string>();

            var directoryInfor = new DirectoryInfo(path);

            if (!directoryInfor.Exists)
            {
                return listFiles;
            }

            var listDirectory = directoryInfor.EnumerateDirectories().ToList();

            if (!listDirectory.IsNullOrEmpty())
            {
                foreach (var directory in listDirectory)
                {
                    listFiles.AddRange(GetAllFileInDirectory(directory.FullName));
                }
            }

            var listFileInDirectory = Directory.EnumerateFiles(path);
            listFiles.AddRange(listFileInDirectory);
            return listFiles;
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <param name="baseDirectory">The base directory.</param>
        /// <param name="input">The sub component.</param>
        /// <returns></returns>
        public static string GetPath(string baseDirectory, string input)
        {
            var subComponent = input;

            if (subComponent.StartsWith(RightSlash))
            {
                subComponent = subComponent.Substring(1);
            }

            var path = Path.Combine(baseDirectory, subComponent);
            return path;
        }

        #endregion
    }
}
