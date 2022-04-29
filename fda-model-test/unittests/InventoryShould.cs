using Xunit;
using structures;
using System.IO;
using System;
using Geospatial.GDALAssist;

namespace fda_model_test.unittests
{
    public class InventoryShould
    {
        static InventoryShould()
        {
            string gdalPath = "C:\\Programs\\6.x Development\\GDAL\\";
            if (!Directory.Exists(gdalPath))
            {
                Console.WriteLine("GDAL directory not found: " + gdalPath);
                return;
            }
            GDALSetup.InitializeMultiplatform(gdalPath);
        }
        [Fact]
        public void loadAStructureInventoryFromShapefile()
        {
            var SI = new Inventory("C:\\TestData\\NSI\\StructureInventory.shp");

        }
    }
}
