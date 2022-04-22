using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace structures
{
    public class DeterministicInventory
{
        IList<DeterministicStructure> _inventory;

        public DeterministicInventory(IList<DeterministicStructure> inventorySample)
        {
            _inventory = inventorySample;
        }
        public IList<StructureDamageResult> ComputeDamages(IList<double> depths)
        {
            //assume each structure has a corresponding index to the depth
            List<StructureDamageResult> results = new List<StructureDamageResult>();
            StructureDamageResult nodamage = new StructureDamageResult(0, 0, 0);
            for(int i = 0; i < _inventory.Count; i++)
            {
                double depth = depths[i];
                if (depth != null)
                {
                    results.Add(_inventory[i].ComputeDamage(depth));
                }
                else
                {
                    results.Add(nodamage);
                }
                
            }
            return results;
        }
    }
}
