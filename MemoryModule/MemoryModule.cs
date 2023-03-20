using System;
using System.Collections.Generic;
using System.Linq;

namespace MemoryModule
{
    public class MemoryModule
    {
        public List<Memory> Memories = new List<Memory>();
        public List<Network> Networks = new List<Network>();
        public List<NeuronConnection> NeuronConnections = new List<NeuronConnection>();

        public MemoryModule()
        {
            
        }

        public void ReceiveData(List<DataPacket> dataPackets)
        {
            List<Neuron> neurons = new List<Neuron>();
            
            int dataCount = dataPackets.Count;
            for (int i = 0; i < dataCount; i++)
            {
                DataPacket packet = dataPackets[i];

                //Get network for this data type
                Network network = NetworkMatch(packet.Type);

                //Get new or existing neuron from network
                Neuron neuron = network.GetNeuron(packet);

                neurons.Add(neuron);
                network.AddNeuron(neuron);
            }

            int count = neurons.Count;
            if (count > 1)
            {
                //Check if these neurons are already all in a connection
                if (!ConnectionExists(neurons))
                {
                    //Associate neurons in this set with each other
                    NewConnection(neurons);
                }
            }

            //Add new frame of data to memory for later recollection
            Memory memory = CurrentMemory();
            memory.AddFrame(new Frame(neurons));

            //If memory has grown stale from receiving repeating data, then start a new memory
            if (memory.Staleness >= 1)
            {
                Memories.Add(new Memory());
            }
        }

        private Network NetworkMatch(Type type)
        {
            int count = Networks.Count;
            for (int i = 0; i < count; i++)
            {
                Network existing = Networks[i];
                if (existing.DataMatch(type))
                {
                    return existing;
                }
            }

            //If none found, start a new network for this data type
            Network network = new Network();
            Networks.Add(network);

            return network;
        }

        private bool ConnectionExists(List<Neuron> neurons)
        {
            List<NeuronConnection> connections = neurons[0].GetConnections(NeuronConnections);

            int connectionCount = connections.Count;
            for (int i = 0; i < connectionCount; i++)
            {
                NeuronConnection connection = connections[i];

                bool connected = true;

                int neuronCount = neurons.Count;
                for (int j = 0; j < neuronCount; j++)
                {
                    Neuron existing = neurons[j];
                    if (!connection.Neurons.Contains(existing.ID))
                    {
                        connected = false;
                        break;
                    }
                }

                if (connected)
                {
                    return true;
                }
            }

            return false;
        }

        private void NewConnection(List<Neuron> neurons)
        {
            NeuronConnection connection = new NeuronConnection();

            int count = neurons.Count;
            for (int i = 0; i < count; i++)
            {
                Neuron existing = neurons[i];
                existing.NeuronConnections.Add(connection.ID);
                connection.Neurons.Add(existing.ID);
            }

            NeuronConnections.Add(connection);
        }

        public Memory CurrentMemory()
        {
            Memory memory = null;
            if (Memories.Count == 0)
            {
                memory = new Memory();
                Memories.Add(memory);
            }
            else
            {
                int count = Memories.Count;
                for (int i = 0; i < count; i++)
                {
                    Memory existing = Memories[i];
                    if (i == count - 1)
                    {
                        //Return most recent memory
                        memory = existing;
                    }
                    else
                    {
                        //Fade old memories
                        existing.Decay();
                    }
                }
            }

            return memory;
        }

        public List<Memory> RecallMemories(List<DataPacket> dataPackets)
        {
            List<Memory> memories = new List<Memory>();

            float similar = 0;

            int dataCount = dataPackets.Count;
            int memCount = Memories.Count;

            for (int m = 0; m < memCount; m++)
            {
                Memory memory = Memories[m];

                for (int d = 0; d < dataCount; d++)
                {
                    DataPacket packet = dataPackets[d];
                    if (memory.HasData(packet))
                    {
                        similar++;
                    }
                }

                float percent_similar = (similar * 100) / dataCount;
                if (percent_similar >= 80)
                {
                    //Increase memory strength from recollection
                    memory.Weight++;

                    memories.Add(memory);
                }
                else
                {
                    //Decay old memories
                    memory.Decay();
                }
            }

            return memories;
        }
    }
}
