using ReQube.Models.ReSharper;
using ReQube.Models.SonarQube;
using System.Collections.Generic;

namespace ReQube
{
    public interface ISonarReportGenerator
    {
        public List<ISonarReport> Generate(Report reSharperReport);

        public int GetSonarLine(ushort reSharperLine)
        {
            // For some reason, some issues don't have line, but actually they are on the first one
            return reSharperLine > 0 ? reSharperLine : 1;
        }
    }
}
