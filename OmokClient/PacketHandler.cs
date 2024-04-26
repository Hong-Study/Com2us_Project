using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MemoryPack;

namespace GameClient;
public partial class mainForm
{
    Dictionary<Int16, Action<ClientPacketData>> _onRecv = new Dictionary<Int16, Action<ClientPacketData>>();
    Dictionary<Int16, Action<IMessage>> _onHandler = new Dictionary<Int16, Action<IMessage>>();

    void InitPacketHandler()
    {
        _onRecv.Add((Int16)PacketType.RES_S_LOGIN, Make<SLoginRes>);
        _onHandler.Add((Int16)PacketType.RES_S_LOGIN, Handle_S_Login);
        _onRecv.Add((Int16)PacketType.REQ_C_LOGOUT, Make<SLogOutRes>);
        _onHandler.Add((Int16)PacketType.REQ_C_LOGOUT, Handle_S_Logout);

        _onRecv.Add((Int16)PacketType.REQ_C_ROOM_ENTER, Make<SRoomEnterRes>);
        _onHandler.Add((Int16)PacketType.REQ_C_ROOM_ENTER, Handle_S_RoomEnter);
        _onRecv.Add((Int16)PacketType.REQ_C_ROOM_LEAVE, Make<SRoomLeaveRes>);
        _onHandler.Add((Int16)PacketType.REQ_C_ROOM_LEAVE, Handle_S_RoomLeave);
        _onRecv.Add((Int16)PacketType.REQ_C_ROOM_CHAT, Make<SRoomChatRes>);
        _onHandler.Add((Int16)PacketType.REQ_C_ROOM_CHAT, Handle_S_RoomChat);

        _onRecv.Add((Int16)PacketType.REQ_C_GAME_READY, Make<SGameReadyRes>);
        _onHandler.Add((Int16)PacketType.REQ_C_GAME_READY, Handle_S_GameReady);
        _onRecv.Add((Int16)PacketType.RES_C_GAME_START, Make<SGameStartReq>);
        _onHandler.Add((Int16)PacketType.RES_S_GAME_READY, Handle_S_GameStart);
        _onRecv.Add((Int16)PacketType.REQ_C_GAME_PUT, Make<SGamePutRes>);
        _onHandler.Add((Int16)PacketType.REQ_C_GAME_PUT, Handle_S_GamePut);
        _onRecv.Add((Int16)PacketType.RES_C_TURN_CHANGE, Make<STurnChangeReq>);
        _onHandler.Add((Int16)PacketType.RES_C_TURN_CHANGE, Handle_S_TurnChange);
        _onRecv.Add((Int16)PacketType.RES_C_GAME_END, Make<SGameEndReq>);
        _onHandler.Add((Int16)PacketType.RES_C_GAME_END, Handle_S_GameEnd);
        _onRecv.Add((Int16)PacketType.RES_C_GAME_CANCLE, Make<SGameCancleReq>);
        _onHandler.Add((Int16)PacketType.RES_C_GAME_CANCLE, Handle_S_GameCancle);
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

    public void Handle_S_Login(IMessage message)
    {
        SLoginRes res = message as SLoginRes;
        if (res == null)
        {
            return;
        }

        if (res.ErrorCode == (Int16)ErrorCode.NONE)
        {
            MessageBox.Show("로그인 성공");
        }
        else
        {
            MessageBox.Show("로그인 실패");
        }
    }

    public void Handle_S_Logout(IMessage message)
    {

    }

    public void Handle_S_RoomEnter(IMessage message)
    {
        
    }

    public void Handle_S_RoomLeave(IMessage message)
    {

    }

    public void Handle_S_RoomUserLeave(IMessage message)
    {

    }

    public void Handle_S_RoomNewUser(IMessage message)
    {

    }

    public void Handle_S_RoomChat(IMessage message)
    {

    }

    public void Handle_S_GameReady(IMessage message)
    {
        SGamePutRes res = message as SGamePutRes;
        if (res == null)
        {
            return;
        }

        if(res.ErrorCode != (Int16)ErrorCode.NONE)
        {
            MessageBox.Show("에러 발생");
        }

        플레이어_돌두기(true, res.X, res.Y);
    }

    public void Handle_S_GameStart(IMessage message)
    {

    }

    public void Handle_S_GameEnd(IMessage message)
    {
        
    }

    public void Handle_S_GamePut(IMessage message)
    {

    }

    public void Handle_S_TurnChange(IMessage message)
    {

    }

    public void Handle_S_GameCancle(IMessage message)
    {

    }
}