#tool nuget:?package=MSBuild.SonarQube.Runner.Tool&version=4.3.1
#tool nuget:?package=JetBrains.ReSharper.CommandLineTools&version=2018.3.1

#addin nuget:?package=Cake.Sonar&version=1.1.18

var target = Argument("target", "Default");

var buildConfiguration = "Release";
var projectName = "dotnet-reqube";
var projectFolder = string.Format("./src/{0}/", projectName);
var solutionFile = string.Format("./src/{0}.sln", projectName);
var projectFile = string.Format("./src/{0}/{0}.csproj", projectName);

var extensionsVersion = XmlPeek(projectFile, "Project/PropertyGroup[1]/VersionPrefix/text()");

Task("UpdateBuildVersion")
  .WithCriteria(BuildSystem.AppVeyor.IsRunningOnAppVeyor)
  .Does(() =>
{
    var buildNumber = BuildSystem.AppVeyor.Environment.Build.Number;

    BuildSystem.AppVeyor.UpdateBuildVersion(string.Format("{0}.{1}", extensionsVersion, buildNumber));
});

Task("Build")
  .Does(() =>
{
    var settings = new DotNetCoreBuildSettings
    {
        Configuration = buildConfiguration
    };

    DotNetCoreBuild(solutionFile, settings);
});

Task("ReSharperInspect")
  .IsDependentOn("Build")
  .Does(() =>
{
    InspectCode(solutionFile, new InspectCodeSettings {
         OutputFile = File("resharper-report.xml")
    });
});

Task("ConverReSharperToSonar")
  .IsDependentOn("ReSharperInspect")
  .Does(() =>
{
    StartProcess("dotnet-reqube", $"-i resharper-report.xml -o sonarqube-report.json -d {MakeAbsolute(Directory("./src/"))}");
});

Task("SonarBegin")
  .IsDependentOn("ConverReSharperToSonar")
  .Does(() => {
     SonarBegin(new SonarBeginSettings {
        Url = "https://sonarcloud.io",
        Login = EnvironmentVariable("sonar:apikey"),
        Key = "dotnet-reqube",
        Name = "dotnet reqube",
        ArgumentCustomization = args => args
            .Append("/o:olsh-github")
            .Append("/d:sonar.externalIssuesReportPaths=sonarqube-report.json"),
        Version = extensionsVersion
     });
  });

Task("SonarEnd")
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
         OutputDirectory = "."
     };

     DotNetCorePack(projectFolder, settings);
});

Task("CreateArtifact")
  .IsDependentOn("NugetPack")
  .WithCriteria(BuildSystem.AppVeyor.IsRunningOnAppVeyor)
  .Does(() =>
{
    BuildSystem.AppVeyor.UploadArtifact(string.Format("{0}.{1}.nupkg", projectName, extensionsVersion));
});

Task("Default")
    .IsDependentOn("NugetPack");

Task("Sonar")
  .IsDependentOn("SonarBegin")
  .IsDependentOn("Build")
  .IsDependentOn("SonarEnd");

Task("CI")
    .IsDependentOn("UpdateBuildVersion")
    .IsDependentOn("Sonar")
    .IsDependentOn("CreateArtifact");

RunTarget(target);
