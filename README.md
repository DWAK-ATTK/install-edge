# Download  
Make sure you have installed [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0) prior to downloading install-edge.  
[.NET Runtime 6.x]

Download the install-edge app from the Releases section of this repo.  


# Usage  
ONLY FOR WINDOWS.  
Commend-line tool.  

```
Flags:
 --quiet       Supresses all console messages except uncaught exceptions.

 --force       Skips "Edge already installed" check.

 --query       Only displays the installed version of Edge and its components.  
               --quiet has no effect with --query.  

 --help        Show the usage screen.  No other parameters will be considered when this one is present.
```



# Why?  
Another tool for the toolbox.


# Alternatives  
This [simple powershell](https://techexpert.tips/powershell/powershell-installing-microsoft-edge/) script is a great alternative, and is in fact what install-edge was based on.  

``` powershell
md -Path $env:temp\edgeinstall -erroraction SilentlyContinue | Out-Null
$Download = join-path $env:temp\edgeinstall MicrosoftEdgeEnterpriseX64.msi
Invoke-WebRequest 'https://msedge.sf.dl.delivery.mp.microsoft.com/filestreamingservice/files/a2662b5b-97d0-4312-8946-598355851b3b/MicrosoftEdgeEnterpriseX64.msi'  -OutFile $Download
Start-Process "$Download" -ArgumentList "/quiet /passive"
```