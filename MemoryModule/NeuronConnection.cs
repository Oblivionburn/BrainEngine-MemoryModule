using System;
using System.Collections.Generic;

namespace MemoryModule
{
    public class NeuronConnection
    {
        public Guid ID;
        public List<Guid> Neurons = new List<Guid>();

        public NeuronConnection()
        {
            ID = Guid.NewGuid();
        }
    }
}
