# Download  
Make sure you have installed [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0) prior to downloading install-edge.  
[.NET Runtime 6.x]

Download the install-edge app from the Releases section of this repo.  


# Usage  
ONLY FOR WINDOWS.  Commend-line tool to download and install MS Edge, Mozilla FireFox, and/or Google Chrome.

```
Flags:
 --quiet           Supresses all console messages except uncaught exceptions.

 --force           Skips "Edge already installed" check.

 --browser:<value> Select which browser you would like to install.
                   Valid values are "edge", "firefox", and "chrome".  A value of "all" will specify all known browsers.
                   Multiple flags may be present.  e.g. `--browser:edge --browser:chrome`
                   DEFUALT: "edge"

--download-only    Only download the installers, but do not run them.

 --query           Only displays the installed version of Edge and its components.  
                   --quiet has no effect with --query.  

 --help            Show the usage screen.  No other parameters will be considered when this one is present.
```



# Why?  
Another tool for the toolbox.  

Also, new Windows Server machines typically only have IE 11 in 'Enhanced Security Mode' pre-installed.  Personally I get pretty sick of having to remember how to disable that mode or add the myriad of exceptions to the security filter in order to download a new browser.


# Alternatives  
This [simple powershell](https://techexpert.tips/powershell/powershell-installing-microsoft-edge/) script is a great alternative, and is in fact what install-edge was based on.  

``` powershell
md -Path $env:temp\edgeinstall -erroraction SilentlyContinue | Out-Null
$Download = join-path $env:temp\edgeinstall MicrosoftEdgeEnterpriseX64.msi
Invoke-WebRequest 'https://msedge.sf.dl.delivery.mp.microsoft.com/filestreamingservice/files/a2662b5b-97d0-4312-8946-598355851b3b/MicrosoftEdgeEnterpriseX64.msi'  -OutFile $Download
Start-Process "$Download" -ArgumentList "/quiet /passive"
```