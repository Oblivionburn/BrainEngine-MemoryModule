using System;
using System.Collections.Generic;
using NetworkModule;

namespace MemoryModule
{
    public class Frame
    {
        public Guid ID;
        public float Weight;
        public DateTime DateTimeStamp = new DateTime();
        public List<Neuron> Neurons = new List<Neuron>();

        public Frame(List<Neuron> neurons)
        {
            ID = Guid.NewGuid();
            Weight = 1;
            DateTimeStamp = DateTime.Now;
            Neurons.AddRange(neurons);
        }

        public bool HasData(DataPacket dataPacket)
        {
            int count = Neurons.Count;
            for (int i = 0; i < count; i++)
            {
                Neuron existing = Neurons[i];
                if (Equals(existing.Data.Data, dataPacket.Data) &&
                    existing.Data.Type == dataPacket.Type)
                {
                    return true;
                }
            }

            return false;
        }

        public bool Similar(Frame frame)
        {
            int otherCount = frame.Neurons.Count;
            int count = Neurons.Count;

            float similar = 0;
            float total = count + otherCount;

            for (int i = 0; i < count; i++)
            {
                Neuron neuron = Neurons[i];

                for (int j = 0; j < otherCount; j++)
                {
                    Neuron oldNeuron = frame.Neurons[i];

                    if (Equals(oldNeuron.Data.Data, neuron.Data.Data) &&
                        oldNeuron.Data.Type == neuron.Data.Type)
                    {
                        similar++;
                    }
                }
            }

            float percent_similar = (similar * 100) / total;
            if (percent_similar >= 80)
            {
                return true;
            }

            return false;
        }

        public void Decay()
        {
            Weight /= 2;

            //If decayed to a millionth or less of original weight, drop a random neuron
            if (Weight <= 0.000001f)
            {
                CryptoRandom random = new CryptoRandom();
                int choice = random.Next(0, Neurons.Count);
                Neurons.RemoveAt(choice);

                //Reset weight so we're not dropping neurons too quickly
                Weight = 1;
            }
        }
    }
}
