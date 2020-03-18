# dotnet-reqube

[![Build status](https://ci.appveyor.com/api/projects/status/kb0260n7o1alqyqv?svg=true)](https://ci.appveyor.com/project/todor/dotnet-reqube)
[![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=dotnet-reqube&metric=alert_status)](https://sonarcloud.io/dashboard?id=dotnet-requbex)
[![NuGet](https://img.shields.io/nuget/v/dotnet-requbex.svg)](https://www.nuget.org/packages/dotnet-requbex/)

.NET Core global tool that converts [ReSharper inspect code](https://www.jetbrains.com/help/resharper/InspectCode.html) report to [SonarQube format](https://docs.sonarqube.org/display/SONAR/Generic+Issue+Data)
or [Roslyn Static Code Analysis Results Format](http://json.schemastore.org/sarif-1.0.0).

## Installation

`dotnet tool install --global dotnet-reqube`

## Usage

Create report for all projects in solution:
`dotnet-reqube -i ResharperReport.xml -o SonarQubeReportFileName.json -d Path\To\Output\Directory`

Create report for a single project in solution:
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
[sonarscanner](https://docs.sonarqube.org/latest/analysis/scan/sonarscanner/), a small modification should be made
in its *SonarQube.Integration.targets* file (located in 
*&lt;UserProfile&gt;/.dotnet/tools/.store/dotnet-sonarscanner\<version>\dotnet-sonarscanner\<version>\tools\netcoreapp3.0\any\Targets*).

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

## Demo

Example project that shows how you can import ReSharper issues to SonarQube using the `dotnet-reqube`
https://github.com/olsh/resharper-to-sonarqube-example
