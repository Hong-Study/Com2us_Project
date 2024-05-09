using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSCommon;

namespace GameClient;

public partial class mainForm
{

    void InitHttpNetwork()
    {
    }

    void HttpNetworkClose()
    {
    }

    async Task HiveRegister(string url, string email, string password)
    {
        HttpClient hiveServer = new HttpClient();
        hiveServer.BaseAddress = new Uri(url);

        HiveRegisterReq req = new HiveRegisterReq();
        req.Email = email;
        req.Password = password;

        try
        {
            var response = await hiveServer.PostAsJsonAsync("/api/register", req);
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
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        finally
        {
            hiveServer.Dispose();
        }
    }

    async Task<bool> HiveLogin(string url, string email, string password)
    {
        HttpClient hiveServer = new HttpClient();
        hiveServer.BaseAddress = new Uri(url);

        HiveLoginReq req = new HiveLoginReq();
        req.Email = email;
        req.Password = password;
        try
        {
            var response = await hiveServer.PostAsJsonAsync("/api/login", req);
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadFromJsonAsync<HiveLoginRes>();

                textBoxApiLoginID.Text = res.UserID;
                textBoxApiLoginPW.Text = res.Token;

                textBoxSocketID.Text = res.UserID;
                textBoxSocketToken.Text = res.Token;

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
        finally
        {
            hiveServer.Dispose();
        }
    }

    async Task<bool> ApiLogin(string url, string userID, string token)
    {
        HttpClient apiServer = new HttpClient();
        apiServer.BaseAddress = new Uri(url);

        ApiLoginReq req = new ApiLoginReq();
        req.UserID = userID;
        req.Token = token;

        try
        {
            var response = await apiServer.PostAsJsonAsync("/api/login", req);
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

                textBoxSocketIP.Text = res.GameServerAddress;
                textBoxSocketPort.Text = res.GameServerPort.ToString();

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
        finally
        {
            apiServer.Dispose();
        }
    }
}