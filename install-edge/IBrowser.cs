using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace install_edge {
	internal interface IBrowser {

		event EventHandler<BrowserMessageEventArgs> DisplayMessage;

		string Name { get; }

		Dictionary<string, string> Items { get; }

		string DownloadUrl { get; }

		string DownloadDestinationPath { get; }

		bool IsInstalled();

		void Download();

		bool IsDownloaded { get; }

		void Install();

		void CleanUp();
	}
}
