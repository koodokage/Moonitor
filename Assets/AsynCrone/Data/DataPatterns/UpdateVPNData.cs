using System;

namespace AsCrone.Transmision
{
    [Serializable]
    public struct UpdateVPNData : ITransmitorData
    {
        public TransmisionAction transmisionAction { get; set; }
        public string ATunnel_ID { get; set; }
        public string ATunnel_IP { get; set; }
        public string GameCollectionName { get; set; } // GAMEID_vAPPREALEASEVERSION
        public string GameDbName { get; set; } // OS_COMPANYNAME
        public string DeviceOS { get; set; }

        //---------PAID INFO--------------//
        public string Country { get; set; }
        public bool isVpn { get; set; }
        public bool Queued { get; set; }
        public UpdateVPNData(TransmisionAction transmisionAction,ref DataLake dataBlock)
        {
            this.transmisionAction = transmisionAction;
            ATunnel_ID = dataBlock.PlayerGuid;
            ATunnel_IP = string.Empty;
            GameCollectionName = dataBlock.GameCollectionName;
            GameDbName = dataBlock.GameStockName;
            Country = dataBlock.Country;
            isVpn = dataBlock.VpnState;
            DeviceOS = dataBlock.DeviceOs;
            Queued = false;
        }
    }






}
