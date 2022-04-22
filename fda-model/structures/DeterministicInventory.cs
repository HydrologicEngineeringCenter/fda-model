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
    }
}
