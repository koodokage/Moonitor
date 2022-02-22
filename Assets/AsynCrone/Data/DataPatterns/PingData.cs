using System;

namespace AsCrone.Transmision
{
    [Serializable]
    public struct PingData : ITransmitorData
    {

        public TransmisionAction transmisionAction { get; set; }
        public string BSID { get; set; }
        public string CIP { get; set; }
        public string DeviceOS { get; set; }
        public bool Pong { get; set; }
        public bool Queued { get; set; }
        public PingData(TransmisionAction transmisionAction, string aTunnel_ID, string aTunnel_IP, bool pong)
        {
            this.transmisionAction = transmisionAction;
            BSID = aTunnel_ID;
            CIP = aTunnel_IP;
            DeviceOS = "Ping";
            Pong = pong;
            Queued = false;
        }
    }

}

