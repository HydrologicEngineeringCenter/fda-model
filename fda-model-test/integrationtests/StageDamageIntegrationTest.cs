using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alternatives;
using compute;
using fda_model.hydraulics;
using fda_model.hydraulics.enums;
using fda_model.structures;
using metrics;
using paireddata;
using scenarios;
using stageDamage;
using Statistics;
using Statistics.Distributions;
using Statistics.Histograms;
using structures;
using Xunit;

namespace fda_model_test.integrationtests
{
    public class StageDamageIntegrationTest
    {
        [Fact]
        public void StageDamageShould()
        {
            //arrange: need structure inventory with occtypes, hydraulic modeling, flow-frequency function, stage-flow function, occtype, convergence criteria, impact area id 

            string pathToNSIShapefile = @"..\..\..\Resources\MuncieNSI\MuncieNSI.shp";
            string pathToIAShapefile = @"..\..\..\Resources\MuncieImpactAreas\ImpactAreas.shp";
            string pathToResult = @"..\..\..\Resources\MuncieResult\Muncie.p04.hdf";
            string pathToTerrain = @"..\..\..\Resources\MuncieTerrain\Terrain (1)_30ft_clip.hdf";
            
            StructureInventoryColumnMap map = new StructureInventoryColumnMap();
            //need to figure out how to include occtype in an inventory
            Inventory inventory = new Inventory(pathToNSIShapefile, pathToIAShapefile, map);

            HydraulicProfile profileOnePercent = new HydraulicProfile(.01, pathToResult, HydraulicDataSource.UnsteadyHDF, "Max", pathToTerrain);
            HydraulicProfile profileTenPercent = new HydraulicProfile(.1, pathToResult, HydraulicDataSource.UnsteadyHDF, "Max", pathToTerrain);
            List<HydraulicProfile> hydraulicDataSetList = new List<HydraulicProfile>() { profileOnePercent, profileTenPercent }; //etc 
            HydraulicDataset hydraulicDataset = new HydraulicDataset(hydraulicDataSetList);
            int stageImpactAreaID = 1;
            ImpactAreaStageDamage stageDamageObject = new ImpactAreaStageDamage(stageImpactAreaID, inventory, hydraulicDataset, )

            //act

                List<UncertainPairedData> stageDamageFunctions = stageDamageObject.Compute();

            //assert
        }
    }
}
