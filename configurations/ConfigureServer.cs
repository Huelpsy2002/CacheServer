using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;

namespace CacheServer.configurations
{
     public  static class ConfigureServer
    {
        public static int maxSimultaneousConnections { get; private set; } = 20;
        public static SemaphoreSlim sem = new SemaphoreSlim(maxSimultaneousConnections, maxSimultaneousConnections);

        public static MemoryCacheEntryOptions memoryCacheEntryOptions = new MemoryCacheEntryOptions()
        {
            SlidingExpiration = TimeSpan.FromMinutes(5), // Reset expiration when accessed
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };


        public static HttpListener listener { get; private set; }


        public static int Port { get; private set; }
        public static string Url { get; private set; }




        public static void AddPortAndUrl(int port, string url)
        {
   
            Validations.Validate(port, url);
            Port = port;
            Url = url;
   
        }



        public static List<IPAddress> GetLocalHostIPs()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            List<IPAddress> ret = host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToList();

            return ret;
        }





        public static HttpListener InitializeListener(List<IPAddress> localhostIPs)
        {
            listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:{ConfigureServer.Port}/");

            // Listen to IP address as well.
            localhostIPs.ForEach(ip =>
            {
                Console.WriteLine("Listening on IP " + "http://" + ip.ToString() + $":{ConfigureServer.Port}" + "/");
                listener.Prefixes.Add("http://" + ip.ToString() + $":{ConfigureServer.Port}" + "/");
            });

            return listener;
        }
    }
}
