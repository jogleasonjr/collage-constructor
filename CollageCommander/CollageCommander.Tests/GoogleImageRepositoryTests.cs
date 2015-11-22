using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CollageCommander.Cli.Repositories;
using System.Drawing;

namespace CollageCommander.Tests
{
    [TestClass]
    public class GoogleImageRepositoryTests
    {
        [TestInitialize]

        [TestMethod]
        public void TestSimpleGet()
        {
            var googs = new GoogleImageRepository();
            int numPandas = 4;

            var pandas = googs.Get("panda", numPandas).Result;

            Assert.AreEqual(4, numPandas);

            foreach(var panda in pandas)
            {
                Assert.IsTrue(panda.Exists);

                //make sure we have valid images
                using (var bmp = Bitmap.FromFile(panda.FullName))
                {
                    
                }
            }
        }
    }
}
