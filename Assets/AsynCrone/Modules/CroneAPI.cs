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

        private static CronePerformer _cronePerformer;
        public static CronePerformer _CronePerformer { get => _cronePerformer; set => _cronePerformer = value; }

        public static void SetCompanyId(string companyId,bool inTestMode)
        {
            if (inTestMode || string.IsNullOrEmpty(companyId))
            {
                companyId = "TESTMODE";
            }

            _CronePerformer.SetCompanyId(companyId);
        }


        public static void CallTimeTracker(StopWatchSubject watchSubject)
        {
            _CronePerformer.ExecuteWatcher(watchSubject);
        }



        public static void OnDataNotReady(AdsType type)
        {

            _CronePerformer.SetAdStates_NotReady(type);
            Tick_AdState++;
            CheckTick(Tick_AdState, Clock_AdStateTick, TransmisionAction.UpdateAdStates);

        }

        public static void OnDataStarted(AdsType type)
        {
            _CronePerformer.SetAdStates_LoadStarted(type);

            Tick_AdState++;
            CheckTick(Tick_AdState, Clock_AdStateTick, TransmisionAction.UpdateAdStates);


        }

        public static void OnDataSuccess(AdsType type)
        {
            _CronePerformer.SetAdStates_Success(type);

            Tick_AdState++;
            CheckTick(Tick_AdState,Clock_AdStateTick,TransmisionAction.UpdateAdStates);
        }


        public static void OnDataRevene(double revenue)
        {
            _CronePerformer.SetDataRevenue(revenue);
            Tick_Revenue++;
            CheckTick(Tick_Revenue, Clock_RevenueTick,TransmisionAction.UpdateRevenue);
        }

        public static void CallLevelTracker(bool isMainLevel , int mainLevel , int subLevel)
        {
            CroneEnventHandler.OnLevelFinished += _CronePerformer.LevelPacked_InEventCallBack;

            if (isMainLevel)
            {
                _CronePerformer.SetMainLevelData(mainLevel);
            }
            else
            {
                _CronePerformer.SetMainLevelData(subLevel);
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
                _CronePerformer.Packed_CallBack(action);
            }
        }

    }

}
