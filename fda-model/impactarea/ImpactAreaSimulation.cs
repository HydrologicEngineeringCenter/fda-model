using System;
namespace impactarea
{
    public class ImpactAreaSimulation
    {
        public string Name { get; }
        public int ID { get; }
        public ead.Simulation Simulation { get; }
        public ImpactAreaSimulation(String name, ead.Simulation sim, int id){
            Name = name;
            Simulation = sim;
            ID = id;
        }
        public metrics.Results Compute(interfaces.IProvideRandomNumbers rp, Int64 iterations){
            return Simulation.Compute(rp,iterations);
        }

    }
}