using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MemoryPack;

using Common;

namespace GameClient;
public partial class mainForm
{
    Dictionary<Int16, Action<ClientPacketData>> _onRecv = new Dictionary<Int16, Action<ClientPacketData>>();
    Dictionary<Int16, Action<IMessage>> _onHandler = new Dictionary<Int16, Action<IMessage>>();

    UserData _myUserData = new UserData();
    UserData _anotherUserData = new UserData();

    Dictionary<string, UserData> _userList = new Dictionary<string, UserData>();

    void InitPacketHandler()
    {
        _onRecv.Add((Int16)PacketType.REQ_S_PING, Make<SPingReq>);
        _onHandler.Add((Int16)PacketType.REQ_S_PING, Handle_S_Ping);

        _onRecv.Add((Int16)PacketType.RES_S_CONNECT, Make<SConnectedRes>);
        _onHandler.Add((Int16)PacketType.RES_S_CONNECT, Handle_S_Connected);

        _onRecv.Add((Int16)PacketType.RES_S_LOGIN, Make<SLoginRes>);
        _onHandler.Add((Int16)PacketType.RES_S_LOGIN, Handle_S_Login);
        _onRecv.Add((Int16)PacketType.RES_S_LOGOUT, Make<SLogOutRes>);
        _onHandler.Add((Int16)PacketType.RES_S_LOGOUT, Handle_S_Logout);

        _onRecv.Add((Int16)PacketType.RES_S_ROOM_ENTER, Make<SRoomEnterRes>);
        _onHandler.Add((Int16)PacketType.RES_S_ROOM_ENTER, Handle_S_RoomEnter);
        _onRecv.Add((Int16)PacketType.REQ_S_NEW_USER_ENTER, Make<SNewUserEnterReq>);
        _onHandler.Add((Int16)PacketType.REQ_S_NEW_USER_ENTER, Handle_S_RoomNewUser);
        _onRecv.Add((Int16)PacketType.RES_S_ROOM_LEAVE, Make<SRoomLeaveRes>);
        _onHandler.Add((Int16)PacketType.RES_S_ROOM_LEAVE, Handle_S_RoomLeave);
        _onRecv.Add((Int16)PacketType.REQ_S_USER_LEAVE, Make<SUserLeaveReq>);
        _onHandler.Add((Int16)PacketType.REQ_S_USER_LEAVE, Handle_S_RoomUserLeave);
        _onRecv.Add((Int16)PacketType.RES_S_ROOM_CHAT, Make<SRoomChatRes>);
        _onHandler.Add((Int16)PacketType.RES_S_ROOM_CHAT, Handle_S_RoomChat);

        _onRecv.Add((Int16)PacketType.RES_S_GAME_READY, Make<SGameReadyRes>);
        _onHandler.Add((Int16)PacketType.RES_S_GAME_READY, Handle_S_GameReady);
        _onRecv.Add((Int16)PacketType.REQ_S_GAME_START, Make<SGameStartReq>);
        _onHandler.Add((Int16)PacketType.REQ_S_GAME_START, Handle_S_GameStart);
        _onRecv.Add((Int16)PacketType.RES_S_GAME_PUT, Make<SGamePutRes>);
        _onHandler.Add((Int16)PacketType.RES_S_GAME_PUT, Handle_S_GamePut);
        _onRecv.Add((Int16)PacketType.REQ_S_TURN_CHANGE, Make<STurnChangeReq>);
        _onHandler.Add((Int16)PacketType.REQ_S_TURN_CHANGE, Handle_S_TurnChange);
        _onRecv.Add((Int16)PacketType.REQ_S_GAME_END, Make<SGameEndReq>);
        _onHandler.Add((Int16)PacketType.REQ_S_GAME_END, Handle_S_GameEnd);
        _onRecv.Add((Int16)PacketType.REQ_S_GAME_CANCLE, Make<SGameCancleReq>);
        _onHandler.Add((Int16)PacketType.REQ_S_GAME_CANCLE, Handle_S_GameCancle);
    }

    void ParsingPacket(byte[] bytes)
    {
        ClientPacketData data = new ClientPacketData(bytes);

        Action<ClientPacketData> action = null;
        if (_onRecv.TryGetValue(data.PacketType, out action))
        {
            action(data);
        }
    }

    void Make<T>(ClientPacketData data) where T : IMessage, new()
    {
        T packet = MemoryPackSerializer.Deserialize<T>(data.Body);
        if (packet == null)
        {
            return;
        }

        Action<IMessage> action = null;
        if (_onHandler.TryGetValue(data.PacketType, out action))
        {
            action(packet);
        }
    }

    public static byte[] PacketSerialized<T>(T packet, PacketType type) where T : IMessage
    {
        byte[] bodyData = MemoryPackSerializer.Serialize(packet);
        Int16 bodyDataSize = 0;
        if (bodyData != null)
        {
            bodyDataSize = (Int16)bodyData.Length;
        }
        var packetSize = (Int16)(bodyDataSize + PacketDef.PACKET_HEADER_SIZE);

        var dataSource = new byte[packetSize];
        Buffer.BlockCopy(BitConverter.GetBytes(packetSize), 0, dataSource, 0, 2);
        Buffer.BlockCopy(BitConverter.GetBytes((Int16)type), 0, dataSource, 2, 2);
        dataSource[4] = 0;

        if (bodyData != null)
        {
            Buffer.BlockCopy(bodyData, 0, dataSource, PacketDef.PACKET_HEADER_SIZE, bodyData.Length);
        }

        return dataSource;
    }

