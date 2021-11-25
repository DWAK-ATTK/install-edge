using install_edge;
using System.Diagnostics;



int width = Console.WindowWidth;

bool quiet = args.Contains("--quiet");
bool force = args.Contains("--force");
bool queryMode = args.Contains("--query");
bool downloadOnly = args.Contains("--download-only");
bool showHelp = args.Contains("--help");

string[] browsers = { "edge" };
Dictionary<string, IBrowser> browserMap = new Dictionary<string, IBrowser> {
	{"edge", new Edge() }, {"firefox", new FireFox()}, {"chrome", new Chrome()}
};

if (showHelp || queryMode) { quiet = false; }


//	Check for help
if (showHelp) {
	PrintUsage();
	return;
}

//	Obtain list of browsers
if (args.Any(a => a.StartsWith("--browser"))) {
	if (args.Contains("--browser:all")) {
		browsers = new string[] { "edge", "firefox", "chrome" };
	} else {
		browsers = args
			.Where(a => a.StartsWith("--browser") && -1 < a.IndexOf(':'))
			.Select(a => a.Substring(a.IndexOf(':') + 1).Trim().ToLower())
			.ToArray();
		if (0 == browsers.Length) { browsers = new string[] { "edge" }; }
	}
}

//	Check for query mode: just display installed Edge versions, or report non-presence
if (queryMode) {
	foreach (string browser in browsers) {
		if (!browserMap.ContainsKey(browser)) {
			PrintLine($"Unrecognized browser: {browser}");
			continue;
		}
		PrintInstalledBrowser(browserMap[browser]);
	}
	return;
}


//	Hook up the DisplayMessage event for each browser.
//		TODO:	Only hook up the event is not in quiet mode.
foreach (string browserName in browsers) {
	IBrowser browser = browserMap[browserName];
	browser.DisplayMessage += (s, e) => {
		PrintLine($"[{((IBrowser)s).Name}] {e.Message}");
	};
}


//	For each requested browser, check if it is already installed.  If so, skip.
foreach (string browserName in browsers) {
	if (!browserMap.ContainsKey(browserName)) {
		PrintLine($"Unrecognized browser: {browserName}");
		continue;
	}
	IBrowser browser = browserMap[browserName];
	if (!force && browser.IsInstalled()) {
		PrintLine($"{browser.Name} web browser is already installed.");
		PrintInstalledBrowser(browser);
		continue;
	}

	browser.Download();
}

if (!downloadOnly) {
	foreach (string browserName in browsers) {
		if (!browserMap.ContainsKey(browserName)) {
			continue;
		}

		IBrowser browser = browserMap[browserName];
		if (browser.IsDownloaded) {
			browser.Install();
			browser.CleanUp();
		}
	}
} else {
	//	download-only
	foreach (string browserName in browsers) {
		if (!browserMap.ContainsKey(browserName)) {
			continue;
		}

		IBrowser browser = browserMap[browserName];
		if (browser.IsDownloaded) {
			PrintLine($"{browser.Name} downloaded to\r\n'{browser.DownloadDestinationPath}'");
		}
	}
}





void PrintInstalledBrowser(IBrowser browser) {
	if (!browser.IsInstalled()) {
		PrintLine($"{browser.Name} is not installed.");
	} else {
		int maxKeyLength = browser.Items.Select(i => i.Key.Length).Max();

		foreach (string key in browser.Items.Keys) {
			PrintLine($"{key.PadRight(maxKeyLength)} : {browser.Items[key]}");
		}
	}
	Console.WriteLine();
}



void PrintUsage() {
	Console.WriteLine("install-edge.exe --help");
	Console.WriteLine("install-edge.exe --query");
	Console.WriteLine("install-edge.exe [--force] [--quiet] [--browser:<value>] [--download-only]");
	Console.WriteLine();
	Console.WriteLine("  --query            Displays the currently installed Edge version");
	Console.WriteLine("  --help             This screen");
	Console.WriteLine("  --force            Skip the 'is Edge already installed' check");
	Console.WriteLine("  --download-only    Only download the installers, do not run them.");
	Console.WriteLine("  --browser:<value>  Specify which browser(s) you want to install.");
	Console.WriteLine("                     <value> may be 'edge', 'chrome', 'forefox', or 'all'");
	Console.WriteLine("                     This flag may be specified multiple times, ");
	Console.WriteLine("                          e.g. --browser:firefox --browser:edge");
	Console.WriteLine("  --quiet            Do not display any output to the console");
	Console.WriteLine();
}



void PrintLine(string message) {
	if (!quiet) {
		Console.WriteLine(message);
	}
}