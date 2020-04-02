# dotnet-reqube

[![Build status](https://ci.appveyor.com/api/projects/status/kb0260n7o1alqyqv?svg=true)](https://ci.appveyor.com/project/todor/dotnet-reqube)
[![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=dotnet-reqube&metric=alert_status)](https://sonarcloud.io/dashboard?id=dotnet-requbex)
[![NuGet](https://img.shields.io/nuget/v/dotnet-requbex.svg)](https://www.nuget.org/packages/dotnet-requbex/)

.NET Core global tool that converts [ReSharper inspect code](https://www.jetbrains.com/help/resharper/InspectCode.html) report to [SonarQube format](https://docs.sonarqube.org/display/SONAR/Generic+Issue+Data)
or [Roslyn Static Code Analysis Results Format](http://json.schemastore.org/sarif-1.0.0).

## Installation

`dotnet tool install --global dotnet-reqube`

## Usage

```
dotnet-reqube [Options]

  -i, --input        Required. ReSharper report in XML format.
  -o, --output       Required. SonarQube report file name.
  -d, --directory    Directory where reports will be saved. Working directory will be used if not set.
  -p, --project      Project to create SonarQube report for. If not set, a report is written for all projects found in
                     the solution.
  -f, --format       (Default: Generic) SonarQube report output format.
  --exclude-rules    Specify the ReSharper rules to exclude from the analysis (Issue[TypeId] from ReSharper's output).
                     The format is <type id>[##<message regex>]|<type id>[##<message regex>]...E.g. CSharpErrors##The
                     modifier 'public'.*|UnusedMemberInSuper.Global|NotAccessedField.Global
  --sonar-dir        The path to the .sonarqube directory of the executing sonar analysis.
  --help             Display this help screen.
  --version          Display version information.
```

Create report for all projects in a solution:
`dotnet-reqube -i ResharperReport.xml -o SonarQubeReportFileName.json -d Path\To\Output\Directory`

Create report for a single project in a solution:
`dotnet-reqube -i ReSharperReport.xml -o SonarQubeReportFileName.json -d Path\To\Output\Directory -p ProjectToReport`

### Roslyn Analyzers Format

*dotnet-reqube* also supports the generation of the files in accordance to the format used by the Roslyn Analyzers.
It allows specifying a description and thus adding a link to the ReSharper site with the rules. 

```
dotnet-reqube -i ResharperReport.xml -o SonarQubeReportFileName.json -f Roslyn
```

*&lt;ProjectName&gt;* can be used as a placeholder in the output file name and will be replaced with the name of the project 
for which a report is generated. 

E.g.

```
dotnet-reqube -i ResharperReport.xml -o "<ProjectName>.dll.ReSharper.RoslynCA.json" -f Roslyn
```

To send the generated Roslyn-like reports to SonarQube/SonarCloud, using 
[sonarscanner](https://docs.sonarqube.org/latest/analysis/scan/sonarscanner/), you have two options:
* Modify its *SonarQube.Integration.targets* file (located in *&lt;UserProfile&gt;/.dotnet/tools/.store/dotnet-sonarscanner
  \<version>\dotnet-sonarscanner\<version>\tools\netcoreapp3.0\any\Targets*).

  Find the line `
  <SonarCompileErrorLog>$(TargetDir)$(TargetFileName).RoslynCA.json</SonarCompileErrorLog>` 
  and add `<SonarReSharperErrorLog>$(TargetDir)$(TargetFileName).ReSharper.RoslynCA.json</SonarReSharperErrorLog>` 
  after it.

  **Note:** If you use output file different from `<ProjectName>.dll.ReSharper.RoslynCA.json`, then `$(TargetFileName).ReSharper.RoslynCA.json`
  should be adjusted accordingly.

  In the same targets file, find `<SonarReportFilePath Condition=" $(SonarCompileErrorLog) != '' AND  $([System.IO.File]::Exists($(SonarCompileErrorLog))) == 'true' " Include="$(SonarCompileErrorLog)" />` 
  and add the following line to the same item group: `
  <SonarReportFilePath Condition=" $(SonarResharperErrorLog) != '' AND  $([System.IO.File]::Exists($(SonarResharperErrorLog))) == 'true' " Include="$(SonarResharperErrorLog)" />`

  After invoking `dotnet build ...`, the ReSharper sonar reports will be taken into account.

* [Preferred] Specify `--sonar-dir` option and provide the path to the *.sonarqube* directory created after `dotnet sonarscanner begin...`
  This will modify the necessary files sent to SonarQube / SonarCloud without any manual work.

  **Note:** This requires the solution/project to be built before running ReSharper's analysis. 
  This is the preferred option, because otherwise for complex projects, there might be false positives.
  **This is used with the Roslyn output format only.**
  ```
  dotnet-reqube -i ResharperReport.xml -o "<ProjectName>.ReSharper.RoslynCA.json" -f Roslyn
                --sonar-dir ".sonarqube"
  ```

### Exclude rules

Most of the times, it's better to use *.sln.DotSettings* to control which rules to be used during a ReSharper's 
analysis. Some times however it's more convinient to filter them as part of dotnet-reqube. Or in the case of 
`CSharpErrors`, this is the only option, since they cannot be filtered using DotSettings.

The format is: `<type id>[##<message regex>]|<type id>[##<message regex>]...`

E.g. 

```
dotnet-reqube -i ResharperReport.xml -o "<ProjectName>.dll.ReSharper.RoslynCA.json" -f Roslyn 
              --exclude-rules "CSharpErrors##Themodifier 'public'.*|UnusedMemberInSuper.Global|NotAccessedField.Global"
```
## Demo

Example project that shows how you can import ReSharper issues to SonarQube using the `dotnet-reqube`
https://github.com/olsh/resharper-to-sonarqube-example
