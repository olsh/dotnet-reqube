using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;

namespace ReQubeSampleProject
{
    class Program
    {
        [NotNull]
        string TestProperty { get; set; }

        static void Main(string[] args)
        {
            new Program().Run(null);
        }

        void Run(string redundantArg)
        {
            TestProperty = null;
            var streamReader = new StreamReader("");            
        }

        void Process(StreamReader streamReader = null, int x)
        {
            streamReader.ReadToEnd();
        }
    }
}
