using System;
using System.Collections.Generic;

namespace MemoryModule
{
    public class Neuron
    {
        public Guid ID;
        public float Weight;
        public List<Guid> NeuronConnections = new List<Guid>();
        public List<Guid> NetworkConnections = new List<Guid>();
        public DataPacket Data;

        public Neuron(Guid networkID, DataPacket data)
        {
            ID = Guid.NewGuid();
            Weight = 1;

            if (!NetworkConnections.Contains(networkID))
            {
                NetworkConnections.Add(networkID);
            }

            Data = data;
        }
    }
}
