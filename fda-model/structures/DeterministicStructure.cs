namespace structures
{
    public class DeterministicStructure
    {
        private string name;
        private double _foundationHeight;
        private double _StructureValue;
        private double _ContentValue;
        private double _OtherValue;
        private DeterministicOccupancyType _occtype;
        public double ComputeDamage(double depth)
        {
            double depthabovefoundHeight = depth - _foundationHeight;
            double structDamagepercent = _occtype.StructureDamageFunction.f(depthabovefoundHeight);

            return structDamagepercent * _StructureValue;
        }
    }
}