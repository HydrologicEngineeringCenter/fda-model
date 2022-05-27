using structures;
using System.Collections.Generic;
using RasMapperLib;
using fda_hydro.hydraulics;
using paireddata;
using System;
using fda_model.metrics;
using metrics;
using Statistics;
using Statistics.Distributions;

namespace fda_model.compute
{
    public class StageAggregatedDamageCompute
    {
        private Inventory _inventory;
        private HydraulicDataset _hydraulics;
        private int _analysisYear;
        private double _priceIndex;
        private LogPearson3 _flowFrequency;
        private UncertainPairedData _stageFlow;
        private GraphicalUncertainPairedData _graphicalFlowFrequency;
        private GraphicalUncertainPairedData _graphicalStageFrequency;
        private double LowestStage;
        private double HighestStage;
        private double LowDelta; //Difference between the lowest stage and .5
        private double HighestDelta; //Difference between the highest stage and .002


        public StageAggregatedDamageCompute(Inventory inventory, HydraulicDataset hydraulics, int analysisYear, double priceIndex, LogPearson3 flowFrequency,UncertainPairedData stageFlow)
        {
            _inventory = inventory;
            _hydraulics = hydraulics;
            _analysisYear = analysisYear;
            _priceIndex = priceIndex;
            _flowFrequency = flowFrequency;
            _stageFlow = stageFlow;

        }
        public StageAggregatedDamageCompute(Inventory inventory, HydraulicDataset hydraulics, int analysisYear, double priceIndex, GraphicalUncertainPairedData flowFrequency, UncertainPairedData stageFlow)
        {
            _inventory = inventory;
            _hydraulics = hydraulics;
            _analysisYear = analysisYear;
            _priceIndex = priceIndex;
            _graphicalFlowFrequency = flowFrequency;
            _stageFlow = stageFlow;

        }
        public StageAggregatedDamageCompute(Inventory inventory, HydraulicDataset hydraulics, int analysisYear, double priceIndex, GraphicalUncertainPairedData stageFrequency)
        {
            _inventory = inventory;
            _hydraulics = hydraulics;
            _analysisYear = analysisYear;
            _priceIndex = priceIndex;
            _graphicalStageFrequency = stageFrequency;
        }

        private void GetRangeOfFlows()
        {
            if (_graphicalStageFrequency.IsNull)
            {
                if (_flowFrequency.IsNull)
                {
                    //Graphical Flow
                    double lowestFlow = _graphicalFlowFrequency.StageOrLogFlowDistributions[0].InverseCDF(.0001);
                    double highestFlow = _graphicalFlowFrequency.StageOrLogFlowDistributions[_graphicalFlowFrequency.StageOrLogFlowDistributions.Length - 1].InverseCDF(.9999);
                    LowestStage = _stageFlow.SamplePairedData(.0001).f(lowestFlow); //TODO Verify f was the correct choice here not f inverse
                    HighestStage = _stageFlow.SamplePairedData(.9999).f(highestFlow);

                    IPairedData medianCurve = _graphicalFlowFrequency.SamplePairedData(.5);
                    double twoYearFlow = medianCurve.f(.5);
                    double fiveHundredYearFlow = medianCurve.f(.002);
                    double twoYearStage = _stageFlow.SamplePairedData(.5).f(twoYearFlow);
                    double fiveHundredYearStage = _stageFlow.SamplePairedData(.5).f(fiveHundredYearFlow);
                    LowDelta = twoYearStage - LowestStage;
                    HighestDelta = HighestStage - fiveHundredYearStage;
                }
                else
                {
                    //Analytical Flow
                    double lowestFlow = _flowFrequency.InverseCDF(.0001);
                    double highestFlow = _flowFrequency.InverseCDF(.9999);
                    LowestStage = _stageFlow.SamplePairedData(.0001).f(lowestFlow); //TODO Verify f was the correct choice here not f inverse
                    HighestStage = _stageFlow.SamplePairedData(.9999).f(highestFlow);

                    double twoYearFlow = _flowFrequency.InverseCDF(.5);
                    double fiveHundredYearFlow = _flowFrequency.InverseCDF(.998);
                    double twoYearStage = _stageFlow.SamplePairedData(.5).f(twoYearFlow);
                    double fiveHundredYearStage = _stageFlow.SamplePairedData(.5).f(fiveHundredYearFlow);
                    LowDelta = twoYearStage - LowestStage;
                    HighestDelta = HighestStage - fiveHundredYearStage;
                }
            }
            else
            {
                //Graphical Stage
                LowestStage = _graphicalStageFrequency.StageOrLogFlowDistributions[0].InverseCDF(.0001);
                HighestStage = _graphicalStageFrequency.StageOrLogFlowDistributions[_graphicalStageFrequency.StageOrLogFlowDistributions.Length-1].InverseCDF(.9999);

                IPairedData medianCurve = _graphicalStageFrequency.SamplePairedData(.5);
                double twoYearStage = medianCurve.f(.5);
                double fiveHundredYearStage = medianCurve.f(.002);
                LowDelta = twoYearStage - LowestStage;
                HighestDelta = HighestStage - fiveHundredYearStage;
            }
        }

