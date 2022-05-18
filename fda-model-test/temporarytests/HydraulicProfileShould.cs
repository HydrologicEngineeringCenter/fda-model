using Xunit;
using fda_hydro.hydraulics;
using System.Collections.Generic;
using System;
using System.IO;
using Geospatial.GDALAssist;
using structures;

namespace fda_model_test_unittests
{
    /// <summary>
    ///Test data needs to all be in the same projection. This test requirest the quick start guide dataset be saved in the locations in the strings. This test should not be run remotely, and can eventually be deleted. It exists to help test and validate the design
    /// of the aggregated stage damage compute.
    /// </summary>
    public class HydraulicProfileShould
    {

        static string terrainFile = "C:\\TestData\\Terrain\\Terrain.hdf";
        static string SIFile = "C:\\TestData\\NSI\\StructureInventory.shp";
        static string impactAreaFile = "C:\\TestData\\ImpactAreas\\ImpactAreas.shp";

        static string[] resultsFiles = new string[]
         {
                "C:\\TestData\\Native Output Files\\Muncie.p08.hdf",
                "C:\\TestData\\Native Output Files\\Muncie.p07.hdf",
                "C:\\TestData\\Native Output Files\\Muncie.p06.hdf",
                "C:\\TestData\\Native Output Files\\Muncie.p05.hdf",
                "C:\\TestData\\Native Output Files\\Muncie.p04.hdf",
                "C:\\TestData\\Native Output Files\\Muncie.p03.hdf",
                "C:\\TestData\\Native Output Files\\Muncie.p02.hdf",
                "C:\\TestData\\Native Output Files\\Muncie.p01.hdf" 
        };

        static double[] probabilities = new double[] { .002, .005, .01, .05, .1, .2, .5, .9 };


        static HydraulicProfileShould()
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
        public void sampleSomeValues()
        {
            List<HydraulicProfile> profiles = new List<HydraulicProfile>();
            for(int i = 0; i < resultsFiles.Length; i++)
            {
             profiles.Add(new HydraulicProfile(probabilities[i],resultsFiles[i],terrainFile));
            }
            HydraulicDataset dataset = new HydraulicDataset(profiles);
            Inventory inventory = new Inventory(SIFile,impactAreaFile);
            float[] depths = dataset.HydraulicProfiles[0].GetHydraulicDataFromUnsteadyHDFs(inventory.GetPointMs());
            Console.WriteLine("we did it!");
        }
    }

}



