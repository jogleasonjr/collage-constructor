using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollageCommander.Cli.Services.Interfaces
{
    interface IBuilderService
    {
        Task<FileInfo> Build(string imagesPath, int imagesCount, int imageSize, int outputColumns, int outputRows);
    }
}
