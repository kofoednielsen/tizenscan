using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace tizenscan
{
    class Program
    {
        static void Main(string[] args)
        {
            var timeout = 500;
            if (args.Count() > 0)
                int.TryParse(args[0], out timeout);
            var localIP = GetDefaultGateway();
            if (localIP == null)
                throw new Exception("No internet connection");
            Console.WriteLine("Scanning network for tizen devices..");
            var clients = new List<TcpClient>();
            for (int i = 0; i < 255; i++)
            {
                var splitIp = localIP.ToString().Split(".").ToArray();
                splitIp[3] = i.ToString();
                var ip = string.Join('.', splitIp);
                var client = new TcpClient();
                clients.Add(client);
                TryConnect(client, ip);
            }
            System.Threading.Thread.Sleep(timeout);
            foreach(var client in clients)
            {
                client.Close();
            }
        }
        public static IPAddress GetDefaultGateway()
        {
            return NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up)
                .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .SelectMany(n => n.GetIPProperties()?.GatewayAddresses)
                .Select(g => g?.Address)
                .Where(a => a != null)
                .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
                .FirstOrDefault();
        }
        public static async Task TryConnect(TcpClient client, string ip)
        {
            try
            {
                await client.ConnectAsync(IPAddress.Parse(ip), 26101);
                if (client.Connected)
                {
                    Console.WriteLine(ip);
                }
                client.Close();
            }
            catch(Exception e)
            {
                client.Close();
            }
        }
    }
}
