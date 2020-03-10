#tool nuget:?package=MSBuild.SonarQube.Runner.Tool&version=4.6.0
#tool nuget:?package=JetBrains.ReSharper.CommandLineTools&version=2019.3.1
#addin nuget:?package=Cake.Sonar&version=1.1.22
#addin "Cake.FileHelpers"

// set the following envrionement variables before running the cake build:
// sonar:organization, sonar:apikey, sonar:projectKey, sonar:projectName, nuget:projectName,
// packageReleaseNotes, packageProjectUrl, packageLicenseUrl, repositoryUrl

using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

public class CoverletSettings : ToolSettings 
{
    public string BuildConfiguration { get; set; }
}

// https://github.com/Romanx/Cake.Coverlet has a bug and works with Debug configuration only
public sealed class CoverletTool : Tool<CoverletSettings>
{
    private readonly ICakeEnvironment _environment;

    public CoverletTool(
        IFileSystem fileSystem, 
        ICakeEnvironment environment, 
        IProcessRunner processRunner, 
        IToolLocator tools)
        : base(fileSystem, environment, processRunner, tools)
    {
        _environment = environment;
    }

    protected override string GetToolName() => "coverlet";

    public void Run(FilePath testFile, FilePath testProject, CoverletSettings settings)
    {
        Run(settings, GetArguments(testFile, testProject, settings));
    }

    private ProcessArgumentBuilder GetArguments(
        FilePath coverageFile,
        FilePath testProject,
        CoverletSettings settings)
    {
        var argumentBuilder = new ProcessArgumentBuilder();

        argumentBuilder.AppendQuoted(coverageFile.MakeAbsolute(_environment).FullPath);
        argumentBuilder.AppendSwitch("--target", "dotnet");
            
        // spaces in project path will not work
        argumentBuilder.AppendSwitchQuoted(
            "--targetargs", 
            $"test {testProject.MakeAbsolute(_environment)} --no-build --configuration {settings.BuildConfiguration}");
        argumentBuilder.AppendSwitch("--format", "opencover");
        argumentBuilder.AppendSwitchQuoted("--output", 
            new DirectoryPath("coverage-results/coverage").MakeAbsolute(_environment).FullPath);

        return argumentBuilder;
    }

    protected override IEnumerable<string> GetToolExecutableNames() => new [] { "coverlet", "coverlet.exe" };
}

var target = Argument("target", "Default");

var buildConfiguration = "Release";
var nugetProjectName = EnvironmentVariable("nuget:projectName") ?? "dotnet-reqube";
var projectName = "dotnet-reqube";
var projectFolder = string.Format("./src/{0}/", projectName);
var solutionFile = string.Format("./src/{0}.sln", projectName);
var projectFile = string.Format("./src/{0}/{0}.csproj", projectName);
var sonarOrganization = EnvironmentVariable("sonar:organization") ?? "olsh-github";
var sonarProjectKey = EnvironmentVariable("sonar:projectKey") ?? "dotnet-reqube";
var sonarProjectName = EnvironmentVariable("sonar:projectName") ?? "dotnet reqube";
var extensionsVersion = XmlPeek(projectFile, "Project/PropertyGroup[1]/VersionPrefix/text()");
var branch = BuildSystem.AppVeyor.Environment.Repository.Branch;
var isMasterBranch = StringComparer.OrdinalIgnoreCase.Equals("master", branch);
var isPullRequest = BuildSystem.AppVeyor.Environment.PullRequest.IsPullRequest;

Task("UpdateBuildVersion")
  .WithCriteria(BuildSystem.AppVeyor.IsRunningOnAppVeyor)
  .Does(() =>
{
    var buildNumber = BuildSystem.AppVeyor.Environment.Build.Number;

    BuildSystem.AppVeyor.UpdateBuildVersion(string.Format("{0}.{1}", extensionsVersion, buildNumber));
});

Task("NugetRestore")
  .Does(() =>
{
    DotNetCoreRestore(solutionFile);
});

Task("Build")
  .Does(() =>
{
    var settings = new DotNetCoreBuildSettings
    {
        Configuration = buildConfiguration,
        MSBuildSettings = 
            new DotNetCoreMSBuildSettings()
                .WithProperty(
                    "PackageReleaseNotes", 
                    EnvironmentVariable("packageReleaseNotes") ?? "https://github.com/olsh/reqube/releases")
                .WithProperty(
                    "PackageProjectUrl", 
                    EnvironmentVariable("packageProjectUrl") ?? "https://github.com/olsh/reqube")
                .WithProperty(
                    "PackageLicenseUrl", 
                    EnvironmentVariable("packageLicenseUrl") ?? "https://raw.githubusercontent.com/olsh/reqube/master/LICENSE")
                .WithProperty(
                    "RepositoryUrl", 
                    EnvironmentVariable("repositoryUrl") ?? "https://github.com/olsh/reqube")
    };

    DotNetCoreBuild(solutionFile, settings);
});

