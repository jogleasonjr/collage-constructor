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
            var buildTask = builder.Build("owl",
                                          imagesCount: 64,
                                          imageSize: 256,
                                          outputColumns: 8,
                                          outputRows: 8);

            var outputFile = buildTask.Result;
            Process.Start(outputFile.FullName);
        }
    }
}
