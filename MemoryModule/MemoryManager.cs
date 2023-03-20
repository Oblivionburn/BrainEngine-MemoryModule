using System.Collections.Generic;
using NetworkModule;

namespace MemoryModule
{
    public static class MemoryManager
    {
        public static List<Memory> Memories = new List<Memory>();

        public static void ReceiveData(List<Neuron> neurons)
        {
            //Add new frame of data to memory for later recollection
            Memory memory = CurrentMemory();
            memory.AddFrame(new Frame(neurons));

            //If memory has grown stale from receiving repeating data, then start a new memory
            if (memory.Staleness >= 1)
            {
                Memories.Add(new Memory());
            }
        }

        public static Memory CurrentMemory()
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
                        if (existing.Frames.Count == 0)
                        {
                            Memories.Remove(existing);
                        }
                    }
                }
            }

            return memory;
        }

        public static List<Memory> RecallMemories(List<DataPacket> dataPackets)
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
                    if (memory.Frames.Count == 0)
                    {
                        Memories.Remove(memory);
                    }
                }
            }

            return memories;
        }
    }
}
