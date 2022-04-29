using RasMapperLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace structures
{
    public class Inventory
{
        private List<Structure> _structures;
        private OccupancyTypeSet _Occtypes;

        //These guys are all the headers from the NSI I want to save in our structure objects
        //These specifically line up with the parameters for the structure constructor
        private String[] _columnsOfInterest = new String[] {"fd_id","x","y","found_ht","val_struct",
            "val_cont","val_vehic","st_damcat","occtype","pop2amu65","pop2amo65","pop2pmu65","pop2pm065"};
        
        public Inventory(string pointShapefilePath)
        {
            PointFeatureLayer structureInventory = new PointFeatureLayer("Structure_Inventory", pointShapefilePath);
            _structures = new List<Structure>();
            for(int i = 0; i < structureInventory.FeatureCount(); i++)
            {
                var row = structureInventory.FeatureRow(i);
                int fid =(int) row["fd_id"];
                double x = (double) row["x"];
                double y = (double) row["y"];
                double found_ht = (double)row["found_ht"];
                double val_struct = (double)row["val_struct"];
                double val_cont = (double)row["val_cont"];
                double val_vehic = (double)row["val_vehic"];
                string st_damcat = (string)row["st_damcat"];
                string occtype = (string)row["occtype"];
                int pop2amu65 = (int)row["pop2amu65"];
                int pop2amo65 = (int)row["pop2amo65"];
                int pop2pmu65 = (int)row["pop2pmu65"];
                int pop2pmo65 = (int)row["pop2pmo65"];

                _structures.Add(new Structure(fid, x, y, found_ht, val_struct, val_cont, val_vehic, st_damcat, occtype, pop2amu65, pop2amo65, pop2pmu65, pop2pmo65));
            }
        }
        public DeterministicInventory Sample(int seed)
        {
            Random random = new Random(seed);

            List<DeterministicOccupancyType> _OcctypesSample = _Occtypes.Sample(random.Next());
            List<DeterministicStructure> inventorySample = new List<DeterministicStructure>();
            foreach(Structure structure in _structures)
            {
                foreach (DeterministicOccupancyType deterministicOccupancyType in _OcctypesSample){
                    if (structure.DamCatName.Equals(deterministicOccupancyType.DamCatName))
                    {
                        if (structure.OccTypeName.Equals(deterministicOccupancyType.Name))
                        {
                            inventorySample.Add(structure.Sample(random.Next(), deterministicOccupancyType));
                            break;
                        }
                    }  
                }
                //it is possible that if an occupancy type doesnt exist a structure wont get added...
            }
            return new DeterministicInventory(inventorySample);
        }
}
}
