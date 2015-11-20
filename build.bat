@echo off
@echo.
@echo *******************************************
@echo * BUILD STARTING   			*
@echo *******************************************
@echo.

call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\VsMSBuildCmd.bat"

msbuild /verbosity:quiet /fl /t:Rebuild /p:Configuration=Release,OutputPath=bin\Release\NuGet\ /property:GenerateLibraryLayout=false /p:NoWarn=0618 src\Comet.csproj

@echo.
@echo *******************************************
@echo * COPYING BINARIES                        *
@echo *******************************************
@echo.

pushd Tools\nuget

mkdir .\Comet\lib\uap10.0
mkdir .\Comet\lib\uap10.0\Comet
mkdir .\Comet\lib\uap10.0\Comet\Themes
mkdir .\Comet\lib\uap10.0\Comet\Properties
copy ..\..\src\bin\release\NuGet\Comet.dll .\Comet\lib\uap10.0\
copy ..\..\src\bin\release\NuGet\Comet.pri .\Comet\lib\uap10.0\
copy ..\..\src\bin\release\NuGet\Comet.xr.xml .\Comet\lib\uap10.0\Comet\
copy ..\..\src\bin\release\NuGet\themes\generic.xbf .\Comet\lib\uap10.0\Comet\Themes
copy ..\..\src\Properties\Comet.rd.xml .\Comet\lib\uap10.0\Comet\Properties

@echo.
@echo *******************************************
@echo * BUILDING NUGET 				*
@echo *******************************************
@echo.

mkdir .\package
nuget pack Comet\Comet.nuspec -o .\package

@echo.
@echo *******************************************
@echo * DONE 		 			*
@echo *******************************************
@echo.

explorer .\package

popd