using System;
using WixSharp;

class Script
{
    static public void Main()
    {
        WixEntity[] programDataFiles = { new File(@"Q2C.SSMS.AddIn") };
        var programDataDir = new Dir(@"%AppData%\Microsoft\MSEnvShared\Addins", programDataFiles);

        WixEntity[] programFilesFiles =
        {
            new File(@"bin\Release\Q2C.SSMS.dll"),
            new File(@"bin\Release\Q2C.Core.dll"),
            new File(@"bin\Release\SQLEditors.dll"),
            new File(@"bin\Release\SqlPackageBase.dll"),
            new File(@"bin\Release\SqlWorkbench.Interfaces.dll")
        };
        var programFilesDir = new Dir(@"%ProgramFiles%\serp1983\Q2C SSMSPlugin", programFilesFiles);

        var project = new Project("Q2C.SSMSPlugin", programFilesDir, programDataDir)
        {
            GUID = new Guid("5F991747-C988-4A89-AF25-E82608615803"),            
            UI = WUI.WixUI_ProgressOnly
        };

        Compiler.BuildMsi(project);
    }
}