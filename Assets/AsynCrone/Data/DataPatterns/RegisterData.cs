using System;

namespace AsCrone.Transmision
{
    [Serializable]
    public struct RegisterData :ITransmitorData
    {

        public TransmisionAction transmisionAction { get; set; }
        public string ATunnel_ID { get; set; }
        public string ATunnel_IP { get; set; }
        //COMPANY BASED
        public string CompanyID { get; set; }
        public string GameID { get; set; }
        public string GameVersion { get; set; }
        //USER DEVICE
        public string DeviceOS { get; set; }
        public string OSVersion { get; set; }
        public string DeviceModel { get; set; }
        //USER INFO
        public string AppDate { get; set; }
        public string Country { get; set; }
        public string TestGroupe { get; set; }
        public bool VpnState { get; set; }
        public bool Queued { get; set; }
        public RegisterData(TransmisionAction _transmisionAction, string aTunnel_ID, string aTunnel_IP, ref DataLake dataBlock)
        {
            transmisionAction = _transmisionAction;
            ATunnel_ID = aTunnel_ID;
            ATunnel_IP = aTunnel_IP;
            CompanyID = dataBlock.CompanyId;
            GameID = dataBlock.GameId;
            GameVersion = dataBlock.AppReleaseVersion;
            DeviceOS = dataBlock.DeviceOs;
            OSVersion = dataBlock.DeviceOs;
            DeviceModel = dataBlock.DeviceModel;
            AppDate = dataBlock.AppDownloadedDate;
            Country = dataBlock.Country;
            TestGroupe = dataBlock.GroupOfTest;
            VpnState = dataBlock.VpnState;
            Queued = false;
        }
    }

}

