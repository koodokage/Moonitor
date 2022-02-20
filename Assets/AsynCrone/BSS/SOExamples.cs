using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BSS
{
    [CreateAssetMenu(menuName = "Exampler")]
    public class SOExamples : SO_Data
    {
        public int MyScore;
        [System.Serializable] struct StructData : IData { public int _score; };

        public override void SaveLocalData()
        {
            base.SaveLocalData();
            StructData sctData = new StructData();
            sctData._score = MyScore;
            bss.Save(sctData);
        }

        public override void LoadLocalData()
        {
            base.LoadLocalData();
            var conntiueToRead = bss.DataExist();
            if (!conntiueToRead) return;
            StructData sctData = new StructData();
            var commingData = (StructData)bss.Load(sctData);
            MyScore = commingData._score;
        }
    }
}

