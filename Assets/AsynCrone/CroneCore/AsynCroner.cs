using System;
using System.Net.Sockets;
using System.Threading;
using AsCrone.Module;
using AsCrone.Transmision;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace AsCrone
{
    [Serializable]
    public  class AsynCroner 
    {
        [SerializeField]private  ResponseData remoteResponse;
        private  CronePerformer cronePerformer;

        public  bool rigidConnected;
        public bool callBacked = false;
        public bool threadCanceled = false;

        const string _ADRESS = "213.159.7.193";
        const int _SLOT = 3001;

         TcpClient _client;
         Thread threadReceiver;
         byte[] dataBlock = new byte[2056];
         byte[] sendingBuffer = null;

        public  ResponseData RemoteResponse { get => remoteResponse; }
        public  CronePerformer CronePerforms { get => cronePerformer; }

         public void OnStart(CronePerformer _cronePerformer)
        {
            this.cronePerformer = _cronePerformer;

            callBacked = true;
            StartAsyncCrone();

            threadReceiver = new Thread(new ThreadStart(JOB_Charger));
            threadReceiver.IsBackground = true;
            threadReceiver.Start();

        }

        void JOB_Charger()
        {
            int connectionAttempt = 0;
            while (!threadCanceled)
            {
                try
                {
                    Thread.Sleep(2000);
                    if (_client.GetStream() != null)
                    {
                        rigidConnected = true;
                    }
                }
                catch
                {
                    StartAsyncCrone();
                    rigidConnected = false;
                }

                while (rigidConnected && !threadCanceled)
                {
                    Thread.Sleep(200);

                    if (IsBack())
                    {
                        if (cronePerformer.RegisteredGame() == true)
                        {
                            sendingBuffer = cronePerformer.regData;
                            connectionAttempt = 0;
                        }
                        else if (cronePerformer.QueueCount())
                        {
                            sendingBuffer = cronePerformer.GetDataInQueue();
                        }
                        else
                        {
                            continue;
                        }


                        try
                        {
                            NetworkStream networkStream = _client.GetStream();

                            if (networkStream.CanWrite)
                            {
                                networkStream.Write(sendingBuffer, 0, sendingBuffer.Length);
                                cronePerformer.Ping();
                                callBacked = false;

                            }
                        }
                        catch (Exception)
                        {
                            //ConsoleLog($"Send Phase Fail {ex.Message}");
                            if(sendingBuffer != null && cronePerformer.RegisteredGame())
                                cronePerformer.ReturnDataInQueue(sendingBuffer);
                            break;
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                        connectionAttempt++;
                        if(connectionAttempt >= 5)
                        {
                            rigidConnected = false;
                            callBacked = true;
                            _client.Close();
                        }

                    }


                }



            }
        }

        //void ConsoleLog(string message)
        //{
        //    Debug.Log($">_ [{message}]");
        //}

        bool IsBack()
        {
            if (callBacked)
            {
                return true;
            }
            return false;
        }

        void StartAsyncCrone()
        {
            _client = new TcpClient();
                _client.BeginConnect(_ADRESS, _SLOT, new AsyncCallback(AcceptingCallback), _client);
        }

        void AcceptingCallback(IAsyncResult asyncResult)
        {
            TcpClient Tclient = (TcpClient)asyncResult.AsyncState;
            Tclient.EndConnect(asyncResult);
            NetworkStream networkStream = Tclient.GetStream();
            networkStream.BeginRead(dataBlock, 0, dataBlock.Length, new AsyncCallback(ReceivingCallback), Tclient);
        }

        void ReceivingCallback(IAsyncResult asyncResult)
        {
            TcpClient Tclient = (TcpClient)asyncResult.AsyncState;
            remoteResponse = BytobConverter<ResponseData>.ByteArrayToObject(dataBlock);
            NetworkStream networkStream = Tclient.GetStream();
            networkStream.BeginRead(dataBlock, 0, dataBlock.Length, new AsyncCallback(ReceivingCallback), Tclient);
            Debug.Log($"READ : {remoteResponse.response}");
            callBacked = true;
            cronePerformer.ExecuteResponseChain = remoteResponse;
        }

        public void Dispose()
        {
            _client.Close();

            if (threadReceiver != null)
            {
                if (threadReceiver.IsAlive)
                {
                    threadCanceled = true;
                }
            }
        }
    }
}

