using System;
using System.Collections.Generic;
using NetworkModule;

namespace MemoryModule
{
    public class Memory
    {
        public Guid ID;
        public float Weight;
        public float Staleness;
        public DateTime StartDateTime = new DateTime();
        public DateTime EndDateTime = new DateTime();
        public List<Frame> Frames = new List<Frame>();

        public Memory()
        {
            ID = Guid.NewGuid();
            Weight = 1;
            StartDateTime = DateTime.Now;
        }

        public void AddFrame(Frame newFrame)
        {
            Frame lastFrame = Frames[Frames.Count - 1];

            //Is the new frame 80%+ similar to the last frame?
            if (newFrame.Similar(lastFrame))
            {
                //If yes, then don't add the new frame and increase staleness
                //If frames are coming in at 1 per millisecond, this gives about 100ms of repeating data before starting new memory
                Staleness += 0.01f;
            }
            else
            {
                //If no, then add the new frame and reset staleness
                Frames.Add(newFrame);
                Staleness = 0;
            }

            //If reached max staleness, end memory
            if (Staleness >= 1)
            {
                EndDateTime = DateTime.Now;
            }
        }

        public bool HasData(DataPacket dataPacket)
        {
            int count = Frames.Count;
            for (int i = 0; i < count; i++)
            {
                Frame frame = Frames[i];
                if (frame.HasData(dataPacket))
                {
                    return true;
                }
            }

            return false;
        }

        public void Decay()
        {
            Weight /= 2;

            CryptoRandom random = new CryptoRandom();
            int choice = random.Next(0, Frames.Count);
            Frame frame = Frames[choice];

            frame.Decay();
            if (frame.Neurons.Count == 0)
            {
                Frames.Remove(frame);
            }
        }
    }
}