Task("Test")
    .IsDependentOn("Build")
    .Does(cxt =>
    {
        var testSettings = new DotNetCoreTestSettings()
        {
            Configuration = buildConfiguration,
            NoBuild = true
        };

        var coverletSettings = new CoverletSettings
        {
            BuildConfiguration = buildConfiguration
        };

        var projects = GetFiles("./src/**/*.tests.csproj");
        foreach(var project in projects)
        {
            var nameWithoutExtension = project.GetFilenameWithoutExtension();
            var dir = project.GetDirectory();

            var dllFile = 
                cxt
                    .Globber
                    .GetFiles(
                        $"{dir.MakeAbsolute(cxt.Environment)}/bin/**/{buildConfiguration}/**/{nameWithoutExtension}.dll")
                    .FirstOrDefault();

            if (dllFile == null) 
            {
                throw new Exception($"Could not find {nameWithoutExtension}.dll");
            }

            Information($"Test DLL is {dllFile}.");

            DotNetCoreTest(project.FullPath, testSettings);
            new CoverletTool(
                cxt.FileSystem, cxt.Environment, cxt.ProcessRunner, cxt.Tools).Run(dllFile, project, coverletSettings);
        }    
});

Task("ReSharperInspect")
  .IsDependentOn("NugetRestore")
  .Does(() =>
{
    InspectCode(solutionFile, new InspectCodeSettings {
         OutputFile = File("resharper-report.xml")
    });

    if (isPullRequest)
    {
        var reSharperOutput = FileReadText("resharper-report.xml");
        Console.WriteLine("ReSharper analysis results:");
        Console.WriteLine(reSharperOutput);
    }
});

Task("ConvertReSharperToSonar")
  .WithCriteria(!isPullRequest)
  .IsDependentOn("ReSharperInspect")
  .Does(() =>
{
    StartProcess("dotnet-reqube", $"-i resharper-report.xml -o sonarqube-report.json -d {MakeAbsolute(Directory("./src/"))}");
});

// Sonar Analysis is not possible on PRs right now, without exposing the user token, which is a vulnerability;
// https://jira.sonarsource.com/browse/MMF-1371
Task("SonarBegin")
  .IsDependentOn("ConvertReSharperToSonar")
  .WithCriteria(!isPullRequest)
  .Does(() => {
     SonarBegin(new SonarBeginSettings {
        Url = "https://sonarcloud.io",
        Login = EnvironmentVariable("sonar:apikey"),
        Key = sonarProjectKey,
        Name = sonarProjectName,
        ArgumentCustomization = args => args
            .Append($"/o:{sonarOrganization}")
            .Append("/d:sonar.externalIssuesReportPaths=sonarqube-report.json")
            .Append("/d:sonar.cs.opencover.reportsPaths=\"coverage-results/coverage.opencover.xml\"")
            .Append("/d:sonar.coverage.exclusions=\"**Test*.cs\"")
            .Append(
                BuildSystem.AppVeyor.IsRunningOnAppVeyor && !isMasterBranch ? $"/d:sonar.branch.name={branch}" : ""),
        Version = extensionsVersion
     });
  });

Task("SonarEnd")
  .WithCriteria(!isPullRequest)
  .Does(() => {
     SonarEnd(new SonarEndSettings {
        Login = EnvironmentVariable("sonar:apikey")
     });
  });

Task("NugetPack")
  .IsDependentOn("Build")
  .Does(() =>
{
     var settings = new DotNetCorePackSettings
     {
         Configuration = buildConfiguration,
         OutputDirectory = ".",
         NoBuild = true,
         ArgumentCustomization = args => args.Append($"-p:PackageId={nugetProjectName}")
     };

     DotNetCorePack(projectFolder, settings);
});

Task("CreateArtifact")
  .IsDependentOn("NugetPack")
  .WithCriteria(BuildSystem.AppVeyor.IsRunningOnAppVeyor)
  .WithCriteria(isMasterBranch)
  .Does(() =>
{
    BuildSystem.AppVeyor.UploadArtifact(string.Format("{0}.{1}.nupkg", nugetProjectName, extensionsVersion));
});

Task("Default")
    .IsDependentOn("NugetPack");

Task("Sonar")
  .IsDependentOn("SonarBegin")
  .IsDependentOn("Build")
  .IsDependentOn("Test")
  .IsDependentOn("SonarEnd");

Task("CI")
    .IsDependentOn("UpdateBuildVersion")
    .IsDependentOn("Sonar")
    .IsDependentOn("CreateArtifact");

RunTarget(target);
