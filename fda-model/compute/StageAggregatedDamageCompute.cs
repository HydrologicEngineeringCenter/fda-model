using structures;
using System.Collections.Generic;
using System.Linq;
using RasMapperLib;
using fda_hydro.hydraulics;
using paireddata;
using System;

namespace fda_model.compute
{
    public class StageAggregatedDamageCompute
    {
        private Inventory _inventory;
        private HydraulicDataset _hydraulics;

        public StageAggregatedDamageCompute(Inventory inventory, HydraulicDataset hydraulics)
        {
            _inventory = inventory;
            _hydraulics = hydraulics;
        }

        private void Compute(int seed, int iterations)
        {
            Random randomNumberGenerator = new Random(seed);

            //get the XYs off the inventory 
            PointMs structurePoints = _inventory.GetPointMs();
            //Create StageDamageOrdinate 
            //Create IList of DamageResult

            //iterate across hydraulics datasets -- we want at each dataset - for a given impact area, for a given damage catagory,
            //for a given asset catagory, we need a distribution of damages
            foreach(HydraulicProfile set in _hydraulics.HydraulicProfiles)
            {
                //get depths for all structure in the inventory 
                float[] depths = set.GetHydraulicDataFromUnsteadyHDFs(structurePoints);

                //add uncertainty 
                for (int i = 0; i < iterations; i++)
                {
                    DeterministicInventory sampledInventory = _inventory.Sample(randomNumberGenerator.Next());

                    //compute an iteration of damage for all structures for this hydraulic set
                    for (int j = 0; j < depths.Count(); j++)
                    {
                        if (depths[j] != -9999)
                        {
                            StructureDamageResult results = sampledInventory.Inventory[j].ComputeDamage(depths[j]);
                            //add result to proper damage result
                        }
                        //this is the loop where we want to load up the histogram with damages by catagory, impact area
                    }
                }
            }
        }
        private PairedData GetStageAggDamCurves(IList<IList<StructureDamageResult>> structureDamageResults, PairedData indexPoint)
        {
            return null;
            //TODO: figure out how to aggregate to this bad boy
        }
    }
}
