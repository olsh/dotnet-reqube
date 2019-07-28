# dotnet-reqube

[![Build status](https://ci.appveyor.com/api/projects/status/kb0260n7o1alqyqv?svg=true)](https://ci.appveyor.com/project/olsh/reqube)
[![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=dotnet-reqube&metric=alert_status)](https://sonarcloud.io/dashboard?id=dotnet-reqube)
[![NuGet](https://img.shields.io/nuget/v/dotnet-reqube.svg)](https://www.nuget.org/packages/dotnet-reqube/)

.NET Core global tool that converts [ReSharper inspect code](https://www.jetbrains.com/help/resharper/InspectCode.html) report to [SonarQube format](https://docs.sonarqube.org/display/SONAR/Generic+Issue+Data)

## Installation

`dotnet tool install --global dotnet-reqube`

## Usage

Create report for all projects in solution:
`dotnet-reqube -i ResharperReport.xml -o SonarQubeReportFileName.json -d Path\To\Output\Directory`

Create report for a single project in solution:
`dotnet-reqube -i ReSharperReport.xml -o SonarQubeReportFileName.json -d Path\To\Output\Directory -p ProjectToReport`

## Demo

Example project that shows how you can import ReSharper issues to SonarQube using the `dotnet-reqube`
https://github.com/olsh/resharper-to-sonarqube-example
