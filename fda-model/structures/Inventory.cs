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
