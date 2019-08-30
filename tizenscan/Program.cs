using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace tizenscan
{
    class Program
    {
        public class MacIp
        {
            public string Mac {get; set;}
            public string Ip { get; set; }
            public MacIp(string mac, string ip)
            {
                Mac = mac;
                Ip = ip;
            }
        }
        static void Main(string[] args)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            cmd.StandardInput.WriteLine("arp -a");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            string data = "";
            using (var reader = new StreamReader(cmd.StandardOutput.BaseStream))
            {
                data = reader.ReadToEnd();
            }

            var list = data.Split(new char[0]).Where(s => s != string.Empty).ToList();
            var macsToCheck = new List<MacIp>();
            for(int i = 9; i < list.Count()-5; i++)
            {
                if (list[i].StartsWith("192.168.8"))
                {
                    macsToCheck.Add(new MacIp(list[i + 1], list[i]));
                }
            }
            var tasks = macsToCheck.Select(m => isSamsung(m)).ToArray();
            Task.WaitAll(tasks);

            Console.WriteLine("Tizen devices: ");
            tasks.ToList().Where(t => t.Result != null).ToList().ForEach(t => Console.WriteLine(t.Result.Ip));
        }
        private static async Task<MacIp> isSamsung(MacIp macIp)
        {
            try
            {
                var url = "https://api.macaddress.io/v1?apiKey=at_RCbN6BIjzLTiNzrD0VhthrFfvKOtw&output=json&search=" + macIp.Mac;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    var data = await reader.ReadToEndAsync();
                    var jsonObject = JObject.Parse(data);
                    var companyName = (string)jsonObject["vendorDetails"]["companyName"];
                    if (companyName.Contains("Samsung"))
                    {
                        return macIp;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
