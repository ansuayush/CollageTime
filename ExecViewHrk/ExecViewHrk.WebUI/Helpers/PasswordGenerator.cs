using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

namespace ExecViewHrk.Domain.Helper
{
    //
    public class PasswordGenerator
    {
        
        public static string GeneratePassword()
        {
            Random prng = new Random();
            const int minCH = 8; //minimum chars in random string
            const int maxCH = 8; //maximum chars in random string

            //valid chars in random string
            const string randCH = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < prng.Next(minCH, maxCH + 1); i++)
            {
                sb.Append(randCH.Substring(prng.Next(0, randCH.Length), 1));
            }

            return sb.ToString();
        }

        
    }
}
