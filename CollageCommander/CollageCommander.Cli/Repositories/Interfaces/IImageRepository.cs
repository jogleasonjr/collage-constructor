using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollageCommander.Cli.Repositories.Interfaces
{
    public interface IImageRepository
    {
        Task<IEnumerable<FileInfo>> Get(string path, int count);
    }
}
