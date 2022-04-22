namespace structures
{
    public class OccupancyType
    {
        private string name;
        private string damcat;
        private paireddata.UncertainPairedData _StructureDamageFunction;
        private paireddata.UncertainPairedData _ContentDamageFunction;
        private paireddata.UncertainPairedData _OtherDamageFunction;
        //other stuff.
        public paireddata.UncertainPairedData StructureDamageFunction
        {
            get { return _StructureDamageFunction; }
        }
    }
}