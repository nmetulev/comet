$sourceNugetExe = "https://dist.nuget.org/win-x86-commandline/v3.2.0/nuget.exe"
$targetNugetExe = "nuget.exe"
Invoke-WebRequest $sourceNugetExe -OutFile $targetNugetExe

nuget.exe restore Comet.sln -NonInteractive 
