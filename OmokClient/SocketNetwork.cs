using System;
using System.Collections.Concurrent;
using System.Windows.Forms;
using Common;

namespace GameClient;

public partial class mainForm
{
    ClientSimpleTcp Network = new ClientSimpleTcp();
    System.Windows.Forms.Timer dispatcherUITimer = new();

    System.Threading.Thread NetworkReadThread = null;
    System.Threading.Thread NetworkSendThread = null;

    bool IsNetworkThreadRunning = false;
    bool IsBackGroundProcessRunning = false;

    PacketBufferManager PacketBuffer = new PacketBufferManager();
    ConcurrentQueue<byte[]> RecvPacketQueue = new ConcurrentQueue<byte[]>();
    ConcurrentQueue<byte[]> SendPacketQueue = new ConcurrentQueue<byte[]>();

    void InitSocketNetwork()
    {
        IsNetworkThreadRunning = true;
        NetworkReadThread = new System.Threading.Thread(this.NetworkReadProcess);
        NetworkReadThread.Start();
        NetworkSendThread = new System.Threading.Thread(this.NetworkSendProcess);
        NetworkSendThread.Start();

        IsBackGroundProcessRunning = true;
        dispatcherUITimer.Tick += new EventHandler(NetworkProcess);
        dispatcherUITimer.Interval = 100;
        dispatcherUITimer.Start();
    }

    void ConnectGameServer(string ip, int port)
    {
        if (Network.Connect(ip, port))
        {
            labelStatus.Text = string.Format("{0}. 서버에 접속 중", DateTime.Now);
            // btnDisconnect.Enabled = true;

            DevLog.Write($"서버에 접속 중", LOG_LEVEL.INFO);
        }
        else
        {
            labelStatus.Text = string.Format("{0}. 서버에 접속 실패", DateTime.Now);
        }

        PacketBuffer.Clear();
    }

    void CloseNetwork()
    {
        IsNetworkThreadRunning = false;
        Network.Close();

        NetworkReadThread.Join();
        NetworkSendThread.Join();

        IsBackGroundProcessRunning = false;
    }

    void NetworkReadProcess()
    {
        while (IsNetworkThreadRunning)
        {
            if (Network.IsConnected() == false)
            {
                System.Threading.Thread.Sleep(1);
                continue;
            }

            var recvData = Network.Receive();

            if (recvData != null)
            {
                System.Console.WriteLine($"받은 데이터: {recvData.Item1}");
                PacketBuffer.Write(recvData.Item2, 0, recvData.Item1);

                while (true)
                {
                    var data = PacketBuffer.Read();
                    if (data == null)
                    {
                        break;
                    }

                    RecvPacketQueue.Enqueue(data);
                }
                //DevLog.Write($"받은 데이터: {recvData.Item2}", LOG_LEVEL.INFO);
            }
            else
            {
                Network.Close();
                SetDisconnectd();
                DevLog.Write("서버와 접속 종료 !!!", LOG_LEVEL.INFO);
            }
        }
    }

    void NetworkSendProcess()
    {
        while (IsNetworkThreadRunning)
        {
            System.Threading.Thread.Sleep(1);

            if (Network.IsConnected() == false)
            {
                continue;
            }


            if (SendPacketQueue.TryDequeue(out var packet))
            {
                Network.Send(packet);
            }
        }
    }

    void NetworkProcess(object sender, EventArgs e)
    {
        ProcessLog();

        try
        {
            byte[] packet = null;

            if (RecvPacketQueue.TryDequeue(out packet))
            {
                ParsingPacket(packet);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(string.Format("NetworkProcess. error:{0}", ex.Message));
        }
    }

    public void SetDisconnectd()
    {
        // if (btnConnect.Enabled == false)
        // {
        //     btnConnect.Enabled = true;
        //     btnDisconnect.Enabled = false;
        // }

        while (true)
        {
            if (SendPacketQueue.TryDequeue(out var temp) == false)
            {
                break;
            }
        }

        listBoxRoomChatMsg.Items.Clear();
        listBoxRoomUserList.Items.Clear();

        EndGame();

        labelStatus.Text = "서버 접속이 끊어짐";

        Network.Close();
    }

    void PostSendPacket<T>(PacketType packetType, T packet) where T : IMessage
    {
        if (Network.IsConnected() == false)
        {
            DevLog.Write("서버 연결이 되어 있지 않습니다", LOG_LEVEL.ERROR);
            return;
        }

        byte[] bytes = PacketSerialized(packet, packetType);

        SendPacketQueue.Enqueue(bytes);
    }

    private void ProcessLog()
    {
        // 너무 이 작업만 할 수 없으므로 일정 작업 이상을 하면 일단 패스한다.
        int logWorkCount = 0;

        while (IsBackGroundProcessRunning)
        {
            System.Threading.Thread.Sleep(1);

            string msg;

            if (DevLog.GetLog(out msg))
            {
                ++logWorkCount;

                if (listBoxLog.Items.Count > 512)
                {
                    listBoxLog.Items.Clear();
                }

                listBoxLog.Items.Add(msg);
                listBoxLog.SelectedIndex = listBoxLog.Items.Count - 1;
            }
            else
            {
                break;
            }

            if (logWorkCount > 8)
            {
                break;
            }
        }
    }
}