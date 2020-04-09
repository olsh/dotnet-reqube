using System.Collections.Generic;

namespace ReQube
{
    public interface ISonarMetaDataWriter
    {
        void AddReSharperAnalysisPaths(IDictionary<string, string> reportPathsByProject);
    }
}
