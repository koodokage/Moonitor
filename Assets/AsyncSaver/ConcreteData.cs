using System.Collections.Generic;
using UnityEngine;

public static class ConcreteData
{
    [System.Serializable]
    public struct StructData
    {
        public int[] sourcePlacement;
        public bool isBuilded;
        public bool isGifted;
        public bool isRewarded;
        public bool inPurchase;
        public bool isOpen;
        public string meshName;
        public float portHeightY;
        public float scaleY;
        public bool bridgeData;
        public int priceData;
        public int sourceData;
        public int geoData;
        public int buildData;
        public int lockData;
        public int purchaseData;
        public int entityClass;
        public int rewardedSegment;
        public int rewardedAmount;
        public bool saveKeyData;
    }
    [System.Serializable]
    public struct StructDataMining
    {
        public int[] sourcePlacement;
      
    }

}
