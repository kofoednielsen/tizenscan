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
        private static Dictionary<string, string> macVendorsMap = new Dictionary<string, string>();
        static void Main(string[] args)
        {
            using (var reader = new StreamReader(File.OpenRead("vendors.txt")))
            {
                var line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    var values = line.Split("\t");
                    var mac = values[0];
                    var vendorName = values[1];
                    macVendorsMap.Add(mac, vendorName);
                }
            }

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
            var lines = data.Split('\n');
            //Get all strings that are a mac address ie. contains 5 dashes
            var maclines = lines.Where(s => s.Count(c => c == '-') == 5).ToList();
            var macsToCheck = new List<MacIp>();
            foreach(var line in maclines)
            {
                var values = line.Split().Select(s => s.Trim()).Where(s => s != string.Empty).ToArray();
                var ip = values[0];
                var mac = values[1].Replace("-", "").ToUpper();
                macsToCheck.Add(new MacIp(mac, ip));
            }
            var samsungMacs = macsToCheck.Where(m => isSamsung(m)).ToList();

            Console.WriteLine("Samsung devices: ");
            samsungMacs.ForEach(t => Console.WriteLine(t.Ip));
        }
        private static bool isSamsung(MacIp macIp)
        {
            string vendor = "";
            string macVendor = macIp.Mac.Substring(0, 6);
            var res = macVendorsMap.TryGetValue(macVendor, out vendor);
            if (res)
                if (vendor.Contains("Samsung"))
                    return true;
                else
                    return false;
            else
                return false;
        }
    }
}
