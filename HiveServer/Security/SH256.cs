using System.Security.Cryptography;
using System.Text;

public class SHA256Hash
{
    const string SecretKey = "Secret";

    public static string? EncodingHash(string value)
    {
        try
        {
            SHA256 sha256Hash = SHA256.Create();
            byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(value));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return null;
        }
    }
}