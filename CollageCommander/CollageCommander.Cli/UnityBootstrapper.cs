using CollageCommander.Cli.Repositories;
using CollageCommander.Cli.Repositories.Interfaces;
using CollageCommander.Cli.Services;
using CollageCommander.Cli.Services.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollageCommander.Cli
{
    internal static class UnityBootstrapper
    {
        public static UnityContainer Configure()
        {
            var container = new UnityContainer();

            //container.RegisterType<IImageRepository, GoogleImageRepository>();
            container.RegisterType<IImageRepository, LocalDirectoryRepository>();
            container.RegisterType<IBuilderService, SimpleGridBuilderService>();

            return container;
        }
    }
}
