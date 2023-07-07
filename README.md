# Windows.Service.Template.Package Instructions
1. Make sure the Wix Toolset is installed: `dotnet tool install --global wix`
2. Make sure you have the HeatWave extension installed which can be found in the Extensions menu from within Visual Studio.
3. Publish the Windows.Service.Template project, all the publish options should be set by default.
4. Make sure you have the Solution set to Release.
5. Build the Windows.Service.Template.Package project to create the installer
6. The installer should be in the Package project bin folder
