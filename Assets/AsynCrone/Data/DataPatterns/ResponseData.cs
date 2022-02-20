using System;
using AsCrone;

namespace AsCrone.Transmision
{
    [Serializable]
    public struct ResponseData : ITransmitorData
    {
        public TransmisionAction transmisionAction { get; set; }
        public TunnelResponse response { get; set; }
        public string ATunnel_ID { get; set; }
        public string ATunnel_IP { get; set; }
        public string DeviceOS { get; set; }
        public bool Queued { get; set; }
        public string TestGroupe { get; set; }
    }

}