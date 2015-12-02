using CollageCommander.Cli.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.IO;
using CollageCommander.Cli.Repositories.Interfaces;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Collections;

namespace CollageCommander.Cli.Services
{
    class ImageIterator : IEnumerator<FileInfo>
    {
        private static readonly Random Random = new Random();

        private readonly IEnumerable<FileInfo> _files;

        private readonly List<FileInfo> _used = new List<FileInfo>();

        public ImageIterator(IEnumerable<FileInfo> files)
        {
            _files = files;
        }

        private FileInfo _current;
        public FileInfo Current
        {
            get
            {
                return _current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return _current;
            }
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

       

        public bool MoveNext()
        {
            if (_used.Count > 0 && _used.Count == _files.Count())
            {
               Reset();
            }

            var availableFiles = _files.Except(_used, new FileInfoEqualityComparer()).ToArray();

            _current = availableFiles[Random.Next(0, availableFiles.Length - 1)];

            _used.Add(_current);

            return true;
        }

        public class FileInfoEqualityComparer : IEqualityComparer<FileInfo>
        {
            public bool Equals(FileInfo x, FileInfo y)
            {
                return x.FullName.Equals(y.FullName);
            }

            public int GetHashCode(FileInfo obj)
            {
                return obj.FullName.GetHashCode();
            }
        }

        public FileInfo GetNext()
        {
            MoveNext();
            return Current;
        }


        public void Reset()
        {
            _used.Clear();
            _current = null;
        }
    }

    public class SimpleGridBuilderService : BuilderServiceBase
    {
        private static readonly Random Random = new Random();

        private readonly IImageRepository _imageRepository;
        public SimpleGridBuilderService(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        public override async Task<FileInfo> Build(string imagesPath, int imagesCount, int imageSize, int outputColumns, int outputRows)
        {
            var sandbox = CreateTempSandboxDirectory();
            var images = await _imageRepository.Get(imagesPath, imagesCount);

            foreach (var image in images)
            {
                ResizeImage(image.FullName, sandbox.FullName, imageSize, imageSize);
            }

            var sandboxIterator = new ImageIterator(Directory.GetFiles(sandbox.FullName).Select(f => new FileInfo(f)));

            using (var outputImage = new Bitmap(outputColumns * imageSize, outputRows * imageSize))
            {
                using (var outputGfx = Graphics.FromImage(outputImage))
                {
                    for (int x = 0; x < outputColumns; x++)
                    {
                        for (int y = 0; y < outputRows; y++)
                        {
                            var thumbnail = Image.FromFile(sandboxIterator.GetNext().FullName);
                            outputGfx.DrawImage(thumbnail, x * imageSize, y * imageSize);
                        }
                    }
                }

                var tempPng = Path.GetTempFileName() + ".png";
                outputImage.Save(tempPng, ImageFormat.Png);

                return new FileInfo(tempPng);
            }
        }

        protected static Image GetRandomImageFromDirectory(string dir)
        {
            var images = Directory.GetFiles(dir);
            var randomImage = images[Random.Next(0, images.Length - 1)];
            return Image.FromFile(randomImage);
        }

        /// <summary>
        /// Accredation: http://stackoverflow.com/a/23400751
        /// </summary>
        protected static void OrientImage(Image img)
        {
            if (Array.IndexOf(img.PropertyIdList, 274) > -1)
            {
                var orientation = (int)img.GetPropertyItem(274).Value[0];
                switch (orientation)
                {
                    case 1:
                        // No rotation required.
                        break;
                    case 2:
                        img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        break;
                    case 3:
                        img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case 4:
                        img.RotateFlip(RotateFlipType.Rotate180FlipX);
                        break;
                    case 5:
                        img.RotateFlip(RotateFlipType.Rotate90FlipX);
                        break;
                    case 6:
                        img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 7:
                        img.RotateFlip(RotateFlipType.Rotate270FlipX);
                        break;
                    case 8:
                        img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                }
                // This EXIF data is now invalid and should be removed.
                img.RemovePropertyItem(274);
            }
        }

        /// <summary>
        /// Accredation: http://stackoverflow.com/a/2001692
        /// </summary>
        protected static void ResizeImage(string imagePath, string outputDir, int canvasWidth, int canvasHeight)
        {
            Image image = Image.FromFile(imagePath);
            OrientImage(image);

            int originalWidth = image.Width;
            int originalHeight = image.Height;

            Image thumbnail =
                new Bitmap(canvasWidth, canvasHeight);
            System.Drawing.Graphics graphic =
                         System.Drawing.Graphics.FromImage(thumbnail);

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;

            /* ------------------ new code --------------- */

            // Figure out the ratio
            double ratioX = (double)canvasWidth / (double)originalWidth;
            double ratioY = (double)canvasHeight / (double)originalHeight;
            // use whichever multiplier is smaller
            double ratio = Math.Max(ratioX, ratioY);

            // now we can get the new height and width
            int newHeight = Convert.ToInt32(originalHeight * ratio);
            int newWidth = Convert.ToInt32(originalWidth * ratio);

            // Now calculate the X,Y position of the upper-left corner 
            // (one of these will always be zero)
            int posX = Convert.ToInt32((canvasWidth - (originalWidth * ratio)) / 2);
            int posY = Convert.ToInt32((canvasHeight - (originalHeight * ratio)) / 2);

            graphic.Clear(Color.White); // white padding
            graphic.DrawImage(image, posX, posY, newWidth, newHeight);

            /* ------------- end new code ---------------- */

            System.Drawing.Imaging.ImageCodecInfo[] info =
                             ImageCodecInfo.GetImageEncoders();
            EncoderParameters encoderParameters;
            encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality,
                             100L);

            thumbnail.Save(Path.Combine(outputDir, Path.GetFileName(imagePath)), info[1],
                             encoderParameters);
        }
    }
}
