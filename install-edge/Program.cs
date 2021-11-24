using install_edge;
using System.Diagnostics;



int width = Console.WindowWidth;
double? lastProgress = 0.0;

string edgeUrl = @"https://msedge.sf.dl.delivery.mp.microsoft.com/filestreamingservice/files/a2662b5b-97d0-4312-8946-598355851b3b/MicrosoftEdgeEnterpriseX64.msi";
string downloadPath = Path.Combine(Path.GetTempPath(), "automated-edge-download", "MicrosoftEdgeEnterpriseX64.msi");
bool quiet = args.Contains("--quiet");
bool force = args.Contains("--force");
bool queryMode = args.Contains("--query");
bool showHelp = args.Contains("--help");

//	Check for help
if(showHelp) {
	PrintUsage();
	return;
}

//	Check for query mode: just display installed Edge versions, or report non-presence
if (queryMode) {
	if(!Edge.IsInstalled()) { Console.WriteLine("Edge is not installed."); }
	else {
		int maxKeyLength = Edge.Items.Select(i => i.Key.Length).Max();

		foreach (string key in Edge.Items.Keys) {
			PrintLine($"{key.PadRight(maxKeyLength)} : {Edge.Items[key]}");
		}
	}
	return;
}

//	Check if Edge is already installed.  If so, exit.
if (!force && Edge.IsInstalled()) {
	PrintLine("Edge web browser is already installed.");
	int maxKeyLength = Edge.Items.Select(i => i.Key.Length).Max();

	foreach (string key in Edge.Items.Keys) {
		PrintLine($"{key.PadRight(maxKeyLength)} : {Edge.Items[key]}");
	}
	return;
}

if (!Directory.Exists(Path.GetDirectoryName(downloadPath))) { Directory.CreateDirectory(Path.GetDirectoryName(downloadPath)); }
if (File.Exists(downloadPath)) { File.Delete(downloadPath); }

using (Downloader downloader = new Downloader(edgeUrl, downloadPath)) {
	downloader.ProgressChanged += (totalBytes, downloadedBytes, percentComplete) => {
		if (lastProgress <= percentComplete - 10.0 || percentComplete == 100.0) {
			PrintLine($"downloaded {downloadedBytes} ({percentComplete}%)");
			lastProgress = percentComplete;
		}
	};

	PrintLine("Beginning download...");
	await downloader.StartDownload();
	PrintLine("Download complete.  Beginning install...");

	Process edgeInstaller = Process.Start(new ProcessStartInfo { FileName = downloadPath, UseShellExecute = true });
	if (edgeInstaller != null) {
		edgeInstaller.Exited += (s, e) => {
			PrintLine("Cleaning up...");
			File.Delete(downloadPath);
			Directory.Delete(Path.GetDirectoryName(downloadPath), true);
		};

		edgeInstaller.WaitForExit();
	}
	PrintLine("done.");
}



void PrintUsage() {
	Console.WriteLine("install-edge.exe --help");
	Console.WriteLine("install-edge.exe --query");
	Console.WriteLine("install-edge.exe [--force] [--quiet]");
	Console.WriteLine();
	Console.WriteLine("  --query    Displays the currently installed Edge version");
	Console.WriteLine("  --help     This screen");
	Console.WriteLine("  --force    Skip the 'is Edge already installed' check");
	Console.WriteLine("  --quiet    Do not display any output to the console");
	Console.WriteLine();
}



void PrintLine(string message) {
	if (!quiet) {
		Console.WriteLine(message);
	}
}