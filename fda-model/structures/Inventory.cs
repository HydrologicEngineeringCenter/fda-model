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
            List<string> names = (List<string>)structureInventory.GetValuesFromColumn("fd_id").OfType<string>();
            List<double> x = (List<double>) structureInventory.GetValuesFromColumn("x").OfType<double>();
            List<double> y = (List<double>) structureInventory.GetValuesFromColumn("y").OfType<double>();
            List<double> foundheight = (List<double>) structureInventory.GetValuesFromColumn("found_ht").OfType<double>();
            List<double> valStructure = (List<double>) structureInventory.GetValuesFromColumn("val_cont").OfType<double>();
            List<double> valContent = (List<double>) structureInventory.GetValuesFromColumn("val_cont").OfType<double>();
            List<double> valVehic = (List<double>) structureInventory.GetValuesFromColumn("val_vehic").OfType<double>();
            List<string> damCat = (List<string>) structureInventory.GetValuesFromColumn("st_damcat").OfType<string>();
            List<string> occtype = (List<string>) structureInventory.GetValuesFromColumn("occtype").OfType<string>();
            List<int>    pop2amu65 = (List<int>) structureInventory.GetValuesFromColumn("pop2amu65").OfType<int>();
            List<int>    pop2amo65 = (List<int>) structureInventory.GetValuesFromColumn("pop2amo65").OfType<int>();
            List<int>    pop2pmu65 = (List<int>) structureInventory.GetValuesFromColumn("pop2pmu65").OfType<int>();
            List<int>    pop2pmo65 = (List<int>) structureInventory.GetValuesFromColumn("pop2pmo65").OfType<int>();
            _structures = new List<Structure>();

            for(int i = 0; i < names.Count(); i++)
            {
                _structures.Add(new Structure(names[i], x[i], y[i], foundheight[i], valStructure[i], valContent[i], valVehic[i], damCat[i], occtype[i], pop2amu65[i], pop2amo65[i], pop2pmu65[i], pop2pmo65[i]));
            }

            IEnumerable<Point> sIPoints = structureInventory.Points();
            foreach(string header in _columnsOfInterest)
            {

            }
            foreach (Point point in sIPoints)
            {
                //This is gonna rely on the NSI column headers. 
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
