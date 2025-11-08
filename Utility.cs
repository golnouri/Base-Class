using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Presentation.Models
{
    public class Utility
    {
        private static readonly string MasterKey = "Replace_With_Very_Strong_Master_Key_From_Config";
        private static void DeriveKeyAndIv(out byte[] key, out byte[] iv)
        {
            using (var sha = SHA256.Create())
            {
                byte[] masterBytes = Encoding.UTF8.GetBytes(MasterKey);
                byte[] hash = sha.ComputeHash(masterBytes);
                key = hash;
                byte[] ivSeed = sha.ComputeHash(Encoding.UTF8.GetBytes(MasterKey + "|iv"));
                iv = new byte[16];
                Array.Copy(ivSeed, 0, iv, 0, 16);
            }
        }
        public static string EncryptDeterministic(string plainText)
        {
            if (plainText == null) throw new ArgumentNullException(nameof(plainText));
            DeriveKeyAndIv(out byte[] key, out byte[] iv);
            using (Aes aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = key;
                aes.IV = iv;

                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs, Encoding.UTF8))
                    {
                        sw.Write(plainText);
                    }
                    byte[] cipherBytes = ms.ToArray();
                    return Convert.ToBase64String(cipherBytes);
                }
            }
        }
        public static string DecryptDeterministic(string cipherText)
        {
            if (cipherText == null) throw new ArgumentNullException(nameof(cipherText));
            DeriveKeyAndIv(out byte[] key, out byte[] iv);
            byte[] cipherBytes;
            try
            {
                cipherBytes = Convert.FromBase64String(cipherText);
            }
            catch (FormatException)
            {
                throw new ArgumentException("cipherText is not a valid Base64 string.");
            }
            using (Aes aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = key;
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor())
                using (var ms = new MemoryStream(cipherBytes))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs, Encoding.UTF8))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        public static bool CheckValue(string[] value)
        {
            List<bool> values = new List<bool>();
            foreach (var item in value)
            {
                if (item == null || item == "")
                {
                    values.Add(false);
                }
                else
                {
                    values.Add(true);
                }
            }
            var dbcheck = values.Where(c => c == false).Count();
            if (dbcheck == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static string HashPassword(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
        public static string ConvertToGregorian(string persianDate)
        {
            PersianCalendar pc = new PersianCalendar();
            string[] parts = persianDate.Split('/');
            int year = int.Parse(parts[0]);
            int month = int.Parse(parts[1]);
            int day = int.Parse(parts[2]);
            DateTime dt = new DateTime(year, month, day, pc);
            return dt.ToString("yyyy/MM/dd");
        }
        public static string ToPersianDate(DateTime dt)
        {
            PersianCalendar pc = new PersianCalendar();
            string year = pc.GetYear(dt).ToString();
            string month = pc.GetMonth(dt).ToString();
            string day = pc.GetDayOfMonth(dt).ToString();

            return $"{year}/{month}/{day}";
        }
    }
}
