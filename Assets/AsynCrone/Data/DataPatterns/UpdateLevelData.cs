using System;

namespace AsCrone.Transmision
{
    [Serializable]
    public struct UpdateLevelData : ITransmitorData
    {

        public TransmisionAction transmisionAction { get; set; }
        public string BSID { get; set; }
        public string CIP { get; set; }
        public string GameCollectionName { get; set; } // GAMEID_vAPPREALEASEVERSION
        public string GameDbName { get; set; } // OS_COMPANYNAME
        public string DeviceOS { get; set; }

        public int Day { get; set; }

        //---------PAID INFO--------------//
        public int MainLevel { get; set; }
        public double MainLevelPlayTime { get; set; }

        public int SubLevel { get; set; }
        public double SubLevelPlayTime { get; set; }
        public bool Queued { get; set; }

        public UpdateLevelData(TransmisionAction transmisionAction, ref DataLake dataBlock)
        {
            this.transmisionAction = transmisionAction;
            BSID = dataBlock.BSID;
            CIP = string.Empty;
            GameCollectionName = dataBlock.GameCollectionName;
            GameDbName = dataBlock.GameStockName;
            Day = dataBlock.Day;
            MainLevel = dataBlock.MainLevel;
            MainLevelPlayTime = dataBlock.MainLevelTime;
            SubLevel = dataBlock.Sub_level;
            SubLevelPlayTime = dataBlock.Sub_levelTime;
            DeviceOS = dataBlock.DeviceOs;
            Queued = false;
        }
    }






}
