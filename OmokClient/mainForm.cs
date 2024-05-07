using Common;
using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows.Forms;

#pragma warning disable CA1416

namespace GameClient
{
    [SupportedOSPlatform("windows10.0.177630")]
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            PacketBuffer.Init((8096 * 10), PacketDef.PACKET_HEADER_SIZE, 2048);

            InitSocketNetwork();

            InitHttpNetwork();

            btnDisconnect.Enabled = false;

            InitPacketHandler();

            Omok_Init();
            DevLog.Write("프로그램 시작 !!!", LOG_LEVEL.INFO);
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseNetwork();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            SetDisconnectd();
        }

        void AddRoomUserList(string userID)
        {
            listBoxRoomUserList.Items.Add(userID);
        }

        void RemoveRoomUserList(string userID)
        {
            object removeItem = null;

            foreach (var user in listBoxRoomUserList.Items)
            {
                if ((string)user == userID)
                {
                    removeItem = user;
                    return;
                }
            }

            if (removeItem != null)
            {
                listBoxRoomUserList.Items.Remove(removeItem);
            }
        }

        string GetOtherPlayer(string myName)
        {
            if (listBoxRoomUserList.Items.Count != 2)
            {
                return null;
            }

            var firstName = (string)listBoxRoomUserList.Items[0];
            if (firstName == myName)
            {
                return firstName;
            }
            else
            {
                return (string)listBoxRoomUserList.Items[1];
            }
        }

        // 로그인 요청
        private async void button2_ClickAsync(object sender, EventArgs e)
        {
            bool isSuccess = await HiveLogin(textBoxUserID.Text, textBoxUserPW.Text);
            if (isSuccess == false)
            {
                return;
            }

            isSuccess = await ApiLogin(_userID, _authToken);
            if (isSuccess == false)
            {
                return;
            }

            ConnectGameServer();

            var loginReq = new CLoginReq();
            loginReq.UserID = _userID;
            loginReq.AuthToken = _authToken;

            PostSendPacket(PacketType.REQ_C_LOGIN, loginReq);            
            DevLog.Write($"로그인 요청:  {textBoxUserID.Text}, {textBoxUserPW.Text}");
        }

        private void btn_RoomEnter_Click(object sender, EventArgs e)
        {
            var roomEnterReq = new CRoomEnterReq();
            roomEnterReq.RoomNumber = Convert.ToInt32(textBoxRoomNumber.Text);

            PostSendPacket(PacketType.REQ_C_ROOM_ENTER, roomEnterReq);
            DevLog.Write($"방 입장 요청:  {textBoxRoomNumber.Text} 번");
        }

        private void btn_RoomLeave_Click(object sender, EventArgs e)
        {
            var roomLeaveReq = new CRoomLeaveReq();
            roomLeaveReq.RoomNumber = Convert.ToInt32(textBoxRoomNumber.Text);

            PostSendPacket(PacketType.REQ_C_ROOM_LEAVE, roomLeaveReq);
            DevLog.Write($"방 퇴장 요청:  {textBoxRoomNumber.Text} 번");

            _userList.Clear();
            listBoxRoomUserList.Items.Clear();
        }

        private void btnRoomChat_Click(object sender, EventArgs e)
        {
            if (textBoxRoomSendMsg.Text.IsEmpty())
            {
                MessageBox.Show("채팅 메시지를 입력하세요");
                return;
            }
            
            var roomChatReq = new CRoomChatReq();
            roomChatReq.Message = textBoxRoomSendMsg.Text;

            PostSendPacket(PacketType.REQ_C_ROOM_CHAT, roomChatReq);
            DevLog.Write($"방 채팅 요청");
        }

        private void btnMatching_Click(object sender, EventArgs e)
        {
            //PostSendPacket(PACKET_ID.MATCH_USER_REQ, null);
            DevLog.Write($"매칭 요청");
        }

        private void listBoxRoomChatMsg_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBoxRelay_TextChanged(object sender, EventArgs e)
        {

        }


        void SendPacketOmokPut(int x, int y)
        {
            var gamePutReq = new CGamePutReq
            {
                X = x,
                Y = y
            };

            PostSendPacket(PacketType.REQ_C_GAME_PUT, gamePutReq);
            DevLog.Write($"put stone 요청 : x  [ {x} ], y: [ {y} ] ");
        }

        private void btn_GameStartClick(object sender, EventArgs e)
        {
            var gameReadyReq = new CGameReadyReq
            {
                IsReady = true
            };

            PostSendPacket(PacketType.REQ_C_GAME_READY, gameReadyReq);
            StartGame(true, "My", "Other");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddUser("test1");
            AddUser("test2");
        }

        void AddUser(string userID)
        {

        }

        // 게임 시작 요청
        private void button3_Click(object sender, EventArgs e)
        {
            var gameReadyReq = new CGameReadyReq
            {
                IsReady = true
            };

            PostSendPacket(PacketType.REQ_C_GAME_READY, gameReadyReq);
            DevLog.Write($"게임 준비 완료 요청");
        }
    }
}
