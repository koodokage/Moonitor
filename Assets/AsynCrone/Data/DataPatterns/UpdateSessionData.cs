using System;

namespace AsCrone.Transmision
{
    [Serializable]
    public struct UpdateSessionData : ITransmitorData
    {

        public TransmisionAction transmisionAction { get; set; }
        public string BSID { get; set; }
        public string CIP { get; set; }
        public string GameCollectionName { get; set; } // GAMEID_vAPPREALEASEVERSION
        public string GameDbName { get; set; } // OS_COMPANYNAME
        public string DeviceOS { get; set; }
        public int Day { get; set; }

        //---------PAID INFO--------------//
        public int Session;
        public double TotalSessioned { get; set; }
        public int SessionPlayTime { get; set; }
        public double TotalPlayedTime { get; set; }
        public bool Queued { get; set; }
        public UpdateSessionData(TransmisionAction transmisionAction, ref DataLake dataBlock)
        {
            this.transmisionAction = transmisionAction;
            BSID = dataBlock.BSID ;
            CIP = string.Empty;
            GameCollectionName = dataBlock.GameCollectionName;
            GameDbName = dataBlock.GameStockName;
            Day = dataBlock.Day;
            Session = dataBlock.DailySessionCount;
            TotalSessioned = dataBlock.TotalSession;
            SessionPlayTime = dataBlock.SessionPlayTimme;
            TotalPlayedTime = dataBlock.TotalPlayTime;
            DeviceOS = dataBlock.DeviceOs;
            Queued = false;
        }
    }






}