    public void Handle_S_Ping(IMessage message)
    {
        SPingReq packet = message as SPingReq;
        if (packet == null)
        {
            return;
        }

        CPongRes pongPacket = new CPongRes();

        byte[] bytes = PacketSerialized(pongPacket, PacketType.RES_C_PONG);
        Network.Send(bytes);
    }

    public void Handle_S_Connected(IMessage message)
    {
        SConnectedRes packet = message as SConnectedRes;
        if (packet == null)
        {
            return;
        }

        if (packet.ErrorCode == (Int16)ErrorCode.NONE)
        {
            DevLog.Write("서버 연결 성공");
        }
        else
        {
            DevLog.Write("서버 연결 실패");
        }
    }

    public void Handle_S_Login(IMessage message)
    {
        SLoginRes packet = message as SLoginRes;
        if (packet == null)
        {
            return;
        }

        if (packet.ErrorCode == (Int16)ErrorCode.NONE)
        {
            DevLog.Write("로그인 성공");
        }
        else
        {
            DevLog.Write("로그인 실패");
        }
    }

    public void Handle_S_Logout(IMessage message)
    {

    }

    public void Handle_S_RoomEnter(IMessage message)
    {
        DevLog.Write("방 입장 패킷 도착");
        SRoomEnterRes packet = message as SRoomEnterRes;
        if (packet == null)
        {
            return;
        }

        if (packet.ErrorCode != (Int16)ErrorCode.NONE)
        {
            MessageBox.Show($"에러 발생 {packet.ErrorCode}");
            return;
        }

        foreach (var user in packet.UserList)
        {
            _anotherUserData = user;

            _userList.Add(user.UserID, user);
            listBoxRoomUserList.Items.Add(user.NickName);
        }

        DevLog.Write($"방 입장 성공: {packet.UserList.Count}");
    }

    public void Handle_S_RoomLeave(IMessage message)
    {

    }

    public void Handle_S_RoomUserLeave(IMessage message)
    {
        SUserLeaveReq packet = message as SUserLeaveReq;
        if (packet == null)
        {
            return;
        }

        if (_userList.TryGetValue(packet.UserID, out UserData user))
        {
            DevLog.Write($"{user.NickName}님이 퇴장하였습니다.");
            
            _userList.Remove(packet.UserID);
            listBoxRoomUserList.Items.Remove(user.NickName);
        }
        else
        {
            // TODO
        }
    }

    public void Handle_S_RoomNewUser(IMessage message)
    {
        SNewUserEnterReq packet = message as SNewUserEnterReq;
        if (packet == null)
        {
            return;
        }

        DevLog.Write($"새로운 유저 입장: {packet.User.NickName}");
        _userList.Add(packet.User.UserID, packet.User);
        listBoxRoomUserList.Items.Add(packet.User.NickName);

        _anotherUserData = packet.User;
    }

    public void Handle_S_RoomChat(IMessage message)
    {
        SRoomChatRes packet = message as SRoomChatRes;
        if (packet == null)
        {
            return;
        }

        string msg = $"{packet.UserName} : {packet.Message}";
        listBoxRoomChatMsg.Items.Add(msg);
    }

    public void Handle_S_GameReady(IMessage message)
    {
        // TODO
    }

    public void Handle_S_GameStart(IMessage message)
    {
        SGameStartReq packet = message as SGameStartReq;
        if (packet == null)
        {
            return;
        }

        if (packet.IsStart == true)
        {
            MessageBox.Show("게임 시작");
            bool isMyTurn = false;
            if (packet.StartPlayerID == _myUserData.UserID)
            {
                isMyTurn = true;
            }
            else
            {
                isMyTurn = false;
            }

            DevLog.Write($"게임 시작: {isMyTurn} {_myUserData.NickName} {_anotherUserData.NickName}");
            StartGame(isMyTurn, _myUserData.NickName, _anotherUserData.NickName);
        }
    }

    public void Handle_S_GameEnd(IMessage message)
    {
        SGameEndReq packet = message as SGameEndReq;
        if (packet == null)
        {
            return;
        }

        if (packet.WinUserID == _myUserData.UserID)
        {
            MessageBox.Show("당신이 승리하였습니다.");
        }
        else
        {
            MessageBox.Show("당신이 패배하였습니다.");
        }

        EndGame();
    }

    public void Handle_S_GamePut(IMessage message)
    {
        SGamePutRes packet = message as SGamePutRes;
        if (packet == null)
        {
            return;
        }

        if (packet.ErrorCode != (Int16)ErrorCode.NONE)
        {
            MessageBox.Show($"에러 발생 {packet.ErrorCode}");
        }

        플레이어_돌두기(true, packet.PosX, packet.PosY);
    }

    public void Handle_S_TurnChange(IMessage message)
    {
        // IsMyTurn
        STurnChangeReq packet = message as STurnChangeReq;
        if (packet == null)
        {
            return;
        }

        if (packet.NextTurnUserID == _myUserData.UserID)
        {
            MessageBox.Show("당신의 차례입니다.");
            TurnChange(true);
        }
        else
        {
            MessageBox.Show("상대방의 차례입니다.");
            TurnChange(false);
        }
    }

    public void Handle_S_GameCancle(IMessage message)
    {
        MessageBox.Show("게임이 종료되었습니다.");

        _userList.Clear();
        listBoxRoomUserList.Items.Clear();
    }
}