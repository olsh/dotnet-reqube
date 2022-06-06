using System.Collections.Generic;

namespace ReQube
{
    public interface ISonarMetaDataWriter
    {
        void AddReSharperAnalysisPaths(List<KeyValuePair<string, string>> reportPathsByProject);
    }
}
