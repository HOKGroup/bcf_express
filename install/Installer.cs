using System;
using System.Collections.Generic;
using Installer;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;

const string outputName = "BCFExpress";
const string projectName = "BCFExpress";

var guidMap = new Dictionary<int, string>
{
    {2018, "7D940019-6ED8-4E1D-8208-55E302F21A8B"},
    {2019, "F8C7156F-255F-48F5-A000-D5590E059545"},
    {2020, "0122F571-6153-4CC7-AF23-90C459A2F5F0"},
    {2021, "2F2060FC-4821-45B2-9EDE-84ED05BFC00B"},
    {2022, "145E368C-9BE0-4653-BC41-BD309DCE9C05"},
    {2023, "00645EC4-0A1A-4B07-97D7-BA59F1F74E5B"},
    {2024, "173BFCD1-AF84-4248-A44F-A5CBFC6B4F32"}
};

var versions = Tools.ComputeVersions(args);
if (!guidMap.TryGetValue(versions.RevitVersion, out var guid))
{
    throw new Exception($"Version GUID mapping missing for the specified version: '{versions.RevitVersion}'");
}

var project = new Project
{
    OutDir = "output",
    Name = projectName,
    GUID = new Guid(guid),
    Platform = Platform.x64,
    UI = WUI.WixUI_InstallDir,
    Version = versions.InstallerVersion,
    MajorUpgrade = MajorUpgrade.Default,
    BackgroundImage = @"install\Resources\Icons\BackgroundImage.png",
    BannerImage = @"install\Resources\Icons\BannerImage.png",
    ControlPanelInfo =
    {
        Manufacturer = "HOK",
        HelpLink = "https://github.com/HOKGroup/bcf_express/issues",
        ProductIcon = @"install\Resources\Icons\ShellIcon.ico"
    }
};

var wixEntities = Generator.GenerateWixEntities(args, versions.AssemblyVersion);
project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.InstallDirDlg);

BuildSingleUserMsi();
BuildMultiUserUserMsi();

void BuildSingleUserMsi()
{
    project.InstallScope = InstallScope.perUser;
    project.OutFileName = $"{outputName}-{versions.AssemblyVersion}-SingleUser";
    project.Dirs =
    new Dir[] {
        new InstallDir($@"%AppDataFolder%\Autodesk\Revit\Addins\{versions.RevitVersion}", wixEntities)
    };
    project.BuildMsi();
}

void BuildMultiUserUserMsi()
{
    project.InstallScope = InstallScope.perMachine;
    project.OutFileName = $"{outputName}-{versions.AssemblyVersion}-MultiUser";
    project.Dirs =
    new Dir[] {
        new InstallDir($@"%CommonAppDataFolder%\Autodesk\Revit\Addins\{versions.RevitVersion}", wixEntities)
    };
    project.BuildMsi();
}