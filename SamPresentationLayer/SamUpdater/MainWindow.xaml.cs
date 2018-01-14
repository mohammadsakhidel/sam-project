using SamUxLib.Code.Utils;
using SamUxLib.Resources.Values;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SamUpdater
{
    public partial class MainWindow : Window
    {
        #region Ctors:
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Constants:
        const int CHECK_UPDATE_INTERVAL = 30000; //3 mins
        const int PROCESS_STOP_WAIT = 3000;
        #endregion

        #region Fields:
        System.Windows.Forms.NotifyIcon _notifyIcon = null;
        #endregion

        #region Event Handlers:
        private void Window_Initialized(object sender, EventArgs e)
        {
            try
            {
                var contextMenu = new System.Windows.Forms.ContextMenuStrip();
                contextMenu.Items.Add(Strings.Quit, null, NotifyIconQuit_Click);

                _notifyIcon = new System.Windows.Forms.NotifyIcon();
                _notifyIcon.MouseClick += NotifyIcon_MouseClick;
                _notifyIcon.Icon = SamUpdater.Resources.Resources.ic_updater;
                _notifyIcon.ContextMenuStrip = contextMenu;
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                #region Update Checker Task:
                Task.Run(() =>
                {
                    while (true)
                    {
                        CheckForUpdates();

                        Thread.Sleep(CHECK_UPDATE_INTERVAL);
                    }
                });
                #endregion

                #region init window:
                _notifyIcon.Visible = true;
                WindowState = WindowState.Minimized;
                var currentVersion = GetLocalUpdateCode();
                Log($"Current update code is: [{currentVersion}]");
                #endregion
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void MaximizeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                e.CanExecute = true;
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void MaximizeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ActivateWindow();
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                this.WindowState = WindowState.Minimized;
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    ActivateWindow();
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private void NotifyIconQuit_Click(object sender, EventArgs e)
        {
            try
            {
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        #endregion

        #region Methods:
        private void Log(string text)
        {
            Dispatcher.Invoke(() =>
            {
                txtLog.Text += $">> {text}{Environment.NewLine}";
                scrollViewer.ScrollToBottom();
            });
        }
        private void CheckForUpdates()
        {
            try
            {
                var updateBaseUrl = ConfigurationManager.AppSettings["updatesBaseUrl"];
                var updatesLocalFolderPath = $@"{AppDomain.CurrentDomain.BaseDirectory}updates\";

                var updateCodes = DownloadUpdateCodes(updateBaseUrl);
                var currentUpdateCode = GetLocalUpdateCode();
                var newUpdateCodes = updateCodes.Where(c => c > currentUpdateCode).ToList();

                #region download update packages to local folder:
                if (updateCodes.Any() && updateCodes.Last() > currentUpdateCode)
                {
                    DownloadAllNewUpdatePackages(newUpdateCodes, updateBaseUrl, updatesLocalFolderPath);
                }
                #endregion

                #region install update packages:
                foreach (var newUpdateCode in newUpdateCodes)
                {
                    InstallPackage(newUpdateCode, updatesLocalFolderPath);
                }
                #endregion
            }
            catch (Exception ex)
            {
                ExceptionManager.Handle(ex);
            }
        }
        private List<int> DownloadUpdateCodes(string updateBaseUrl)
        {
            using (WebClient client = new WebClient())
            {
                var infoFileUrl = new Uri(new Uri(updateBaseUrl), "info.txt");
                var updatesInfo = client.DownloadString(infoFileUrl);
                var updateCodes = updatesInfo.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(l => Regex.IsMatch(l, @"\d+"))
                    .Select(l => Convert.ToInt32(l))
                    .ToList();
                updateCodes.Sort();
                return updateCodes;
            }
        }
        private int GetLocalUpdateCode()
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings["localUpdateCode"]);
        }
        private void DownloadAllNewUpdatePackages(List<int> updateCodesToDownload, string updateBaseUrl, string updatesLocalFolderPath)
        {
            if (!Directory.Exists(updatesLocalFolderPath))
                Directory.CreateDirectory(updatesLocalFolderPath);

            using (WebClient client = new WebClient())
            {
                foreach (var code in updateCodesToDownload)
                {
                    var downloadUrl = new Uri(new Uri(updateBaseUrl), $"{code}.zip");
                    var localFilePath = $@"{updatesLocalFolderPath}\{code}.zip";
                    client.DownloadFile(downloadUrl, localFilePath);
                }
            }
        }
        private void InstallPackage(int packageCode, string localFolder)
        {
            #region Paths:
            var pkgFilePath = $@"{localFolder}{packageCode}.zip";
            var pkgFolderPath = $@"{localFolder}{packageCode}\";
            var pkgSamClientFolderPath = $@"{pkgFolderPath}SamClient\";
            var pkgSyncAgentFolder = $@"{pkgFolderPath}SamSyncAgent\";
            var pkgKioskFolder = $@"{pkgFolderPath}SamKiosk\";
            var pkgDesktopFolder = $@"{pkgFolderPath}SamDesktop\";
            var pkgScriptsFolder = $@"{pkgFolderPath}Scripts\";

            var lclSamClientFolderPath = ConfigurationManager.AppSettings["clientFolderPath"];
            var lclSyncAgentFolder = ConfigurationManager.AppSettings["syncAgentFolderPath"];
            var lclkioskFolder = ConfigurationManager.AppSettings["kioskFolderPath"];
            var lclDesktopFolder = ConfigurationManager.AppSettings["desktopFolderPath"];
            #endregion

            #region extract package:
            if (Directory.Exists(pkgFolderPath))
                Directory.Delete(pkgFolderPath, true);

            ZipFile.ExtractToDirectory(pkgFilePath, pkgFolderPath);
            #endregion

            #region stop needed processes:

            #region stop sam client process:
            var updateSamClient = !string.IsNullOrEmpty(lclSamClientFolderPath) && Directory.Exists(pkgSamClientFolderPath) &&
                (Directory.GetFiles(pkgSamClientFolderPath).Any() || Directory.GetDirectories(pkgSamClientFolderPath).Any());
            var samClientProcessStopped = false;
            if (updateSamClient)
                samClientProcessStopped = StopProcess("SamClient");
            #endregion

            #region stop sam sync agent process:
            var updateSyncAgent = !string.IsNullOrEmpty(lclSyncAgentFolder) && Directory.Exists(pkgSyncAgentFolder) &&
                (Directory.GetFiles(pkgSyncAgentFolder).Any() || Directory.GetDirectories(pkgSyncAgentFolder).Any());
            var syncAgentProcessStopped = false;
            if (updateSyncAgent)
            {
                var status = VersatileUtil.GetWindowsServiceStatus(SamUtils.Constants.WindowsServices.sync_service);
                if (status == ServiceControllerStatus.Running)
                {
                    VersatileUtil.StopService(SamUtils.Constants.WindowsServices.sync_service);
                    syncAgentProcessStopped = true;
                }
            }
            #endregion

            #region stop kiosk process:
            var updateKiosk = !string.IsNullOrEmpty(lclkioskFolder) && Directory.Exists(pkgKioskFolder) &&
                (Directory.GetFiles(pkgKioskFolder).Any() || Directory.GetDirectories(pkgKioskFolder).Any());
            var kioskProcessStopped = false;
            if (updateKiosk)
                kioskProcessStopped = StopProcess("SamKiosk");
            #endregion

            #region stop desktop process:
            var updateDesktop = !string.IsNullOrEmpty(lclDesktopFolder) && Directory.Exists(pkgDesktopFolder) &&
                (Directory.GetFiles(pkgDesktopFolder).Any() || Directory.GetDirectories(pkgDesktopFolder).Any());
            var desktopProcessStopped = false;
            if (updateDesktop)
                desktopProcessStopped = StopProcess("SamDesktop");
            #endregion

            #endregion

            using (var scope = new TransactionScope())
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SamClientConnection"].ConnectionString))
            using (var command = new SqlCommand() { Connection = connection })
            {
                #region Execute SQL Scripts on SamClientDb:
                var hasScripts = false;
                if (Directory.Exists(pkgScriptsFolder))
                {
                    var sqlFiles = Directory.GetFiles(pkgScriptsFolder, "*.sql").ToList();
                    if (sqlFiles.Any())
                    {
                        hasScripts = true;
                        sqlFiles.Sort();
                        var rowsAffected = 0;
                        connection.Open();
                        foreach (var sqlFile in sqlFiles)
                        {
                            var query = File.ReadAllText(sqlFile);
                            command.CommandText = (query);
                            rowsAffected += command.ExecuteNonQuery();
                        }

                        Log($"{sqlFiles.Count} SQL script file{(sqlFiles.Count > 1 ? "s" : "")} executed successfully. Affected rows: [{rowsAffected}].");
                    }
                }
                #endregion

                #region Update Files:
                #region update SamClient:
                if (updateSamClient)
                {
                    #region copy files:
                    Thread.Sleep(PROCESS_STOP_WAIT);
                    var copiedFilesCount = CopyAllItemsInDirectory(pkgSamClientFolderPath, lclSamClientFolderPath);
                    #endregion

                    Log($"Sam client application updated. {copiedFilesCount} files copied.");
                }
                #endregion

                #region update SamSyncAgent:
                if (updateSyncAgent)
                {
                    #region copy files:
                    Thread.Sleep(PROCESS_STOP_WAIT);
                    var copiedFilesCount = CopyAllItemsInDirectory(pkgSyncAgentFolder, lclSyncAgentFolder);
                    #endregion

                    Log($"Sync agent service updated. {copiedFilesCount} files copied.");
                }
                #endregion

                #region update SamKiosk:
                if (updateKiosk)
                {
                    #region copy files:
                    Thread.Sleep(PROCESS_STOP_WAIT);
                    var copiedFilesCount = CopyAllItemsInDirectory(pkgKioskFolder, lclkioskFolder);
                    #endregion

                    Log($"Kiosk application updated. {copiedFilesCount} files copied.");
                }
                #endregion

                #region update SamDesktop:
                if (updateDesktop)
                {
                    #region copy files:
                    Thread.Sleep(PROCESS_STOP_WAIT);
                    var copiedFilesCount = CopyAllItemsInDirectory(pkgDesktopFolder, lclDesktopFolder);
                    #endregion

                    Log($"Desktop application updated. {copiedFilesCount} files copied.");
                }
                #endregion
                #endregion

                #region Update Version Code:
                AddOrUpdateAppSetting("localUpdateCode", packageCode.ToString());
                #endregion

                if (hasScripts)
                    scope.Complete();
            }

            #region start stopped processes:
            if (samClientProcessStopped)
            {
                var processPath = $@"{lclSamClientFolderPath}\SamClient.exe";
                StartProcess(processPath, true);
            }

            if (syncAgentProcessStopped)
            {
                VersatileUtil.StartService(SamUtils.Constants.WindowsServices.sync_service);
            }

            if (kioskProcessStopped)
            {
                var processPath = $@"{lclkioskFolder}\SamKiosk.exe";
                StartProcess(processPath, false);
            }

            if (desktopProcessStopped)
            {
                var processPath = $@"{lclDesktopFolder}\SamDesktop.exe";
                StartProcess(processPath, false);
            }
            #endregion

            Log($"Update code [{packageCode}] successfully installed.");
        }
        private bool StopProcess(string name)
        {
            var processes = Process.GetProcessesByName(name);
            foreach (var p in processes)
            {
                p.Kill();
                p.WaitForExit();
            }

            return processes.Any();
        }
        private void StartProcess(string exeFilePath, bool asAdmin)
        {
            var startInfo = new ProcessStartInfo(exeFilePath);
            if (asAdmin)
                startInfo.Verb = "runas";
            var process = Process.Start(startInfo);
            var id = 1;
        }
        private int CopyAllItemsInDirectory(string sourceDir, string destDir)
        {
            var allDirectories = Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories);
            foreach (string dirPath in allDirectories)
            {
                var destDirPath = dirPath.Replace(sourceDir, destDir);
                if (!Directory.Exists(destDirPath))
                    Directory.CreateDirectory(destDirPath);
            }

            var allFiles = Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories);
            foreach (string srcFilePath in allFiles)
            {
                var destFilePath = srcFilePath.Replace(sourceDir, destDir);
                File.Copy(srcFilePath, destFilePath, true);
            }

            return allFiles.Count();
        }
        private void ActivateWindow()
        {
            WindowState = WindowState.Normal;
            Activate();
            Topmost = true;
            Topmost = false;
            Focus();
        }
        private void AddOrUpdateAppSetting(string key, string value)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            if (settings[key] == null)
            {
                settings.Add(key, value);
            }
            else
            {
                settings[key].Value = value;
            }
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
        #endregion
    }
}
