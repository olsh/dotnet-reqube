using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CommandLine;
using JetBrains.Annotations;
using ReQube.Models;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("dotnet-reqube.tests")]
namespace ReQube
{
    [UsedImplicitly]
    internal class Program
    {
        internal static void Main(string[] args)
        {
            new Program().Run(args);
        }

        internal void Run(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options => GetSonarConverter(options).Convert())
                .WithNotParsed(HandleParseError);
        }

        [UsedImplicitly]
        protected virtual ISonarConverter GetSonarConverter(Options options)
        {
            return new SonarConverter(options);
        }

        [UsedImplicitly]
        protected virtual void Exit(int statusCode)
        {
            Environment.Exit(statusCode);
        }
        
        private void HandleParseError(IEnumerable<Error> errors)
        {
            // ReSharper disable once SimplifyLinqExpression - for empty collections R#'s suggestion is incorrect
            if (!errors.Any(e => e.Tag == ErrorType.HelpRequestedError))
            {
                Exit(1);
            }
        }
    }
}
