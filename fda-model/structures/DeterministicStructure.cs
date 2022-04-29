namespace structures
{
    public class DeterministicStructure
    {
        private int _fdid;
        private double _foundationHeight;
        private double _StructureValue;
        private double _ContentValue;
        private double _OtherValue;
        private DeterministicOccupancyType _occtype;

        public DeterministicStructure(int name, double structValueSample, double foundHeightSample)
        {
            _fdid = name;
            _StructureValue = structValueSample;
            _foundationHeight = foundHeightSample;
        }

        public StructureDamageResult ComputeDamage(double depth)
        {
            double depthabovefoundHeight = depth - _foundationHeight;
            double structDamagepercent = _occtype.StructureDamageFunction.f(depthabovefoundHeight);
            double structDamage = structDamagepercent * _StructureValue;
            return new StructureDamageResult(structDamage,structDamage,structDamage);
        }
    }
}