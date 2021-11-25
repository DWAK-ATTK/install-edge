using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace install_edge {
	internal class Edge : Browser {

		public Edge() {
			_downloadUrl = @"https://msedge.sf.dl.delivery.mp.microsoft.com/filestreamingservice/files/a2662b5b-97d0-4312-8946-598355851b3b/MicrosoftEdgeEnterpriseX64.msi";
			_downloadPath = Path.Combine(Path.GetTempPath(), "automated-browser-download", "edge", "MicrosoftEdgeEnterpriseX64.msi");
		}

		public override string Name => "Edge";


		public override bool IsInstalled() {
			LoadAllInstalledSoftware();

			_items = _items
				.Where(i => i.Key.Contains("edge", StringComparison.InvariantCultureIgnoreCase))
				.ToDictionary(i => i.Key, i => i.Value);
			return 0 < _items.Count;
		}
	}



	internal class Chrome : Browser {

		public Chrome() {
			_downloadUrl = @"http://dl.google.com/chrome/install/375.126/chrome_installer.exe";
			_downloadPath = Path.Combine(Path.GetTempPath(), "automated-browser-download", "chrome", "ChromeInstaller.exe");
		}

		public override string Name => "Chrome";


		public override bool IsInstalled() {
			LoadAllInstalledSoftware();

			_items = _items
				.Where(i => i.Key.Contains("chrome", StringComparison.InvariantCultureIgnoreCase))
				.ToDictionary(i => i.Key, i => i.Value);
			return 0 < _items.Count;
		}
	}



	internal class FireFox : Browser {

		public FireFox() {
			_downloadUrl = @"https://download.mozilla.org/?product=firefox-latest&os=win64&lang=en-US";
			_downloadPath = Path.Combine(Path.GetTempPath(), "automated-browser-download", "firefox", "firefox-installer.exe");
		}
		public override string Name => "FireFox";


		public override bool IsInstalled() {
			LoadAllInstalledSoftware();

			_items = _items
				.Where(i => i.Key.Contains("firefox", StringComparison.InvariantCultureIgnoreCase))
				.ToDictionary(i => i.Key, i => i.Value);
			return 0 < _items.Count;
		}
	}
}
