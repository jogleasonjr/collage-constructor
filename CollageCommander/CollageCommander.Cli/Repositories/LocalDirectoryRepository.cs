using CollageCommander.Cli.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CollageCommander.Cli.Repositories
{
    public class LocalDirectoryRepository : IImageRepository
    {
        public Task<IEnumerable<FileInfo>> Get(string path, int count)
        {
            return Task.Run(() => Directory.GetFiles(path).Take(count).Select(f => new FileInfo(f)));
        }
    }
}
