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
        System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
        bool _isMatching = false;
        bool _isMatchingSuccess = false;

        public mainForm()
        {
            InitializeComponent();

            _timer.Tick += new EventHandler(SendCheckMachingReq);
            _timer.Interval = 1000;
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            PacketBuffer.Init((8096 * 10), PacketDef.PACKET_HEADER_SIZE, 2048);

            InitSocketNetwork();

            InitHttpNetwork();

            InitPacketHandler();

            Omok_Init();
            DevLog.Write("프로그램 시작 !!!", LOG_LEVEL.INFO);
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseNetwork();
        }

        private async void btnRegister_ClickAsync(object sender, EventArgs e)
        {
            string hiveUrl = textBoxHiveIP.Text;
            string email = textBoxHiveID.Text;
            string password = textBoxHivePW.Text;

            if (hiveUrl.IsEmpty() || email.IsEmpty() || password.IsEmpty())
            {
                MessageBox.Show("Hive 정보를 입력하세요");
                return;
            }

            DevLog.Write($"Hive 회원 가입 요청: {email}, {password}");

            await HiveRegister(hiveUrl, email, password);
        }

        private async void btnLogin_CLick(object sender, EventArgs e)
        {
            await HiveLogin();
            await ApiLogin();
        }

        private async void btnHiveLogin_Click(object sender, EventArgs e)
        {
            await HiveLogin();
        }

        private async void btnApiLogin_Click(object sender, EventArgs e)
        {
            await ApiLogin();
        }

        async Task HiveLogin()
        {
            string hiveUrl = textBoxHiveIP.Text;
            string email = textBoxHiveID.Text;
            string password = textBoxHivePW.Text;

            if (hiveUrl.IsEmpty() || email.IsEmpty() || password.IsEmpty())
            {
                MessageBox.Show("Hive 정보를 입력하세요");
                return;
            }

            DevLog.Write($"Hive 로그인 요청: {email}, {password}");

            bool isSuccess = await HiveLogin(hiveUrl, email, password);
            if (isSuccess == false)
            {
                DevLog.Write("Hive 로그인 요청 실패");
                return;
            }
        }

        async Task ApiLogin()
        {

            string apiServerUrl = textBoxApiIP.Text;
            string email = textBoxApiLoginID.Text;
            string password = textBoxApiLoginPW.Text;

            if (apiServerUrl.IsEmpty() || email.IsEmpty() || password.IsEmpty())
            {
                MessageBox.Show("API 정보를 입력하세요");
                return;
            }

            DevLog.Write($"API 로그인 요청: {email}, {password}");

            bool isSuccess = await ApiLogin(apiServerUrl, email, password);
            if (isSuccess == false)
            {
                DevLog.Write("API 로그인 요청 실패");
                return;
            }
        }

        private void btnSocketConnect_Click(object sender, EventArgs e)
        {
            string ip = textBoxSocketIP.Text;
            int port = Convert.ToInt32(textBoxSocketPort.Text);

            ConnectGameServer(ip, port);
        }

        private void btnSocketLogin_Click(object sender, EventArgs e)
        {
            string userID = textBoxSocketID.Text;
            string token = textBoxSocketToken.Text;

            if (userID.IsEmpty() || token.IsEmpty())
            {
                MessageBox.Show("로그인 정보를 입력하세요");
                return;
            }

            var loginReq = new CLoginReq();
            loginReq.UserID = userID;
            loginReq.AuthToken = token;

            PostSendPacket(PacketType.REQ_C_LOGIN, loginReq);
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

        private async void btn_Matching_Click(object sender, EventArgs e)
        {
            string apiServerUrl = textBoxApiIP.Text;
            string userID = textBoxSocketID.Text;
            string authToken = textBoxSocketToken.Text;

            if (userID.IsEmpty() || authToken.IsEmpty())
            {
                MessageBox.Show("로그인 먼저 진행해주세요");
                return;
            }

            var res = await ApiRequestMatch(apiServerUrl, userID, authToken);
            if (res == null)
            {
                DevLog.Write("매칭 실패");
                return;
            }

            if (res.ErrorCode != ErrorCode.NONE)
            {
                DevLog.Write("매칭 실패");
                return;
            }

            DevLog.Write("매칭 시작");
            _isMatching = true;

            _timer.Start();
        }

        private async void btn_MatchingCancle_Click(object sender, EventArgs e)
        {
            string apiServerUrl = textBoxApiIP.Text;
            string userID = textBoxSocketID.Text;
            string authToken = textBoxSocketToken.Text;

            if (userID.IsEmpty() || authToken.IsEmpty())
            {
                MessageBox.Show("로그인 먼저 진행해주세요");
                return;
            }

            var res = await ApiCancletMatch(apiServerUrl, userID, authToken);
            if (res == null)
            {
                DevLog.Write("매칭 취소 실패");
                return;
            }

            if (res.ErrorCode != ErrorCode.NONE)
            {
                DevLog.Write("매칭 취소 실패");
                return;
            }
            DevLog.Write("매칭 취소 성공");
            _isMatching = false;

            _timer.Stop();
        }

        private void listBoxRoomChatMsg_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBoxRelay_TextChanged(object sender, EventArgs e)
        {

        }

        async void SendCheckMachingReq(object sender, EventArgs e)
        {
            DevLog.Write("매칭 요청 중");

            if (_isMatching == false)
            {
                _timer.Stop();
                return;
            }

            if (_isMatchingSuccess)
            {
                _timer.Stop();
                return;
            }

            string apiServerUrl = textBoxApiIP.Text;
            string userID = textBoxSocketID.Text;
            string authToken = textBoxSocketToken.Text;

            if (userID.IsEmpty() || authToken.IsEmpty())
            {
                MessageBox.Show("로그인 먼저 진행해주세요");
                return;
            }

            var res = await ApiCheckMatch(apiServerUrl, userID, authToken);
            if (res == null)
            {
                DevLog.Write("매칭 중");
                return;
            }

            if (res.ErrorCode == ErrorCode.NONE)
            {
                DevLog.Write("매칭 성공");
                _isMatchingSuccess = true;

                textBoxSocketIP.Text = res.ServerAddress;
                textBoxSocketPort.Text = res.Port.ToString();
                textBoxRoomNumber.Text = res.RoomNumber.ToString();

                ConnectGameServer(res.ServerAddress, res.Port);
                _timer.Stop();
            }
            else
            {
                DevLog.Write("매칭 실패");
                _isMatchingSuccess = false;
                _timer.Start();
            }
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

        private void btn_GameReady_Click(object sender, EventArgs e)
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

        private void button_ApiLogin(object sender, EventArgs e)
        {
            // InitHttpNetwork(textBoxIP.Text);

            MessageBox.Show("IP 설정 완료");
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
