$nugetPath = Get-ToolPath -Name 'NuGet.exe'
if (-not $nugetPath)
{
    Write-Warning (Get-LocalizedString -Key "Unable to locate nuget.exe. Package restore will not be performed for the solutions")
}

Invoke-Tool -Path $nugetPath -Arguments "restore Comet.sln -NonInteractive -Source https://api.nuget.org/v3/index.json"
