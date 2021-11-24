using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace install_edge {
	internal static class Edge {

		/*
				$INSTALLED = Get-ItemProperty HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\* |  Select-Object DisplayName, DisplayVersion, Publisher, InstallDate
				$INSTALLED += Get-ItemProperty HKLM:\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\* | Select-Object DisplayName, DisplayVersion, Publisher, InstallDate
				$INSTALLED | ?{ $_.DisplayName -match 'edge' } | sort-object -Property DisplayName -Unique | Format-Table -AutoSize
		 */
		public static bool IsInstalled() {
			_items.Clear();
			GetAllAppEntries(Registry.LocalMachine, @"Software\Microsoft\Windows\CurrentVersion\Uninstall");
			GetAllAppEntries(Registry.LocalMachine, @"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");

			_items = _items
				.Where(i => i.Key.Contains("edge", StringComparison.InvariantCultureIgnoreCase))
				.ToDictionary(i => i.Key, i => i.Value);
			return 0 < _items.Count;
		}



		private static void GetAllAppEntries(RegistryKey hive, string registryPath) {
			using (RegistryKey uninstallKey = hive.OpenSubKey(registryPath, false)) {
				GetAllAppEntries(uninstallKey);
				uninstallKey.Close();
			}
		}

		private static void GetAllAppEntries(RegistryKey uninstallKey) {
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



		private static Dictionary<string, string> _items = new Dictionary<string, string>();
		public static Dictionary<string, string> Items => _items;
	}
}
