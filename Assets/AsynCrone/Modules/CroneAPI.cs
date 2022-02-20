using AsCrone.Module;
using AsCrone.Transmision;
namespace AsCrone
{
    public static class CroneAPI
    {
        private const int Clock_AdStateTick = 3;
        private const int Clock_RevenueTick = 2;
        private static int Tick_AdState = 0;
        private static int Tick_Revenue = 0;

        private static AsynCroneEvents asynCroneEventler;
        public static AsynCroneEvents CroneEnventHandler { get => asynCroneEventler; set => asynCroneEventler = value; }

        private static DataLake dataBlock;
        public static DataLake DataBlock { get => dataBlock; set => dataBlock = value; }

        public static void SetCompanyId(string companyId,bool inTestMode)
        {
            if (inTestMode)
            {
                companyId = "TESTMODE";
            }

            dataBlock.SetCompanyid(companyId);
        }


        public static void CallTimeTracker(StopWatchSubject watchSubject)
        {
            dataBlock.ExecuteAStopwatch(watchSubject);
        }



        public static void OnDataNotReady(AdsType type)
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

            Tick_AdState++;
            CheckTick(Tick_AdState, Clock_AdStateTick, TransmisionAction.UpdateAdStates);


        }

        public static void OnDataStarted(AdsType type)
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
            Tick_AdState++;
            CheckTick(Tick_AdState, Clock_AdStateTick, TransmisionAction.UpdateAdStates);


        }

        public static void OnDataSuccess(AdsType type)
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
            Tick_AdState++;
            CheckTick(Tick_AdState,Clock_AdStateTick,TransmisionAction.UpdateAdStates);
        }


        public static void OnDataRevene(double revenue)
        {
            dataBlock.Paid_Revenue = revenue;
            Tick_Revenue++;
            CheckTick(Tick_Revenue, Clock_RevenueTick,TransmisionAction.UpdateRevenue);
        }

        public static void BakedDataVpn()
        {
            if (dataBlock.VpnState == true)
            {
                dataBlock.ExecuteWebApiRequest();
            }
        }

        public static void CallLevelTracker(bool isMainLevel , int mainLevel , int subLevel)
        {
            CroneEnventHandler.OnLevelFinished += AsynCroner.CronePerforms.LevelPacked_InEventCallBack;

            if (isMainLevel)
            {
                dataBlock.MainLevel = mainLevel;
                dataBlock.Sub_level = -1;
                dataBlock.ExecuteAStopwatch(StopWatchSubject.MainLevel);
            }
            else
            {
                dataBlock.MainLevel = -1;
                dataBlock.Sub_level = subLevel;
                dataBlock.ExecuteAStopwatch(StopWatchSubject.SubLevel);
            }
        }

        public static void ExecuteLevelTracker()
        {
            asynCroneEventler.OnLevelFinish();

        }

        private static void CheckTick(int tick, int tickClock, TransmisionAction action)
        {
            if (tick > tickClock)
            {
                AsynCroner.CronePerforms.Packed_CallBack(action);
            }
        }

    }

}
