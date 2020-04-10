using JetBrains.Annotations;
using ReQube.Models;
using System;

namespace ReQube
{
    [PublicAPI]
    public class SonarReportGeneratorFactory
    {
        [PublicAPI]
        public virtual ISonarReportGenerator GetGenerator(SonarOutputFormat format) =>      
            format switch
            {
                SonarOutputFormat.Generic => new SonarGenericReportGenerator(),
                SonarOutputFormat.Roslyn => new SonarRoslynReportGenerator(),
                _ => throw new NotSupportedException($"Format {format} is not supported.")
            };
        }  
}
