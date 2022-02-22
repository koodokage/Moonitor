using System;
using AsCrone;

namespace AsCrone.Transmision
{
    [Serializable]
    public struct ResponseData : ITransmitorData
    {
        public TransmisionAction transmisionAction { get; set; }
        public TunnelResponse response { get; set; }
        public string BSID { get; set; }
        public string CIP { get; set; }
        public string DeviceOS { get; set; }
        public bool Queued { get; set; }
        public string TestGroupe { get; set; }
    }

}