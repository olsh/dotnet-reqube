using ReQube.Models.ReSharper;
using ReQube.Models.SonarQube;
using ReQube.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace ReQube
{
    public abstract class SonarBaseReportGenerator : ISonarReportGenerator
    {
        public abstract List<ISonarReport> Generate(Report reSharperReport);

        protected void ReadIssueFile(string file, ref string lastLoadedFilePath, ref string lastLoadedFileContent)
        {
            if (lastLoadedFilePath != file)
            {
                lastLoadedFilePath = file;
                lastLoadedFileContent = File.ReadAllText(lastLoadedFilePath);
            }
        }

        protected static (int StartColumn, int EndColumn) FindLineOffset(string globalOffset, string fileContent)
        {
            var offsetTokens = globalOffset.Split("-", StringSplitOptions.RemoveEmptyEntries);
            var globalStart = int.Parse(offsetTokens[0]);
            var globalEnd = int.Parse(offsetTokens[1]);

            return FileUtils.FindLineOffset(fileContent, globalStart, globalEnd);
        }
    }
}
