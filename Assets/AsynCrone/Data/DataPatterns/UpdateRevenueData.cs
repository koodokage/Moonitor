using System;

namespace AsCrone.Transmision
{
    [Serializable]
    public struct UpdateRevenueData:ITransmitorData
    {

        public TransmisionAction transmisionAction { get; set; }
        public string ATunnel_ID { get; set; }
        public string ATunnel_IP { get; set; }
        public string GameCollectionName { get; set; } // GAMEID_vAPPREALEASEVERSION
        public string GameDbName { get; set; } // OS_COMPANYNAME
        public string DeviceOS { get; set; }
        public int Day { get; set; }

        //---------PAID INFO--------------//
        public double DailyRevenue { get; set; }
        public double TotalRevenue{ get; set; }
        public bool Queued { get; set; }
        public UpdateRevenueData(TransmisionAction transmisionAction, ref DataLake dataBlock)
        {
            this.transmisionAction = transmisionAction;
            ATunnel_ID = dataBlock.PlayerGuid;
            ATunnel_IP = string.Empty;
            GameCollectionName = dataBlock.GameCollectionName;
            GameDbName = dataBlock.GameStockName;
            Day = dataBlock.Day;
            DailyRevenue = dataBlock.Paid_Revenue;
            TotalRevenue = dataBlock.TotalRevenue;
            DeviceOS = dataBlock.DeviceOs;
            Queued = false;
        }
    }






}
