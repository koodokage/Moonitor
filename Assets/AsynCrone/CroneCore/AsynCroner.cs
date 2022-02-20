using System;
using System.Net.Sockets;
using System.Threading;
using AsCrone.Module;
using AsCrone.Transmision;

namespace AsCrone
{
    public static class AsynCroner 
    {
        private static ResponseData remoteResponse;
        private static CronePerformer cronePerformer;

        public static bool rigidConnected;
        static bool callBacked = false;

        const string _ADRESS = "213.159.7.193";
        const int _SLOT = 3001;

        static TcpClient _client;
        static Thread threadReceiver;
        static byte[] dataBlock = new byte[2056];
        static byte[] sendingBuffer = null;

        public static ResponseData RemoteResponse { get => remoteResponse; }
        public static CronePerformer CronePerforms { get => cronePerformer; }

        public static void SetCronePerformer(CronePerformer value)
        {
            cronePerformer = value;
            OnStart();
        }

        static void OnStart()
        {
            _client = new TcpClient();
            callBacked = true;
            StartAsyncCrone();

            threadReceiver = new Thread(new ThreadStart(JOB_Charger));
            threadReceiver.IsBackground = true;
            threadReceiver.Start();

        }

        static void JOB_Charger()
        {
            while (true)
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
                    //ConsoleLog("Try Connection");
                    rigidConnected = false;
                }


                while (rigidConnected)
                {
                    Thread.Sleep(200);
                    if (IsBack())
                    {
                        if (cronePerformer.QueueCount())
                        {
                            sendingBuffer = cronePerformer.GetDataInQueue();
                        }
                        else
                            continue;


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
                            cronePerformer.ReturnDataInQueue(sendingBuffer);
                            break;
                        }
                    }


                }



            }
        }

        //void ConsoleLog(string message)
        //{
        //    Debug.Log($">_ [{message}]");
        //}

        static bool IsBack()
        {
            if (callBacked)
            {
                return true;
            }
            return false;
        }

        static void StartAsyncCrone()
        {
            try
            {
                _client.BeginConnect(_ADRESS, _SLOT, new AsyncCallback(AcceptingCallback), _client);
            }
            catch
            {
                //ConsoleLog($"Connection Breaked");
            }
        }

        static void AcceptingCallback(IAsyncResult asyncResult)
        {
            TcpClient Tclient = (TcpClient)asyncResult.AsyncState;
            Tclient.EndConnect(asyncResult);
            NetworkStream networkStream = Tclient.GetStream();
            networkStream.BeginRead(dataBlock, 0, dataBlock.Length, new AsyncCallback(ReceivingCallback), Tclient);
        }

        static void ReceivingCallback(IAsyncResult asyncResult)
        {
            TcpClient Tclient = (TcpClient)asyncResult.AsyncState;
            remoteResponse = BytobConverter<ResponseData>.ByteArrayToObject(dataBlock);
            NetworkStream networkStream = Tclient.GetStream();
            networkStream.BeginRead(dataBlock, 0, dataBlock.Length, new AsyncCallback(ReceivingCallback), Tclient);

            callBacked = true;
            cronePerformer.ExecuteResponseChain = remoteResponse;
        }

        public static void Dispose()
        {
            if (threadReceiver != null)
            {
                if (threadReceiver.IsAlive)
                {
                    threadReceiver.Abort();

                }
            }
            _client.Close();
        }
    }
}

