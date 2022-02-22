using System;

namespace AsCrone.Transmision
{
    public interface ITransmitorData
    {
        public TransmisionAction transmisionAction { get; set; }
        public string BSID { get; set; }
        public string CIP { get; set; }
        public string DeviceOS { get; set; }
        public bool Queued { get; set; }


    }
}