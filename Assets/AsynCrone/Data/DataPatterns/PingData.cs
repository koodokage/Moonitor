using System;

namespace AsCrone.Transmision
{
    [Serializable]
    public struct PingData : ITransmitorData
    {

        public TransmisionAction transmisionAction { get; set; }
        public string ATunnel_ID { get; set; }
        public string ATunnel_IP { get; set; }
        public string DeviceOS { get; set; }
        public bool Pong { get; set; }
        public bool Queued { get; set; }
        public PingData(TransmisionAction transmisionAction, string aTunnel_ID, string aTunnel_IP, bool pong)
        {
            this.transmisionAction = transmisionAction;
            ATunnel_ID = aTunnel_ID;
            ATunnel_IP = aTunnel_IP;
            DeviceOS = "Ping";
            Pong = pong;
            Queued = false;
        }
    }

}

