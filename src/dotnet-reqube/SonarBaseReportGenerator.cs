using System;
using System.Collections.Generic;
using System.IO;

using ReQube.Logging;
using ReQube.Models.ReSharper;
using ReQube.Models.SonarQube;
using ReQube.Utils;

using Serilog;

namespace ReQube
{
    public abstract class SonarBaseReportGenerator : ISonarReportGenerator
    {
        internal static ILogger Logger { private get; set; } = LoggerFactory.GetLogger();

        public abstract List<ISonarReport> Generate(Report reSharperReport);

        protected void ReadIssueFile(string file, ref string lastLoadedFilePath, ref string lastLoadedFileContent, ref string[] lastLoadedFileLines)
        {
            if (!File.Exists(file))
            {
                lastLoadedFilePath = "";
                lastLoadedFileContent = "";
                lastLoadedFileLines = null;
                return;
            }

            if (lastLoadedFilePath != file)
            {
                lastLoadedFilePath = file;
                lastLoadedFileContent = File.ReadAllText(lastLoadedFilePath);
                lastLoadedFileLines = File.ReadAllLines(lastLoadedFilePath);
            }
        }

        protected static (int? StartColumn, int? EndColumn) FindLineOffset(
            string globalOffset,
            int lineNumber,
            string filePath,
            string fileContent,
            string[] lastLoadedFileLines)
        {
            var offsetTokens = globalOffset.Split("-", StringSplitOptions.RemoveEmptyEntries);
            var globalStart = int.Parse(offsetTokens[0]);
            var globalEnd = int.Parse(offsetTokens[1]);

            var lineOffset = FileUtils.FindLineOffset(fileContent, globalStart, globalEnd);

            if (lastLoadedFileLines == null || lastLoadedFileLines.Length < lineNumber)
            {
                Logger.Warning(
                    "Unable to find line {LineNumber} in file {FilePath}, fallback to the entire line highlighting",
                    lineNumber,
                    filePath);

                return (null, null);
            }

            var line = lastLoadedFileLines[lineNumber - 1];
            if (lineOffset.StartColumn < 0 || line.Length < lineOffset.EndColumn)
            {
                Logger.Information(
                    "Line offset pointer ({StartOffset}, {EndOffset}) is out of range. {FilePath} has {LineLength} characters at line {LineNumber}, fallback to the entire line highlighting",
                    lineOffset.StartColumn,
                    lineOffset.EndColumn,
                    filePath,
                    line.Length,
                    lineNumber);

                return (null, null);
            }

            if (lineOffset.StartColumn == lineOffset.EndColumn)
            {
                lineOffset.EndColumn++;
            }

            return lineOffset;
        }
    }
}
