using CollageCommander.Cli.Repositories;
using CollageCommander.Cli.Services.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollageCommander.Cli
{
    class Program
    {
        private static readonly UnityContainer Container = UnityBootstrapper.Configure();

        static void Main(string[] args)
        {
            var builder = Container.Resolve<IBuilderService>();
            var buildTask = builder.Build(@"C:\Users\John\Downloads\1Y",
                                          imagesCount: 144,
                                          imageSize: 256,
                                          outputColumns: 14,
                                          outputRows: 11);

            var outputFile = buildTask.Result;
            Process.Start(outputFile.FullName);
        }
    }
}