        private void Compute(int seed, int iterations)
        {
            GetRangeOfFlows();

            //Get a an array of stages to compute
            int desiredOrdinates = 200;
            double[] stagesForCompute = new double[desiredOrdinates];
            stagesForCompute[0] = LowestStage;
            for (int i = 1; i < desiredOrdinates; i++)
            {
                stagesForCompute[i] = LowestStage + (HighestStage - LowestStage) /desiredOrdinates*i;
            }

            //get distance between the lowest stage and the .5 and the distance between the highest and the .002 
          

            Random randomNumberGenerator = new Random(seed);
            //get the XYs off the inventory 
            PointMs structurePoints = _inventory.GetPointMs();
            //StageDamageOrdinate stageDamageOrdinate = new StageDamageOrdinate();
            //Create StageDamageOrdinate 
            //Create IList of DamageResult

            //iterate across hydraulics datasets -- we want at each dataset - for a given impact area, for a given damage catagory,
            //for a given asset catagory, we need a distribution of damages
            foreach (HydraulicProfile set in _hydraulics.HydraulicProfiles)
            {
                //get depths for all structure in the inventory 
                float[] depths = set.GetDepths(structurePoints);
                Dictionary<string, ConsequenceResults> resultsDictionary = new Dictionary<string, ConsequenceResults>();
                
                //add uncertainty 
                for (int i = 0; i < iterations; i++)
                {
                    //resampling the variable properties on those structures 
                    //compute an iteration of damage for all structures for this hydraulic set
                    DeterministicInventory sampledInventory = _inventory.Sample(randomNumberGenerator.Next());
                    List<StructureDamageResult> damages = (List<StructureDamageResult>)sampledInventory.ComputeDamages(depths);
                    sortResultsIntoObjects(damages, ref resultsDictionary);
                }
               
            }
        }

        private void sortResultsIntoObjects(List<StructureDamageResult> results, ref Dictionary<string, ConsequenceResults> resultsDictionary )
        {
            //dictionary will store the ConsequenceResults objects for each impact area
            

            //for each structure 
            for(int i=0; i<_inventory.GetPointMs().Count; i++)
            {
                //create the individual consequence result objects
                Structure thisStructure = _inventory.Structures[i];
                ConsequenceResult structureResult = new ConsequenceResult(thisStructure.DamCatName, "StructureDamage", new Statistics.ConvergenceCriteria());
                ConsequenceResult contentResult =   new ConsequenceResult(thisStructure.DamCatName, "ContentDamage", new Statistics.ConvergenceCriteria());
                ConsequenceResult vehicleResult =   new ConsequenceResult(thisStructure.DamCatName, "VehicleDamage", new Statistics.ConvergenceCriteria());

                //add the data to them
                structureResult.AddConsequenceRealization(results[i].StructureDamage, 0);
                contentResult.AddConsequenceRealization(results[i].ContentDamage, 0);
                vehicleResult.AddConsequenceRealization(results[i].OtherDamage, 0);

                //if the dictionary doesn't already have an entry for that impact area
                if (!resultsDictionary.ContainsKey(thisStructure.ImpactAreaID))
                {
                    //add it
                    resultsDictionary.Add(thisStructure.ImpactAreaID, new ConsequenceResults(thisStructure.ImpactAreaID));
                    
                }
                //and fill it
                resultsDictionary[thisStructure.ImpactAreaID].AddConsequenceResult(structureResult);
                resultsDictionary[thisStructure.ImpactAreaID].AddConsequenceResult(contentResult);
                resultsDictionary[thisStructure.ImpactAreaID].AddConsequenceResult(vehicleResult);

            }
        }

    }
}
