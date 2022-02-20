using System;

namespace AsCrone.Transmision
{
    public interface ITransmitorData
    {
        public TransmisionAction transmisionAction { get; set; }
        public string ATunnel_ID { get; set; }
        public string ATunnel_IP { get; set; }
        public string DeviceOS { get; set; }
        public bool Queued { get; set; }


    }
}