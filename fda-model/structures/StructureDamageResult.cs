using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace structures
{
    public class StructureDamageResult
{
        private readonly double _StructureDamage;
        private readonly double _ContentDamage;
        private readonly double _OtherDamage;

        public double StructureDamage { get; set; }
        public double ContentDamage { get; set; }
        public double OtherDamage { get; set; }

        public StructureDamageResult(double structuredamage, double contentdamage, double otherdamage)
        {
            _StructureDamage = structuredamage;
            _ContentDamage = contentdamage;
            _OtherDamage = otherdamage;
        }
}
}
