using Nuke.Common.IO;

sealed partial class Build
{
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "output";
    readonly AbsolutePath ChangeLogPath = RootDirectory / "Changelog.md";

    protected override void OnBuildInitialized()
    {
        Configurations =
        new string[] {
            "Release*",
            "Installer*"
        };

        InstallersMap = new()
        {
            {Solution.Installer, Solution.BCFExpress}
        };

        VersionMap = new()
        {
            // {"Release R19", "2019.0.01"},
            // {"Release R20", "2020.0.01"},
            // {"Release R21", "2021.0.01"},
            // {"Release R22", "2022.0.01"},
            {"Release R23", "2023.0.01"},
            // {"Release R24", "2024.0.01"}
        };
    }
}