using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace install_edge {
	internal class Browser : IBrowser {

		public event EventHandler<BrowserMessageEventArgs> DisplayMessage;

		public virtual string Name { get; }


		protected Dictionary<string, string> _items = new Dictionary<string, string>();
		public virtual Dictionary<string, string> Items => _items;


		public virtual bool IsInstalled() {
			throw new NotImplementedException();
		}



		protected virtual void OnDisplayMesage(string message) {
			DisplayMessage?.Invoke(this, new BrowserMessageEventArgs { Message = message });
		}



		protected virtual void LoadAllInstalledSoftware() {
			/*
					$INSTALLED = Get-ItemProperty HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\* |  Select-Object DisplayName, DisplayVersion, Publisher, InstallDate
					$INSTALLED += Get-ItemProperty HKLM:\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\* | Select-Object DisplayName, DisplayVersion, Publisher, InstallDate
					$INSTALLED | ?{ $_.DisplayName -match 'edge' } | sort-object -Property DisplayName -Unique | Format-Table -AutoSize
			 */
			_items.Clear();
			GetAllAppEntries(Registry.LocalMachine, @"Software\Microsoft\Windows\CurrentVersion\Uninstall");
			GetAllAppEntries(Registry.LocalMachine, @"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
		}



		protected virtual void GetAllAppEntries(RegistryKey hive, string registryPath) {
			using (RegistryKey uninstallKey = hive.OpenSubKey(registryPath, false)) {
				GetAllAppEntries(uninstallKey);
				uninstallKey.Close();
			}
		}

		protected virtual void GetAllAppEntries(RegistryKey uninstallKey) {
			string[] subkeyNames = uninstallKey.GetSubKeyNames();        //	get all installed software
			foreach (string subkey in subkeyNames) {
				RegistryKey appKey = uninstallKey.OpenSubKey(subkey, false);
				string displayName = "";
				int index = 0;
				while (true) {
					displayName = appKey.GetValue("DisplayName")?.ToString() ?? Guid.NewGuid().ToString();
					if (0 < index) {
						displayName += $" ({index})";
					}
					if (!_items.ContainsKey(displayName)) {
						_items.Add(displayName, appKey.GetValue("DisplayVersion")?.ToString() ?? string.Empty);
						break;
					}
					index++;
				}

				appKey.Close();
			}
		}



		public virtual void Download() {
			if (string.IsNullOrWhiteSpace(_downloadUrl)) {
				throw new ArgumentNullException(nameof(DownloadUrl));
			}
			if (string.IsNullOrWhiteSpace(_downloadPath)) {
				throw new ArgumentNullException(nameof(DownloadDestinationPath));
			}

			if (!Directory.Exists(Path.GetDirectoryName(_downloadPath))) { Directory.CreateDirectory(Path.GetDirectoryName(_downloadPath)); }
			if (File.Exists(_downloadPath)) { File.Delete(_downloadPath); }
			double? lastProgress = 0.0;

			using (Downloader downloader = new Downloader(_downloadUrl, _downloadPath)) {
				downloader.ProgressChanged += (totalBytes, downloadedBytes, percentComplete) => {
					if (lastProgress <= percentComplete - 10.0 || percentComplete == 100.0) {
						OnDisplayMesage($"downloaded {downloadedBytes} ({percentComplete}%)");
						lastProgress = percentComplete;
					}
				};

				OnDisplayMesage("Beginning download...");
				Task task = downloader.StartDownload();
				task.Wait();
				_isDownloaded = true;
				OnDisplayMesage("Download complete.");
			}
		}


		
		

		public virtual void Install() {
			OnDisplayMesage("Beginning install...");
			Process installer = Process.Start(new ProcessStartInfo { FileName = _downloadPath, UseShellExecute = true });
			if (installer != null) {
				installer.WaitForExit();
			}
			OnDisplayMesage("done.");
		}



		public virtual void CleanUp() {
			OnDisplayMesage("Cleaning up...");
			if (File.Exists(_downloadPath)) {
				File.Delete(_downloadPath);
			}
			if (Directory.Exists(Path.GetDirectoryName(_downloadPath))) {
				Directory.Delete(Path.GetDirectoryName(_downloadPath), true);
			}
		}


		protected string _downloadUrl = string.Empty;
		public string DownloadUrl => _downloadUrl;


		protected string _downloadPath = string.Empty;
		public string DownloadDestinationPath => _downloadPath;


		protected bool _isDownloaded = false;
		public bool IsDownloaded => _isDownloaded;

	}
}
