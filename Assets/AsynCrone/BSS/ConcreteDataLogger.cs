using System.Collections.Generic;
using UnityEngine;
namespace BSS
{
    public static class ConcreteDataLogger
    {

        public static void SaveData(string key, StructData data)
        {
            SavingProcess(key, data);
        }

        [System.Serializable]
        public struct StructData : IData
        {
            public string savedData;
        }


        private static void SavingProcess(string fileName, StructData data)
        {
            BinnarySaveSystem bss = new BinnarySaveSystem(fileName);
            StructData sctData = new StructData();
            sctData = data;
            bss.Save(sctData);
        }


        public static void LoadingProcess(string fileName, ref StructData data)
        {
            BinnarySaveSystem bss = new BinnarySaveSystem(fileName);
            var conntiuetoread = bss.DataExist();
            if (!conntiuetoread) return;
            StructData sctdata = new StructData();
            var commingdata = (StructData)bss.Load(sctdata);
            data = commingdata;
        }

        public static void DeleteDataProcess(string fileName)
        {
            BinnarySaveSystem.DeleteDatas(fileName);
        }

        public static void DeleteDataProcess(string[] fileName)
        {
            BinnarySaveSystem.DeleteDatas(fileName);
        }
    }
}

