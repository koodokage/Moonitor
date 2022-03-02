using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsCrone.Module;
using AsCrone.Transmision;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace AsCrone
{
#if UNITY_EDITOR
    [CustomEditor(typeof(DataLake), true), CanEditMultipleObjects]
    public class DataLakeEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DataLake data = (DataLake)target;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (GUILayout.Button("Save Data"))
            {
                data.Saving();
            }
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (GUILayout.Button("Load Data"))
            {
                data.Loading();
            }
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (GUILayout.Button("Delete Data"))
            {
                data.Delete();
            }
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        }

    }
#endif
    public class DataLake : ScriptableObject, ISaveableScript
    {
        Dictionary<StopWatchSubject, GameObject> stopWatchs = new Dictionary<StopWatchSubject, GameObject>();
        internal Queue<byte[]> requestQueue = new Queue<byte[]>();

        public string FileName => "Dlake";
        [Header("Settings"),Space(5)]
        [Tooltip("Certificate issued by the Publisher")]public string CertificateID;
        [Tooltip("Data Center authorized by the Publisher")] public DataCenter dataCenter;
        [Tooltip("Server Machine authorized by the Publisher")] public DataSlot dataSlot;

        internal void OnAwake()
        {
            CroneAPI.SetCompanyId(CertificateID, true);
            sessionElapsedSec = 0;
            ExecuteActionManager();
            ExecuteAStopwatch(StopWatchSubject.Session);

            Loading();

            if (string.IsNullOrEmpty(Country))
                ExecuteWebApiRequest();

            if (IsNewGame)
            {
                var saltDate = string.Format("{0:dd-MM-yyyy}", DateTime.UtcNow);
                AppDownloadedDate = saltDate;
                AppDownloadedDate_Local = DateTime.Now.ToString();

                IsNewGame = false;
            }


            GetGameid();
            GetDeviceModel();
            GetReleaseVersion();
            GetOS_Mobile();

            DailySessionCount++;
            Debug.Log($"DAY : {Day}");

            Saving();
        }

        private void ExecuteActionManager()
        {
            GameObject eventManager = new GameObject("EH-API");
            eventManager.AddComponent<AsynCroneEvents>();
            CroneAPI.CroneEnventHandler = eventManager.GetComponent<AsynCroneEvents>();
        }

        public  void Reset()
        {
            Country = string.Empty;
            GroupOfTest = string.Empty;
            BSID = string.Empty;
            isNew = true;
            IsNewGame = true;
            GameId = string.Empty;
            CompanyId = string.Empty;
            DeviceOs = string.Empty;
            DeviceModel = string.Empty;
            OSVersion = string.Empty;
            AppReleaseVersion = string.Empty;
            AppDownloadedDate = string.Empty;
            AppDownloadedDate_Local = string.Empty;
            DailySessionCount = 0;
            Sub_level = 0;
            MainLevel = 0;
            MainLevelTime = 0;
            sub_levelTime = 0;
            sessionElapsedSec = 0;
            LastSavedDay = 0;
            TotalPlayTime = 0;
            TotalRevenue = 0;
            TotalSession = 0;
            Paid_Revenue = 0;
            ADSRewarded_NotReadyCount = 0;
            ADSRewarded_startedCount = 0;
            ADSRewarded_successCount = 0;
            ADSBanner_successCount = 0;
            ADSInterstitial_NotReadyCount = 0;
            ADSInterstitial_startedCount = 0;
            ADSInterstitial_successCount = 0;
        }

        internal void SaveQueueInLocal()
        {
            Saving();
        }
        [Serializable]
        public struct StructedData
        {
            public List<byte[]> userRequests;
            public bool userisNew;
            public bool appisNew;
            public string userID;
            public string userAppDownladedDate;
            public string userAppDownladedDate_Local;
            public string userCountry;
            public bool userUseVpn;
            public string userTestGroup;
            public double TotalSessionCount;
            public double TotalPlayedTimes;
            public double TotalRevenue;
            public int DaySessions;
            public int SaveLogDay;
        }
        public void Saving()
        {
            StructedData structed = new StructedData();

            if (LastSavedDay != Day)
            {
                LastSavedDay = Day;
                structed.SaveLogDay = LastSavedDay;
                DailySessionCount = 0;
            }

            structed.userRequests = requestQueue.ToList();

            structed.DaySessions = DailySessionCount;

            structed.userID = BSID;
            structed.userisNew = IsNew;
            structed.appisNew = IsNewGame;

            structed.userAppDownladedDate = AppDownloadedDate;
            structed.userAppDownladedDate_Local = AppDownloadedDate_Local;
            structed.userCountry = country; // use local in start is aldready pulled
            structed.userUseVpn = VpnState;
            structed.userTestGroup = GroupOfTest;

            structed.TotalPlayedTimes = TotalPlayTime;
            structed.TotalRevenue = TotalRevenue;
            structed.TotalSessionCount = TotalSession;
            AsyncJob.AsyncSave(structed,FileName);

        }
        public  void Loading()
        {
            if (!BinnarySaveSystem.DataExisted(FileName)) return;
            StructedData sctData = new StructedData();
            var commingData = AsyncJob.AsyncLoad(sctData,FileName);
            InitializeData(commingData);
        }
        public void Delete()
        {
            AsyncJob.AsyncDelete(FileName);
        }
        private void InitializeData(StructedData inCome)
        {
            if (lastSavedDay == Day)
            {
                DailySessionCount = inCome.DaySessions;
            }
            else
            {
                DailySessionCount = 0;
            }

            InsertListToQueue(inCome.userRequests);

            LastSavedDay = inCome.SaveLogDay;

            BSID = inCome.userID;
            IsNew = inCome.userisNew;
            IsNewGame = inCome.appisNew;

            AppDownloadedDate = inCome.userAppDownladedDate;
            AppDownloadedDate_Local = inCome.userAppDownladedDate_Local;
            Country = inCome.userCountry;
            VpnState = inCome.userUseVpn;
            GroupOfTest = inCome.userTestGroup;

            TotalRevenue = inCome.TotalRevenue;
            TotalPlayTime = inCome.TotalPlayedTimes;
            TotalSession = inCome.TotalSessionCount;
        }


        #region Private Fields

        [Tooltip("Collection Name")] private string gameCollectionName;
        [Tooltip("Target Name")] private string gameStockName;
        [Header("Serialized"),Space(5),SerializeField, Tooltip("Player Setting Product Name")] private string gameId;
        [SerializeField, Tooltip("Player Setting Company Name")] private string companyId;
        [SerializeField, Tooltip("OS Type")] private string deviceOS;
        [SerializeField, Tooltip("OS Version")] private string osVersion;
        [SerializeField, Tooltip("Device Manufacturer Identity")] private string deviceModel;
        [SerializeField, Tooltip("Player Setting Version")] private string appReleaseVersion;
        [SerializeField, Tooltip("Player Unique ID")] private string bsID;
        [SerializeField, Tooltip("Send Automatic Register Request")] private bool isNew = true;
        [SerializeField, Tooltip("Is New APP")] private bool isNewApp = true;
        [SerializeField, Tooltip("App Store Download Date")] private string appDownloadedDate;
        [SerializeField, Tooltip("App Store Download Date In Local Region")] private string appDownloadedDate_Local;
        [SerializeField, Tooltip("Country 2-Letters Code")] private string country;
        [SerializeField, Tooltip("DB Test Group")] private string groupOfTest;
        [SerializeField, Tooltip("Chek For Day Passed")] private int lastSavedDay;
        [SerializeField, Tooltip("Per Day Session Count")] private int dailySessionCount;
        [SerializeField, Tooltip("Per Day Session Elapsed Second")] private int sessionElapsedSec;
        [SerializeField, Tooltip("Vpn State")] private bool vpnState;

        [SerializeField, Tooltip("Level Pack")] private int level;
        [SerializeField, Tooltip("Level Pack")] private int sub_level;
        [SerializeField, Tooltip("Level Pack")] private double levelTime;
        [SerializeField, Tooltip("Level Pack")] private double sub_levelTime;

        [SerializeField, Tooltip("Daily Paid Revenue")] private double paidRevenue;
        [SerializeField, Tooltip("Total App Revenue")] private double totalRevenue;
        [SerializeField, Tooltip("Total Session Count")] private double totalSession;
        [SerializeField, Tooltip("Total Play Time In Second")] private double totalPlayTime;

        [SerializeField, Tooltip("Ads-NotReady(Rewarded)")] private int adsRewarded_notReadyCount;
        [SerializeField, Tooltip("Ads-Success(Rewarded)")] private int adsRewarded_successCount;
        [SerializeField, Tooltip("Ads-Started(Rewarded)")] private int adsRewarded_startedCount;

        [SerializeField, Tooltip("Ads-NotReady(Interstitial)")] private int adsInterstitial_notReadyCount;
        [SerializeField, Tooltip("Ads-Success(Interstitial)")] private int adsInterstitial_successCount;
        [SerializeField, Tooltip("Ads-Started(Interstitial)")] private int adsInterstitial_startedCount;

        [SerializeField, Tooltip("Ads-Success(Banner)")] private int adsBanner_successCount;

        #endregion

        #region Device Info

        internal string OSVersion
        {
            get
            {
                return osVersion;
            }
            private set => osVersion = value;
        }
        public string DeviceOs
        {
            get
            {
                return deviceOS;
            }
            private set { deviceOS = value; }
        }

        public string DeviceModel
        {
            get
            {

                return deviceModel;
            }
            private set { deviceModel = value; }

        }
        #endregion

        #region API-Info

        internal string Country
        {
            get
            {
                return country;
            }

            set
            {
                country = value;
            }
        }


        internal bool VpnState
        {
            get => vpnState;
            set
            {
                vpnState = value;
            }
        }

        #endregion

        #region AppData-Info

        internal string AppDownloadedDate_Local { get => appDownloadedDate_Local; set => appDownloadedDate_Local = value; }
        internal int Day { get => DayCounter.GetLocalDay(AppDownloadedDate_Local); }
        public int LastSavedDay { get => lastSavedDay; set => lastSavedDay = value; }
        internal int DailySessionCount
        {
            get => dailySessionCount; set
            {
                dailySessionCount = value;
                TotalSession += value;
            }
        }

        public int SessionPlayTimme
        {
            get => sessionElapsedSec; set
            {
                sessionElapsedSec = value;
                TotalPlayTime += value;
            }
        }

        internal bool IsNew { get { return isNew; } set { isNew = value; } }
        public bool IsNewGame { get => isNewApp; set => isNewApp = value; }

        #endregion

        #region Analytic-Info

        internal string BSID { get => bsID; set { bsID = value; } }
        internal string GroupOfTest { get => groupOfTest; set => groupOfTest = value; }
        internal string AppReleaseVersion
        {
            get { return appReleaseVersion; }
            private set { appReleaseVersion = value; }
        }
        internal string AppDownloadedDate { get { return appDownloadedDate; } private set { appDownloadedDate = value; } }
        #endregion

        #region Developer Info

        public string CompanyId
        {
            get
            {
                return companyId;
            }

            private set { companyId = value; }

        }



        public string GameId
        {
            get
            {
                return gameId;
            }
            private set { gameId = value; }

        }

        #endregion

        #region ADS-Info
        public double Paid_Revenue
        {
            get => paidRevenue; set
            {
                paidRevenue = value;
                TotalRevenue += value;
            }
        }
        public double TotalRevenue { get => totalRevenue; set => totalRevenue = value; }
        public double TotalSession { get => totalSession; set => totalSession = value; }
        public double TotalPlayTime { get => totalPlayTime; set => totalPlayTime = value; }
        public int ADSRewarded_NotReadyCount { get => adsRewarded_notReadyCount; set => adsRewarded_notReadyCount = value; }
        public int ADSInterstitial_NotReadyCount { get => adsInterstitial_notReadyCount; set => adsInterstitial_notReadyCount = value; }
        public int ADSRewarded_successCount { get => adsRewarded_successCount; set => adsRewarded_successCount = value; }
        public int ADSRewarded_startedCount { get => adsRewarded_startedCount; set => adsRewarded_startedCount = value; }
        public int ADSInterstitial_successCount { get => adsInterstitial_successCount; set => adsInterstitial_successCount = value; }
        public int ADSInterstitial_startedCount { get => adsInterstitial_startedCount; set => adsInterstitial_startedCount = value; }
        public int ADSBanner_successCount { get => adsBanner_successCount; set => adsBanner_successCount = value; }

        public int MainLevel { get => level; set => level = value; }
        public double MainLevelTime { get => levelTime; set => levelTime = value; }
        public double Sub_levelTime { get => sub_levelTime; set => sub_levelTime = value; }
        public int Sub_level { get => sub_level; set => sub_level = value; }

        public string GameCollectionName
        {
            get
            {
                gameCollectionName = $"{GameId.ToUpper()}_v{AppReleaseVersion}";
                return gameCollectionName;
            }
        }

        public string GameStockName
        {
            get
            {
                gameStockName = $"{DeviceOs.ToUpper()}_{CompanyId.ToUpper()}";
                return gameStockName;
            }
        }


        #endregion

        public async void InsertListToQueue(List<byte[]> requestList)
        {
            await ListToQueueAsync(requestList);
        }

        private Task ListToQueueAsync(List<byte[]> requestList)
        {
            return Task.Run(() =>
            {
                for (int i = requestList.Count; i < 0; i--)
                {
                    requestQueue.Enqueue(requestList[i]);
                }
            });
        }

        private void ExecuteWebApiRequest()
        {

            GameObject locater = new GameObject("IPN-WEB_API");
            locater.AddComponent<IPnAPI>();
            locater.GetComponent<IPnAPI>().RegisterBlock(this);
        }

        public void ExecuteAStopwatch(StopWatchSubject stopWatchSubject)
        {
            GameObject handler;
            if (stopWatchs.TryGetValue(stopWatchSubject, out handler))
            {
                handler.GetComponent<WatchAPI>().dataBlock = this;
                handler.GetComponent<WatchAPI>().subject = stopWatchSubject;
                handler.SetActive(true);
                return;
            }

            handler = new GameObject($"{stopWatchSubject}-API");
            handler.AddComponent<WatchAPI>();
            handler.GetComponent<WatchAPI>().dataBlock = this;
            handler.GetComponent<WatchAPI>().subject = stopWatchSubject;
            stopWatchs[stopWatchSubject] = handler;

        }

        private void GetReleaseVersion()
        {
            AppReleaseVersion = Application.version;
        }

        private void GetDeviceModel()
        {
            DeviceModel = SystemInfo.deviceModel;
        }

        public void SetCompanyid(string companyName)
        {
            var name = companyName;
            if (name == "Default Company" || name == string.Empty)
            {
                Debug.LogError("PLEASE SET YOUR COMPANY NAME");
                CompanyId = "$NotSet$";
                Application.Quit();
                return;
            }
            CompanyId = name.ToUpper();
            Debug.Log($"[COMPANY ID] {CompanyId}");
        }

        private void GetGameid()
        {
            var name = Application.productName;
            if (name == string.Empty)
            {
                Debug.LogError("PLEASE SET YOUR PRODUCT NAME");
                GameId = "$NotSet$";
            }
            GameId = name.ToUpper();
            Debug.Log($"[GAME ID] {gameId}");
        }

        protected void GetOS_Mobile()
        {
            var osInfo = SystemInfo.operatingSystem;
            string[] splitOs = osInfo.Split(' ');
            // MOBILE STRUCTURE
            DeviceOs = splitOs[0];

#if UNITY_IOS
            OSVersion = splitOs[1];
            Debug.Log($"OS Ver. {osVersion}");
#elif UNITY_ANDROID
            OSVersion = splitOs[2];
            Debug.Log($"OS Ver. {osVersion}");
#endif

        }

        public void SaveInQueue(byte[] request)
        {
            requestQueue.Enqueue(request);
        }

        internal void SaveInQueue_NotReachable(ITransmitorData request)
        {
            request.Queued = true;
            byte[] byteBuffer = BytobConverter<ITransmitorData>.ObjectToByteArray(request);
            requestQueue.Enqueue(byteBuffer);
            //Save Local Data
        }

        internal void SaveInQueue_NotReachable(byte[] request)
        {
            ITransmitorData data = BytobConverter<ITransmitorData>.ByteArrayToObject(request);
            data.Queued = true;
            byte[] byteBuffer = BytobConverter<ITransmitorData>.ObjectToByteArray(data);
            requestQueue.Enqueue(byteBuffer);
            //Save Local Data
        }

      
    }



}
