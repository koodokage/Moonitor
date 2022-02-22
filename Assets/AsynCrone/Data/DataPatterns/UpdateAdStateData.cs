using System;

namespace AsCrone.Transmision
{
    [Serializable]
    public struct UpdateAdStateData : ITransmitorData
    {

        public TransmisionAction transmisionAction { get; set; }
        public string BSID { get; set; }
        public string CIP { get; set; }
        public string GameCollectionName { get; set; } // GAMEID_vAPPREALEASEVERSION
        public string GameDbName { get; set; } // OS_COMPANYNAME
        public string DeviceOS { get; set; }
        public int Day { get; set; }

        //---------PAID INFO--------------//
        public int Interstitial_LoadStart { get; set; }
        public int Interstitial_NotReady { get; set; }
        public int Interstitial_Success { get; set; }
        public int Rewarded_LoadStart { get; set; }
        public int Rewarded_NotReady { get; set; }
        public int Rewarded_Success { get; set; }
        public int Banner_Success { get; set; }
        public bool Queued { get; set; }

        public UpdateAdStateData(TransmisionAction transmisionAction,ref DataLake dataBlock)
        {
            this.transmisionAction = transmisionAction;
            BSID = dataBlock.BSID;
            CIP = string.Empty;
            GameCollectionName = dataBlock.GameCollectionName;
            GameDbName = dataBlock.GameStockName;
            Day = dataBlock.Day;
            Rewarded_LoadStart = dataBlock.ADSRewarded_startedCount;
            Rewarded_LoadStart = dataBlock.ADSRewarded_startedCount;
            Rewarded_NotReady = dataBlock.ADSRewarded_NotReadyCount;
            Rewarded_Success = dataBlock.ADSRewarded_successCount;
            DeviceOS = dataBlock.DeviceOs;
            Interstitial_LoadStart = dataBlock.ADSInterstitial_startedCount;
            Interstitial_NotReady = dataBlock.ADSInterstitial_NotReadyCount;
            Interstitial_Success = dataBlock.ADSInterstitial_successCount;
            Banner_Success = dataBlock.ADSBanner_successCount;
            Queued = false;
        }
    }






}
