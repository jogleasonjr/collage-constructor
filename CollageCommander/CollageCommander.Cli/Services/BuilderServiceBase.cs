using CollageCommander.Cli.Services.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CollageCommander.Cli.Services
{
    public abstract class BuilderServiceBase : IBuilderService
    {
        public abstract Task<FileInfo> Build(string imagesPath, int imagesCount, int imageHeightWidth, int outputColumns, int outputRows);

        protected DirectoryInfo CreateTempSandboxDirectory()
        {
            var dir = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            dir.Create();

            return dir;
        }
    }
}
