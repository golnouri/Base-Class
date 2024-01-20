/*
    Developer: Mojtaba Golnouri
    Free open source utility
    www.golnouri.ir

    مواردی که در این کلاس برای سهولت کار توسعه داده شده است شامل:
    *رمز نگاری یک رشته بر اساس الگوریتم AES
    *بر گرداندن عبارت کد شده به رشته اصلی
    *MD5 کردن یک رشته برای password یا ...
    *تبدیل تاریخ میلادی به شمسی
    *تبدیل تاریخ شمسی به میلادی
    *بررسی خالی بودن یک فیلد
 */

using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Models
{
    public class Utility
    {
        /*  
            for example:
            byte[] code_message  = await Utility.Encrypt(DateTime.Now.ToString());
            string dcode_message = await Utility.Decrypt(code_message);
        */

        public static async Task<byte[]> Encrypt(string input)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] key = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
                aes.Key = key;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] buffer = Encoding.UTF8.GetBytes(input);
                byte[] encrypted = encryptor.TransformFinalBlock(buffer, 0, buffer.Length);
                // اضافه کردن IV به ابتدای داده های رمزگذاری شده
                return aes.IV.Concat(encrypted).ToArray();
            }
        }
        public static async Task<string> Decrypt(byte[] input)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] key = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
                aes.Key = key;
                aes.IV = input.Take(16).ToArray();
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                byte[] buffer = decryptor.TransformFinalBlock(input, 16, input.Length - 16);
                return Encoding.UTF8.GetString(buffer);
            }
        }
        /*  
            for example:
            string[] strings = new string[2];
            strings[0] = us.Name;
        */
        public static bool CheckValue(string[] value)
        {
            List<bool> values = new List<bool>();
            foreach (var item in value)
            {
                if(item == null || item == "")
                {
                    values.Add(false);
                }
                else
                {
                    values.Add(true);
                }
            }
            var dbcheck = values.Where(c => c == false).Count();
            if(dbcheck == 0)
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
