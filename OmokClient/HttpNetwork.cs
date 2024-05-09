using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSCommon;

namespace GameClient;

public partial class mainForm
{
    HttpClient _hiveServer = new HttpClient();
    HttpClient _apiServer = new HttpClient();

    string _hiveServerUrl = "http://35.92.65.156:5241";
    string _apiServerUrl = "http://35.92.65.156:5122";
    string _socketServerUrl  = "";
    string _userID = "";
    string _authToken = "";

    void InitHttpNetwork(string ip)
    {
        _hiveServerUrl = $"http://{ip}:5241";
        _apiServerUrl = $"http://{ip}:5122";
        _socketServerUrl = ip;

        _hiveServer.BaseAddress = new Uri(_hiveServerUrl);
        _apiServer.BaseAddress = new Uri(_apiServerUrl);
    }

    void HttpNetworkClose()
    {
        _hiveServer.Dispose();
        _apiServer.Dispose();
    }

    async Task HiveRegister(string email, string password)
    {
        HiveRegisterReq req = new HiveRegisterReq();
        req.Email = email;
        req.Password = password;

        var response = await _hiveServer.PostAsJsonAsync("/api/register", req);
        if (response.IsSuccessStatusCode)
        {
            var res = await response.Content.ReadFromJsonAsync<HiveRegisterRes>();
            if (res.IsSuccess)
            {
                MessageBox.Show("Hive 회원가입 성공");
            }
            else
            {
                MessageBox.Show("Hive 회원가입 실패");
            }
        }
        else
        {
            MessageBox.Show("Hive 회원가입 실패");
        }
    }

    async Task<bool> HiveLogin(string email, string password)
    {
        HiveLoginReq req = new HiveLoginReq();
        req.Email = email;
        req.Password = password;
        try
        {
            var response = await _hiveServer.PostAsJsonAsync("/api/login", req);
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadFromJsonAsync<HiveLoginRes>();
                _userID = res.UserID;
                _authToken = res.Token;

                return true;
            }
            else
            {
                MessageBox.Show("Hive 로그인 실패");
                return false;
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
            return false;
        }
    }

    async Task<bool> ApiLogin(string userID, string token)
    {
        ApiLoginReq req = new ApiLoginReq();
        req.UserID = userID;
        req.Token = token;

        try
        {
            var response = await _apiServer.PostAsJsonAsync("/api/login", req);
            if (response.IsSuccessStatusCode)   
            {
                var res = await response.Content.ReadFromJsonAsync<ApiLoginRes>();

                MessageBox.Show("API 로그인 성공");

                _myUserData.UserID = res.GameData.user_id;
                _myUserData.NickName = res.GameData.user_name;
                _myUserData.Level = res.GameData.level;
                _myUserData.Exp = res.GameData.exp;
                _myUserData.Gold = res.GameData.gold;
                _myUserData.Win = res.GameData.win;
                _myUserData.Lose = res.GameData.lose;

                _gameServerAddress = _socketServerUrl;
                _gameServerPort = res.GameServerPort;

                return true;
            }
            else
            {
                MessageBox.Show("API 로그인 실패");

                return false;
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
            return false;
        }
    }
}