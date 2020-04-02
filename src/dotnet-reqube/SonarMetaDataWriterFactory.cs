using JetBrains.Annotations;
using ReQube.Models;

namespace ReQube
{
    [PublicAPI]
    public class SonarMetaDataWriterFactory
    {
        [PublicAPI]
        public virtual ISonarMetaDataWriter GetMetaDataWriter(Options options)
        {
            return options.SonarDirectory != null && options.OutputFormat == SonarOutputFormat.Roslyn 
                ? new SonarRoslynMetaDataWriter(options.SonarDirectory) : null;
        }
    }
}
