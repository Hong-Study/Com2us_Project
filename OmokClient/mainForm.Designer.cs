namespace GameClient
{
    partial class mainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        const int defaultY = 50;
        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnHiveLogin = new System.Windows.Forms.Button();
            this.btnRegister = new System.Windows.Forms.Button();
            this.btnApiLogin = new System.Windows.Forms.Button();

            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBoxHivePW = new System.Windows.Forms.TextBox();
            this.textBoxApiLoginID = new System.Windows.Forms.TextBox();
            this.textBoxApiLoginPW = new System.Windows.Forms.TextBox();
            this.labelApiLoginID = new System.Windows.Forms.Label();
            this.labelApiLoginPW = new System.Windows.Forms.Label();

            this.textBoxHiveIP = new System.Windows.Forms.TextBox();
            this.labelHiveIP = new System.Windows.Forms.Label();
            this.textBoxApiIP = new System.Windows.Forms.TextBox();
            this.labelApiIP = new System.Windows.Forms.Label();

            this.textBoxSocketIP = new System.Windows.Forms.TextBox();
            this.labelSocketIP = new System.Windows.Forms.Label();
            this.textBoxSocketPort = new System.Windows.Forms.TextBox();
            this.labelSocketPort = new System.Windows.Forms.Label();
            this.btnSocketConnect = new System.Windows.Forms.Button();

            this.labelHivePW = new System.Windows.Forms.Label();
            this.textBoxHiveID = new System.Windows.Forms.TextBox();
            this.labelHiveID = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.listBoxLog = new System.Windows.Forms.ListBox();

            this.labelSocketID = new System.Windows.Forms.Label();
            this.textBoxSocketID = new System.Windows.Forms.TextBox();
            this.textBoxSocketToken = new System.Windows.Forms.TextBox();
            this.labelSocketToken = new System.Windows.Forms.Label();
            this.btnSocketLogin = new System.Windows.Forms.Button();

            this.Room = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.btnMatching = new System.Windows.Forms.Button();
            this.GameStartBtn = new System.Windows.Forms.Button();
            this.btnRoomChat = new System.Windows.Forms.Button();
            this.textBoxRoomSendMsg = new System.Windows.Forms.TextBox();
            this.listBoxRoomChatMsg = new System.Windows.Forms.ListBox();
            this.listBoxRoomUserList = new System.Windows.Forms.ListBox();
            this.btn_RoomLeave = new System.Windows.Forms.Button();
            this.btn_RoomEnter = new System.Windows.Forms.Button();
            this.textBoxRoomNumber = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox5.SuspendLayout();
            this.Room.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRegister
            // 
            this.btnRegister.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRegister.Location = new System.Drawing.Point(420, 20);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(88, 26);
            this.btnRegister.TabIndex = 28;
            this.btnRegister.Text = "회원가입";
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_ClickAsync);
            // 
            // btnHiveLogin
            // 
            this.btnHiveLogin.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnHiveLogin.Location = new System.Drawing.Point(421, 50);
            this.btnHiveLogin.Name = "btnHiveLogin";
            this.btnHiveLogin.Size = new System.Drawing.Size(88, 26);
            this.btnHiveLogin.TabIndex = 29;
            this.btnHiveLogin.Text = "Hive 로그인";
            this.btnHiveLogin.UseVisualStyleBackColor = true;
            this.btnHiveLogin.Click += new System.EventHandler(this.btnHiveLogin_Click);
            // 
            // btnApiLogin
            // 
            this.btnApiLogin.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnApiLogin.Location = new System.Drawing.Point(421, 80);
            this.btnApiLogin.Name = "btnApiLogin";
            this.btnApiLogin.Size = new System.Drawing.Size(88, 26);
            this.btnApiLogin.TabIndex = 29;
            this.btnApiLogin.Text = "Api 로그인";
            this.btnApiLogin.UseVisualStyleBackColor = true;
            this.btnApiLogin.Click += new System.EventHandler(this.btnApiLogin_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textBoxHiveIP);
            this.groupBox5.Controls.Add(this.labelHiveIP);
            this.groupBox5.Controls.Add(this.textBoxApiIP);
            this.groupBox5.Controls.Add(this.labelApiIP);
            this.groupBox5.Controls.Add(this.textBoxHivePW);
            this.groupBox5.Controls.Add(this.labelHivePW);
            this.groupBox5.Controls.Add(this.textBoxHiveID);
            this.groupBox5.Controls.Add(this.labelHiveID);
            this.groupBox5.Controls.Add(this.textBoxApiLoginID);
            this.groupBox5.Controls.Add(this.textBoxApiLoginPW);
            this.groupBox5.Controls.Add(this.labelApiLoginID);
            this.groupBox5.Controls.Add(this.labelApiLoginPW);
            this.groupBox5.Location = new System.Drawing.Point(12, 5);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(403, 110);
            this.groupBox5.TabIndex = 27;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "API Test";
            // 
            // textBoxHiveIP
            // 
            this.textBoxHiveIP.Location = new System.Drawing.Point(55, 23);
            this.textBoxHiveIP.MaxLength = 50;
            this.textBoxHiveIP.Name = "textBoxHiveIP";
            this.textBoxHiveIP.Size = new System.Drawing.Size(140, 21);
            this.textBoxHiveIP.TabIndex = 18;
            this.textBoxHiveIP.Text = "http://127.0.0.1:5241";
            this.textBoxHiveIP.WordWrap = false;
            // 
            // labelHiveIP
            // 
            this.labelHiveIP.AutoSize = true;
            this.labelHiveIP.Location = new System.Drawing.Point(10, 26);
            this.labelHiveIP.Name = "labelHiveIP";
            this.labelHiveIP.Size = new System.Drawing.Size(61, 12);
            this.labelHiveIP.TabIndex = 17;
            this.labelHiveIP.Text = "Hive IP";
            // 
            // textBoxApiIP
            // 
            this.textBoxApiIP.Location = new System.Drawing.Point(55, 60);
            this.textBoxApiIP.MaxLength = 50;
            this.textBoxApiIP.Name = "textBoxApiIP";
            this.textBoxApiIP.Size = new System.Drawing.Size(140, 21);
            this.textBoxApiIP.TabIndex = 18;
            this.textBoxApiIP.Text = "http://127.0.0.1:5122";
            this.textBoxApiIP.WordWrap = false;
            // 
            // labelApiIP
            // 
            this.labelApiIP.AutoSize = true;
            this.labelApiIP.Location = new System.Drawing.Point(10, 63);
            this.labelApiIP.Name = "labelApiIP";
            this.labelApiIP.Size = new System.Drawing.Size(61, 12);
            this.labelApiIP.TabIndex = 17;
            this.labelApiIP.Text = "API IP";
            // 
            // textBoxHivePW
            // 
            this.textBoxHivePW.Location = new System.Drawing.Point(250, 38);
            this.textBoxHivePW.MaxLength = 30;
            this.textBoxHivePW.Name = "textBoxHivePW";
            this.textBoxHivePW.Size = new System.Drawing.Size(120, 21);
            this.textBoxHivePW.TabIndex = 18;
            this.textBoxHivePW.Text = "test1234!@";
            this.textBoxHivePW.WordWrap = false;
            // 
            // labelHivePW
            // 
            this.labelHivePW.AutoSize = true;
            this.labelHivePW.Location = new System.Drawing.Point(200, 40);
            this.labelHivePW.Name = "labelHivePW";
            this.labelHivePW.Size = new System.Drawing.Size(61, 12);
            this.labelHivePW.TabIndex = 17;
            this.labelHivePW.Text = "Hive PW";
            // 
            // textBoxHiveID
            // 
            this.textBoxHiveID.Location = new System.Drawing.Point(250, 18);
            this.textBoxHiveID.MaxLength = 30;
            this.textBoxHiveID.Name = "textBoxHiveID";
            this.textBoxHiveID.Size = new System.Drawing.Size(120, 21);
            this.textBoxHiveID.TabIndex = 11;
            this.textBoxHiveID.Text = "test12@naver.com";
            this.textBoxHiveID.WordWrap = false;
            // 
            // labelHiveID
            // 
            this.labelHiveID.AutoSize = true;
            this.labelHiveID.Location = new System.Drawing.Point(200, 23);
            this.labelHiveID.Name = "labelHiveID";
            this.labelHiveID.Size = new System.Drawing.Size(61, 12);
            this.labelHiveID.TabIndex = 10;
            this.labelHiveID.Text = "Hive ID";
            // 
            // textBoxApiID
            // 
            this.textBoxApiLoginID.Location = new System.Drawing.Point(250, 60);
            this.textBoxApiLoginID.MaxLength = 20;
            this.textBoxApiLoginID.Name = "textBoxApiLoginID";
            this.textBoxApiLoginID.Size = new System.Drawing.Size(120, 21);
            this.textBoxApiLoginID.TabIndex = 18;
            this.textBoxApiLoginID.Text = "";
            this.textBoxApiLoginID.WordWrap = false;
            // 
            // labelApiLoginID
            // 
            this.labelApiLoginID.AutoSize = true;
            this.labelApiLoginID.Location = new System.Drawing.Point(200, 62);
            this.labelApiLoginID.Name = "labelApiLoginID";
            this.labelApiLoginID.Size = new System.Drawing.Size(61, 12);
            this.labelApiLoginID.TabIndex = 10;
            this.labelApiLoginID.Text = "API ID";
            // 
            // textBoxApiLoginPW
            // 
            this.textBoxApiLoginPW.Location = new System.Drawing.Point(250, 82);
            this.textBoxApiLoginPW.MaxLength = 100;
            this.textBoxApiLoginPW.Name = "textBoxApiLoginPW";
            this.textBoxApiLoginPW.Size = new System.Drawing.Size(120, 21);
            this.textBoxApiLoginPW.TabIndex = 18;
            this.textBoxApiLoginPW.Text = "";
            this.textBoxApiLoginPW.WordWrap = false;
            // 
            // labelApiLoginPW
            // 
            this.labelApiLoginPW.AutoSize = true;
            this.labelApiLoginPW.Location = new System.Drawing.Point(200, 83);
            this.labelApiLoginPW.Name = "labelApiLoginPW";
            this.labelApiLoginPW.Size = new System.Drawing.Size(61, 12);
            this.labelApiLoginPW.TabIndex = 10;
            this.labelApiLoginPW.Text = "API PW";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(18, defaultY + 645);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(111, 12);
            this.labelStatus.TabIndex = 40;
            this.labelStatus.Text = "서버 접속 상태: ???";
            // 
            // listBoxLog
            // 
            this.listBoxLog.FormattingEnabled = true;
            this.listBoxLog.HorizontalScrollbar = true;
            this.listBoxLog.ItemHeight = 12;
            this.listBoxLog.Location = new System.Drawing.Point(12, defaultY + 388);
            this.listBoxLog.Name = "listBoxLog";
            this.listBoxLog.Size = new System.Drawing.Size(496, 148);
            this.listBoxLog.TabIndex = 41;
            // 
            // textBoxSocketIP
            // 
            this.textBoxSocketIP.Location = new System.Drawing.Point(62, defaultY + 70);
            this.textBoxSocketIP.MaxLength = 100;
            this.textBoxSocketIP.Name = "textBoxSocketIP";
            this.textBoxSocketIP.Size = new System.Drawing.Size(87, 21);
            this.textBoxSocketIP.TabIndex = 45;
            this.textBoxSocketIP.Text = "";
            this.textBoxSocketIP.WordWrap = false;
            // 
            // labelSocketIP
            // 
            this.labelSocketIP.AutoSize = true;
            this.labelSocketIP.Location = new System.Drawing.Point(12, defaultY + 73);
            this.labelSocketIP.Name = "labelSocketIP";
            this.labelSocketIP.Size = new System.Drawing.Size(56, 12);
            this.labelSocketIP.TabIndex = 44;
            this.labelSocketIP.Text = "IP :";
            // 
            // textBoxSocketPort
            // 
            this.textBoxSocketPort.Location = new System.Drawing.Point(230, defaultY + 70);
            this.textBoxSocketPort.MaxLength = 20;
            this.textBoxSocketPort.Name = "textBoxSocketPort";
            this.textBoxSocketPort.Size = new System.Drawing.Size(87, 21);
            this.textBoxSocketPort.TabIndex = 18;
            this.textBoxSocketPort.Text = "";
            this.textBoxSocketPort.WordWrap = false;
            // 
            // labelSocketPort
            // 
            this.labelSocketPort.Location = new System.Drawing.Point(162, defaultY + 75);
            this.labelSocketPort.Name = "labelSocketPort";
            this.labelSocketPort.Size = new System.Drawing.Size(61, 12);
            this.labelSocketPort.TabIndex = 10;
            this.labelSocketPort.Text = "Port : ";
            // 
            // btnSocketConnect
            // 
            this.btnSocketConnect.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSocketConnect.Location = new System.Drawing.Point(330, defaultY + 64);
            this.btnSocketConnect.Name = "btnSocketConnect";
            this.btnSocketConnect.Size = new System.Drawing.Size(69, 26);
            this.btnSocketConnect.TabIndex = 46;
            this.btnSocketConnect.Text = "Connect";
            this.btnSocketConnect.UseVisualStyleBackColor = true;
            this.btnSocketConnect.Click += new System.EventHandler(this.btnSocketConnect_Click);
            //
            // labelSocketID
            // 
            this.labelSocketID.AutoSize = true;
            this.labelSocketID.Location = new System.Drawing.Point(12, defaultY + 101);
            this.labelSocketID.Name = "labelSocketID";
            this.labelSocketID.Size = new System.Drawing.Size(46, 12);
            this.labelSocketID.TabIndex = 42;
            this.labelSocketID.Text = "UserID:";
            // 
            // textBoxSocketID
            // 
            this.textBoxSocketID.Location = new System.Drawing.Point(62, defaultY + 100);
            this.textBoxSocketID.MaxLength = 30;
            this.textBoxSocketID.Name = "textBoxSocketID";
            this.textBoxSocketID.Size = new System.Drawing.Size(87, 21);
            this.textBoxSocketID.TabIndex = 43;
            this.textBoxSocketID.Text = "";
            this.textBoxSocketID.WordWrap = false;
            //
            // textBoxSocketToken
            // 
            this.textBoxSocketToken.Location = new System.Drawing.Point(230, defaultY + 100);
            this.textBoxSocketToken.MaxLength = 100;
            this.textBoxSocketToken.Name = "textBoxSocketToken";
            this.textBoxSocketToken.Size = new System.Drawing.Size(87, 21);
            this.textBoxSocketToken.TabIndex = 45;
            this.textBoxSocketToken.Text = "";
            this.textBoxSocketToken.WordWrap = false;
            // 
            // labelSocketToken
            // 
            this.labelSocketToken.AutoSize = true;
            this.labelSocketToken.Location = new System.Drawing.Point(162, defaultY + 105);
            this.labelSocketToken.Name = "labelSocketToken";
            this.labelSocketToken.Size = new System.Drawing.Size(56, 12);
            this.labelSocketToken.TabIndex = 44;
            this.labelSocketToken.Text = "AuthToken:";
            // 
            // btnSocketLogin
            // 
            this.btnSocketLogin.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSocketLogin.Location = new System.Drawing.Point(330, defaultY + 94);
            this.btnSocketLogin.Name = "btnSocketLogin";
            this.btnSocketLogin.Size = new System.Drawing.Size(69, 26);
            this.btnSocketLogin.TabIndex = 46;
            this.btnSocketLogin.Text = "Login";
            this.btnSocketLogin.UseVisualStyleBackColor = true;
            this.btnSocketLogin.Click += new System.EventHandler(this.btnSocketLogin_Click);
            // 
            // Room
            // 
            this.Room.Controls.Add(this.button1);
            this.Room.Controls.Add(this.button3);
            this.Room.Controls.Add(this.btnMatching);
            this.Room.Controls.Add(this.GameStartBtn);
            this.Room.Controls.Add(this.btnRoomChat);
            this.Room.Controls.Add(this.textBoxRoomSendMsg);
            this.Room.Controls.Add(this.listBoxRoomChatMsg);
            this.Room.Controls.Add(this.listBoxRoomUserList);
            this.Room.Controls.Add(this.btn_RoomLeave);
            this.Room.Controls.Add(this.btn_RoomEnter);
            this.Room.Controls.Add(this.textBoxRoomNumber);
            this.Room.Controls.Add(this.label3);
            this.Room.Location = new System.Drawing.Point(14, defaultY + 119);
            this.Room.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Room.Name = "Room";
            this.Room.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Room.Size = new System.Drawing.Size(495, 264);
            this.Room.TabIndex = 47;
            this.Room.TabStop = false;
            this.Room.Text = "Room";
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button3.Location = new System.Drawing.Point(391, 18);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(91, 28);
            this.button3.TabIndex = 57;
            this.button3.Text = "Game Ready";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnMatching
            // 
            this.btnMatching.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnMatching.Location = new System.Drawing.Point(296, 18);
            this.btnMatching.Name = "btnMatching";
            this.btnMatching.Size = new System.Drawing.Size(78, 28);
            this.btnMatching.TabIndex = 54;
            this.btnMatching.Text = "Matching";
            this.btnMatching.UseVisualStyleBackColor = true;
            this.btnMatching.Click += new System.EventHandler(this.btnMatching_Click);
            // 
            // GameStartBtn
            // 
            this.GameStartBtn.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.GameStartBtn.Location = new System.Drawing.Point(341, 223);
            this.GameStartBtn.Name = "GameStartBtn";
            this.GameStartBtn.Size = new System.Drawing.Size(148, 28);
            this.GameStartBtn.TabIndex = 55;
            this.GameStartBtn.Text = "dummy - GameStart";
            this.GameStartBtn.UseVisualStyleBackColor = true;
            this.GameStartBtn.Click += new System.EventHandler(this.btn_GameStartClick);
            // 
            // btnRoomChat
            // 
            this.btnRoomChat.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRoomChat.Location = new System.Drawing.Point(432, 191);
            this.btnRoomChat.Name = "btnRoomChat";
            this.btnRoomChat.Size = new System.Drawing.Size(50, 26);
            this.btnRoomChat.TabIndex = 53;
            this.btnRoomChat.Text = "chat";
            this.btnRoomChat.UseVisualStyleBackColor = true;
            this.btnRoomChat.Click += new System.EventHandler(this.btnRoomChat_Click);
            // 
            // textBoxRoomSendMsg
            // 
            this.textBoxRoomSendMsg.Location = new System.Drawing.Point(7, 192);
            this.textBoxRoomSendMsg.MaxLength = 32;
            this.textBoxRoomSendMsg.Name = "textBoxRoomSendMsg";
            this.textBoxRoomSendMsg.Size = new System.Drawing.Size(419, 21);
            this.textBoxRoomSendMsg.TabIndex = 52;
            this.textBoxRoomSendMsg.Text = "test1";
            this.textBoxRoomSendMsg.WordWrap = false;
            // 
            // listBoxRoomChatMsg
            // 
            this.listBoxRoomChatMsg.FormattingEnabled = true;
            this.listBoxRoomChatMsg.ItemHeight = 12;
            this.listBoxRoomChatMsg.Location = new System.Drawing.Point(137, 51);
            this.listBoxRoomChatMsg.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.listBoxRoomChatMsg.Name = "listBoxRoomChatMsg";
            this.listBoxRoomChatMsg.Size = new System.Drawing.Size(349, 136);
            this.listBoxRoomChatMsg.TabIndex = 51;
            this.listBoxRoomChatMsg.SelectedIndexChanged += new System.EventHandler(this.listBoxRoomChatMsg_SelectedIndexChanged);
            // 
            // listBoxRoomUserList
            // 
            this.listBoxRoomUserList.FormattingEnabled = true;
            this.listBoxRoomUserList.ItemHeight = 12;
            this.listBoxRoomUserList.Location = new System.Drawing.Point(8, 51);
            this.listBoxRoomUserList.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.listBoxRoomUserList.Name = "listBoxRoomUserList";
            this.listBoxRoomUserList.Size = new System.Drawing.Size(123, 136);
            this.listBoxRoomUserList.TabIndex = 49;
            // 
            // btn_RoomLeave
            // 
            this.btn_RoomLeave.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_RoomLeave.Location = new System.Drawing.Point(216, 18);
            this.btn_RoomLeave.Name = "btn_RoomLeave";
            this.btn_RoomLeave.Size = new System.Drawing.Size(66, 26);
            this.btn_RoomLeave.TabIndex = 48;
            this.btn_RoomLeave.Text = "Leave";
            this.btn_RoomLeave.UseVisualStyleBackColor = true;
            this.btn_RoomLeave.Click += new System.EventHandler(this.btn_RoomLeave_Click);
            // 
            // btn_RoomEnter
            // 
            this.btn_RoomEnter.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_RoomEnter.Location = new System.Drawing.Point(144, 18);
            this.btn_RoomEnter.Name = "btn_RoomEnter";
            this.btn_RoomEnter.Size = new System.Drawing.Size(66, 26);
            this.btn_RoomEnter.TabIndex = 47;
            this.btn_RoomEnter.Text = "Enter";
            this.btn_RoomEnter.UseVisualStyleBackColor = true;
            this.btn_RoomEnter.Click += new System.EventHandler(this.btn_RoomEnter_Click);
            // 
            // textBoxRoomNumber
            // 
            this.textBoxRoomNumber.Location = new System.Drawing.Point(98, 20);
            this.textBoxRoomNumber.MaxLength = 6;
            this.textBoxRoomNumber.Name = "textBoxRoomNumber";
            this.textBoxRoomNumber.Size = new System.Drawing.Size(38, 21);
            this.textBoxRoomNumber.TabIndex = 44;
            this.textBoxRoomNumber.Text = "1";
            this.textBoxRoomNumber.WordWrap = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 12);
            this.label3.TabIndex = 43;
            this.label3.Text = "Room Number:";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Peru;
            this.panel1.Location = new System.Drawing.Point(521, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(604, 657);
            this.panel1.TabIndex = 57;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button1.Location = new System.Drawing.Point(183, 223);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(136, 28);
            this.button1.TabIndex = 58;
            this.button1.Text = "Dummy 유저 등록";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1135, 680);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Room);
            this.Controls.Add(this.btnSocketLogin);
            this.Controls.Add(this.textBoxSocketToken);
            this.Controls.Add(this.labelSocketToken);
            this.Controls.Add(this.textBoxSocketID);
            this.Controls.Add(this.labelSocketID);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.listBoxLog);
            this.Controls.Add(this.btnHiveLogin);
            this.Controls.Add(this.btnRegister);
            this.Controls.Add(this.btnApiLogin);
            this.Controls.Add(this.btnSocketConnect);
            this.Controls.Add(this.textBoxSocketPort);
            this.Controls.Add(this.labelSocketPort);
            this.Controls.Add(this.textBoxSocketIP);
            this.Controls.Add(this.labelSocketIP);
            this.Controls.Add(this.groupBox5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "mainForm";
            this.Text = "네트워크 테스트 클라이언트";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.mainForm_FormClosing);
            this.Load += new System.EventHandler(this.mainForm_Load);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.Room.ResumeLayout(false);
            this.Room.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Button btnHiveLogin;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Button btnApiLogin;
        private System.Windows.Forms.GroupBox groupBox5;

        private System.Windows.Forms.Label labelHiveIP;
        private System.Windows.Forms.TextBox textBoxHiveIP;
        private System.Windows.Forms.Label labelApiIP;
        private System.Windows.Forms.TextBox textBoxApiIP;

        private System.Windows.Forms.TextBox textBoxApiLoginID;
        private System.Windows.Forms.TextBox textBoxApiLoginPW;
        private System.Windows.Forms.Label labelApiLoginID;
        private System.Windows.Forms.Label labelApiLoginPW;

        private System.Windows.Forms.TextBox textBoxSocketIP;
        private System.Windows.Forms.Label labelSocketIP;
        private System.Windows.Forms.TextBox textBoxSocketPort;
        private System.Windows.Forms.Label labelSocketPort; 
        private System.Windows.Forms.Button btnSocketConnect;

        private System.Windows.Forms.TextBox textBoxHivePW;
        private System.Windows.Forms.Label labelHivePW;
        private System.Windows.Forms.TextBox textBoxHiveID;
        private System.Windows.Forms.Label labelHiveID;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.ListBox listBoxLog;
        private System.Windows.Forms.Label labelSocketID;
        private System.Windows.Forms.TextBox textBoxSocketID;
        private System.Windows.Forms.TextBox textBoxSocketToken;
        private System.Windows.Forms.Label labelSocketToken;
        private System.Windows.Forms.Button btnSocketLogin;
        private System.Windows.Forms.GroupBox Room;
        private System.Windows.Forms.Button btn_RoomLeave;
        private System.Windows.Forms.Button btn_RoomEnter;
        private System.Windows.Forms.TextBox textBoxRoomNumber;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRoomChat;
        private System.Windows.Forms.TextBox textBoxRoomSendMsg;
        private System.Windows.Forms.ListBox listBoxRoomChatMsg;
        private System.Windows.Forms.ListBox listBoxRoomUserList;
        private System.Windows.Forms.Button btnMatching;
        private System.Windows.Forms.Button GameStartBtn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
    }
}

