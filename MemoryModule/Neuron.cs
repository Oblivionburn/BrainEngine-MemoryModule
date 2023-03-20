using System;
using System.Collections.Generic;

namespace MemoryModule
{
    public class Neuron
    {
        public Guid ID;
        public Guid NetworkID;
        public List<Guid> NeuronConnections = new List<Guid>();
        public float Weight;
        public DataPacket Data;

        public Neuron(Guid networkID, DataPacket data)
        {
            ID = Guid.NewGuid();
            Weight = 1;
            NetworkID = networkID;
            Data = data;
        }

        public List<NeuronConnection> GetConnections(List<NeuronConnection> NeuronConnections)
        {
            List<NeuronConnection> connections = new List<NeuronConnection>();

            int count = NeuronConnections.Count;
            for (int i = 0; i < count; i++)
            {
                NeuronConnection connection = NeuronConnections[i];
                if (connection.Neurons.Contains(ID))
                {
                    connections.Add(connection);
                }
            }

            return connections;
        }
    }
}
