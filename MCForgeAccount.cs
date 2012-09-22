using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace MCForge
{
    public static class MCForgeAccount
    {
        public static bool LoggedIn { get; private set; }
        public static CookieContainer Cookies { get; private set; }


        public static bool Login()
        {
            return Login(Server.mcforgeUser, Server.mcforgePass);
        }

        public static bool Login(string user, string pass)
        {
            try
            {
                LoggedIn = false;
                Server.s.Log("Logging into MCForge.net...");
                byte[] data = Encoding.ASCII.GetBytes(string.Format("action=do_login&username={0}&password={1}", user, pass));

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.mcforge.net/forums/member.php");
                request.CookieContainer = new CookieContainer();
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                request.Timeout = 15000; // 15 seconds

                using (Stream stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string responseData = reader.ReadToEnd();
                        if (responseData.IndexOf("You have successfully been logged in.") != -1)
                        {
                            Server.s.Log("Successfully logged into MCForge.net!");
                            Cookies = new CookieContainer();
                            Cookies.Add(response.Cookies);
                            LoggedIn = true;
                        }
                        else Server.s.Log("Login failed!");
                    }
                }

                return LoggedIn;
            }
            catch (Exception ex)
            {
                Server.ErrorLog(ex);
                return false;
            }
        }
    }
}
