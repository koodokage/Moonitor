using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace AsCrone.Module
{
    public class IPnAPI : MonoBehaviour
    {
        public class Country
        {
            public string country;
            public bool hosting;
        }

        string countryName = string.Empty;
        bool chekVpn = false;
        DataLake dataBlock;

        public void RegisterBlock(DataLake dataBlock)
        {
            this.dataBlock = dataBlock;
            BEGIN_API();
        }

        private void BEGIN_API()
        {
            StartCoroutine(GET());

        }

        IEnumerator GET()
        {
            /*
             * CURRENT version
             *http://ip-api.com/json/?fields=country,hosting
             *VPN ip test
             *http://ip-api.com/json/138.68.66.154?fields=country,hosting,query
             * 
             */

            UnityWebRequest webRequest = UnityWebRequest.Get("http://ip-api.com/json/?fields=country,hosting");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                dataBlock.Country = string.Empty;
                Debug.Log("EMPTY");
                yield return new WaitForEndOfFrame();
                //Destroy(gameObject);
            }
            else if (webRequest.isDone || webRequest.result == UnityWebRequest.Result.Success)
            {
                Country format = JsonUtility.FromJson<Country>(webRequest.downloadHandler.text);

                Country countryData = new Country();
                countryData.country = format.country;
                countryData.hosting = format.hosting;

                countryName = countryData.country;
                chekVpn = countryData.hosting;

                dataBlock.Country = countryName;
                dataBlock.VpnState = chekVpn;

                yield return new WaitForEndOfFrame();
                //Destroy(gameObject);
            }

        }

    }
}


