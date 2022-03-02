using System.Diagnostics;
using AsCrone.Module;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace AsCrone.Transmision
{

//#if UNITY_EDITOR
//    [CustomEditor(typeof(CronePerformer), true), CanEditMultipleObjects]
//    public class CronePerformerEditor : Editor
//    {

//        public override void OnInspectorGUI()
//        {
//            base.OnInspectorGUI();
//            CronePerformer data = (CronePerformer)target;

//            if (GUILayout.Button("START LEVEL"))
//            {
//                data.SendUpdateRewarded();
//            }
//            if (GUILayout.Button("STOP LEVEL"))
//            {
//                data.Stop();
//            }
//        }

//    }
//#endif

    public class CronePerformer : MonoBehaviour
    {
        [Tooltip("Network Send-Response (ECHO) Speed")] public float NetworkSpeed_MS;
        [SerializeField] private AsynCroner croner;
        [SerializeField] DataLake dataBlock;
        private ResponseData remoteResponse;
        public byte[] regData; 
        private Stopwatch pingWatch;
        internal string PlayerID { get => dataBlock.BSID; set => dataBlock.BSID = value; }
        internal DataLake SetDataLake { set => dataBlock = value; }
        internal DataSlot selectedRoom { get => dataBlock.dataSlot; }
        internal DataCenter selectedCenter { get => dataBlock.dataCenter; }


        private void Initialization()
        {
            if (dataBlock == null)
            {
                Debug.LogError($"Warning : Null Data Block ! [Please Set A Data Block]");
                Application.Quit();
                return;
            }
            dataBlock.OnAwake();
            BuildRegisterData();
         
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            croner = new AsynCroner();
            croner.OnStart(this);
            CroneAPI._CronePerformer = this;
            Initialization();
        }

        private void OnDisable()
        {
            croner.Dispose();
        }

        public ResponseData ExecuteResponseChain
        {
            get { return remoteResponse; }
            set
            {
                remoteResponse = value;
                Pong();
                switch (remoteResponse.response)
                {
                    case TunnelResponse.Registered:
                        Response_OnRegistered();
                        break;
                    case TunnelResponse.QueueUp:
                        Response_OnQuery();
                        break;
                    case TunnelResponse.Success:
                        Response_OnSuccess();
                        break;
                    case TunnelResponse.TimeOut:
                        Response_OnTimeOut();
                        break;
                    case TunnelResponse.Unauthorized:
                        Response_Rejected();
                        break;
                    case TunnelResponse.Pong:
                        Response_OnPinged();
                        break;
                }

            }
        }


        private void Response_OnPinged()
        {
        }

        private void Response_OnRegistered()
        {
            if (dataBlock.IsNew)
            {

                PlayerID = remoteResponse.BSID;

                if (PlayerID == string.Empty)
                {
                    dataBlock.IsNew = true;
                    return;

                }
                dataBlock.IsNew = false;
                dataBlock.Saving();
            }

        }

        private void Response_OnSuccess()
        {
        }

        private void Response_OnTimeOut()
        {
            // TODO : Response Time Out Sequence
        }

        private void Response_Rejected()
        {
            // TODO : Ban Client ?? Daily Weekly &

        }

        private void Response_OnQuery()
        {

        }

        private void BytedDataToQueue(ITransmitorData requestData)
        {
            byte[] identifierBuffer = BytobConverter<ITransmitorData>.ObjectToByteArray(requestData);
            dataBlock.SaveInQueue(identifierBuffer);
        }
        public void Ping()
        {
            pingWatch = new Stopwatch();
            pingWatch.Start();
        }
        public void SetCompanyId(string companyId)
        {
            dataBlock.SetCompanyid(companyId);
        }
        public void ExecuteWatcher(StopWatchSubject subject)
        {
            dataBlock.ExecuteAStopwatch(subject);
        }
        public void SetAdStates_NotReady(AdsType type)
        {
            switch (type)
            {
                case AdsType.Interstial:
                    dataBlock.ADSInterstitial_NotReadyCount++;
                    break;
                case AdsType.Rewarded:
                    dataBlock.ADSRewarded_NotReadyCount++;
                    break;
            }
        }
        public void SetAdStates_LoadStarted(AdsType type)
        {
            switch (type)
            {
                case AdsType.Interstial:
                    dataBlock.ADSInterstitial_startedCount++;
                    break;
                case AdsType.Rewarded:
                    dataBlock.ADSRewarded_startedCount++;
                    break;
            }
        }
        public void SetAdStates_Success(AdsType type)
        {
            switch (type)
            {
                case AdsType.Interstial:
                    dataBlock.ADSInterstitial_successCount++;
                    break;
                case AdsType.Rewarded:
                    dataBlock.ADSRewarded_successCount++;
                    break;
                case AdsType.Banner:
                    dataBlock.ADSBanner_successCount++;
                    break;
            }
        }
        public void SetDataRevenue(double amount)
        {
            dataBlock.Paid_Revenue = amount;
        }
        public void SetMainLevelData(int mainLevel)
        {
            dataBlock.Sub_level = -1;
            dataBlock.MainLevel = mainLevel;
            ExecuteWatcher(StopWatchSubject.MainLevel);
        }
        public void SetSubLevelData(int subLevel)
        {
            dataBlock.MainLevel = -1;
            dataBlock.Sub_level = subLevel;
            ExecuteWatcher(StopWatchSubject.SubLevel);
        }
        public bool QueueCount()
        {
            if(dataBlock.requestQueue.Count > 0)
            {
                return true;
            }
            return false;

        }
        public bool RegisteredGame()
        {
            if (dataBlock.IsNew)
            {
                return true;
            }
            return false;

        }
        public byte[] GetDataInQueue()
        {
            return dataBlock.requestQueue.Dequeue();
        }
        public void ReturnDataInQueue(byte[] dataNotSended)
        {
            dataBlock.SaveInQueue_NotReachable(dataNotSended);
        }
        private void Pong()
        {
            if (pingWatch == null)
                return;

            NetworkSpeed_MS = pingWatch.ElapsedMilliseconds;
            pingWatch.Stop();
            pingWatch = null;
        }

        #region DATA PERFORMS
        private void PrepareToSend(TransmisionAction ttAction)
        {
            if (ttAction == TransmisionAction.Register)
                return;

            ITransmitorData dataOnSend = null;

            switch (ttAction)
            {
                case TransmisionAction.Ping:
                    dataOnSend = PerformPingPongAction();
                    break;
                case TransmisionAction.UpdateAdStates:
                case TransmisionAction.UpdateRevenue:
                case TransmisionAction.UpdateLevel:
                case TransmisionAction.UpdateSession:
                case TransmisionAction.UpdateVpn:
                    dataOnSend = PerformUpdateActions(ttAction);
                    break;
            }

            if(ttAction == TransmisionAction.Ping)
            {
                BytedDataToQueue(dataOnSend);
                return;
            }

            

            BytedDataToQueue(dataOnSend);

        }
        public void BuildRegisterData()
        {
            if (dataBlock.IsNew)
            {
                var dataOnRegSend = PerformRegisterAction();
                regData = BytobConverter<ITransmitorData>.ObjectToByteArray(dataOnRegSend);
                return;
            }
        }
        public byte[] PrepareToGetDataBytes(TransmisionAction ttAction)
        {
            ITransmitorData dataOnSend = null;

            switch (ttAction)
            {
                case TransmisionAction.Ping:
                    dataOnSend = PerformPingPongAction();
                    break;
                case TransmisionAction.Register:
                    dataOnSend = PerformRegisterAction();
                    break;
                case TransmisionAction.UpdateAdStates:
                case TransmisionAction.UpdateRevenue:
                case TransmisionAction.UpdateLevel:
                case TransmisionAction.UpdateSession:
                case TransmisionAction.UpdateVpn:
                    dataOnSend = PerformUpdateActions(ttAction);
                    break;
            }
            byte[] identifierBuffer = BytobConverter<ITransmitorData>.ObjectToByteArray(dataOnSend);
            return identifierBuffer;

        }
        private ITransmitorData PrepareToGetData(TransmisionAction ttAction)
        {
            ITransmitorData dataOnSend = null;

            switch (ttAction)
            {
                case TransmisionAction.Ping:
                    dataOnSend = PerformPingPongAction();
                    break;
                case TransmisionAction.Register:
                    dataOnSend = PerformRegisterAction();
                    break;
                case TransmisionAction.UpdateAdStates:
                case TransmisionAction.UpdateRevenue:
                case TransmisionAction.UpdateLevel:
                case TransmisionAction.UpdateSession:
                case TransmisionAction.UpdateVpn:
                    dataOnSend = PerformUpdateActions(ttAction);
                    break;
            }

            return dataOnSend;

        }

        private ITransmitorData PerformRegisterAction()
        {
            RegisterData request = new RegisterData(TransmisionAction.Register,PlayerID,string.Empty,ref dataBlock);
            return request;
        }

        private ITransmitorData PerformUpdateActions(TransmisionAction transmisionAction)
        {
            ITransmitorData updateData = null;
            switch (transmisionAction)
            {
                case TransmisionAction.UpdateAdStates:
                    updateData = new UpdateAdStateData(TransmisionAction.UpdateAdStates, ref dataBlock);
                    break;
                case TransmisionAction.UpdateRevenue:
                    updateData = new UpdateRevenueData(TransmisionAction.UpdateRevenue, ref dataBlock);
                    break;
                case TransmisionAction.UpdateLevel:
                    updateData = new UpdateLevelData(TransmisionAction.UpdateLevel, ref dataBlock);
                    break;
                case TransmisionAction.UpdateSession:
                    updateData = new UpdateSessionData(TransmisionAction.UpdateSession, ref dataBlock);
                    break;
                case TransmisionAction.UpdateVpn:
                    updateData = new UpdateVPNData(TransmisionAction.UpdateVpn, ref dataBlock);
                    break;
            }
            return updateData;
        }

        private ITransmitorData PerformPingPongAction()
        {
            PingData request =  new PingData(TransmisionAction.Ping,PlayerID , string.Empty , true);
            return request;
        }
        #endregion 

        internal void Packed_CallBack(TransmisionAction transmisionAction)
        {
            //ChekConnection
            if (croner.rigidConnected)
            {
                PrepareToSend(transmisionAction);
            }
            else
            {
                ITransmitorData readyToQueue = PrepareToGetData(transmisionAction);
                dataBlock.SaveInQueue_NotReachable(readyToQueue);
            }
        }

        internal void LevelPacked_InEventCallBack()
        {
            TransmisionAction action = TransmisionAction.UpdateLevel;

            if (croner.rigidConnected)
            {
                PrepareToSend(action);
            }
            else
            {
                ITransmitorData readyToQueue = PrepareToGetData(action);
                dataBlock.SaveInQueue_NotReachable(readyToQueue);
            }
        }

        //TEST
        internal void SendUpdateRewarded()
        {
            CroneAPI.CallLevelTracker(true,1,0);
        }
        internal void Stop()
        {
            CroneAPI.ExecuteLevelTracker();
        }
    }

}

