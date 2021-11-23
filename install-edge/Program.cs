

using System.Diagnostics;

int width = Console.WindowWidth;
double? lastProgress = 0.0;

string edgeUrl = @"https://msedge.sf.dl.delivery.mp.microsoft.com/filestreamingservice/files/a2662b5b-97d0-4312-8946-598355851b3b/MicrosoftEdgeEnterpriseX64.msi";
string downloadPath = Path.Combine(Path.GetTempPath(), "automated-edge-download", "MicrosoftEdgeEnterpriseX64.msi");

if (!Directory.Exists(Path.GetDirectoryName(downloadPath))) { Directory.CreateDirectory(Path.GetDirectoryName(downloadPath)); }
if (File.Exists(downloadPath)) { File.Delete(downloadPath); }

using (Downloader downloader = new Downloader(edgeUrl, downloadPath)) {
	downloader.ProgressChanged += (totalBytes, downloadedBytes, percentComplete) => {
		if (lastProgress <= percentComplete - 10.0) {
			Console.WriteLine($"downloaded {downloadedBytes} ({percentComplete}%)");
			lastProgress = percentComplete;
		}
	};

	Console.WriteLine("Beginning download...");
	await downloader.StartDownload();
	Console.WriteLine("Download complete.  Beginning install...");

	Process edgeInstaller = Process.Start(new ProcessStartInfo { FileName = downloadPath, UseShellExecute = true });
	if (edgeInstaller != null) {
		edgeInstaller.Exited += (s, e) => {
			Console.WriteLine("Cleaning up...");
			File.Delete(downloadPath);
			Directory.Delete(Path.GetDirectoryName(downloadPath), true);
		};

		edgeInstaller.WaitForExit();
	}
	Console.WriteLine("done.");
}