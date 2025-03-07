using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using CacheServer.configurations;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;

namespace CacheServer
{
    public class Server
    {


        private static readonly HttpClient client = new HttpClient();
        
        private static IMemoryCache MemoryCache = new MemoryCache(new MemoryCacheOptions() );
       

        private static void SetCache<T>(string key, T value)
        {
            MemoryCache.Set(key, value,ConfigureServer.memoryCacheEntryOptions); 
        }

        private static T GetCache<T>(string key)
        {
            return MemoryCache.TryGetValue(key, out T value) ? value : default(T); 
        }


        private static bool CheckExistMemoryCache(string url)
        {

            return MemoryCache.Get(url) != null;
           
           
        }

        private static async Task<HttpResponseMessage> Request(HttpListenerRequest request)
        {
         
        
            try
            {
                string url = ConfigureServer.Url + request.RawUrl;
                
                if (!CheckExistMemoryCache(url)) // if the cache memory  does not have the response,make a request to the actual server 
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                   
                    var content = await response.Content.ReadAsStringAsync();
                    SetCache(url, response);
                    
                    response.Headers.Add("X-Cache-Status", "MISS");
                    return response;
                    
                }
                // if we reached here thats mean the cache memory have the response so we return it
                var cachedResponse =  GetCache<HttpResponseMessage>(url);
                cachedResponse.Headers.Remove("X-Cache-Status"); // remove the header that is cached to prevent mutli values adding to the header
                cachedResponse.Headers.Add("X-Cache-Status", "HIT");
                return cachedResponse;
                
               
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
                return new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    Content = new StringContent("Service unavailable")
                };
            }

        }

        
        private static async Task Response(HttpListenerResponse CacheServerResponse , HttpResponseMessage responseMessage)
        {

            using (CacheServerResponse.OutputStream)
            {
                Console.WriteLine("headers:");
                foreach (var header in responseMessage.Headers)
                {
                    Console.WriteLine(header.Key + ":" + string.Join(", ", header.Value));
                    CacheServerResponse.Headers[header.Key] = string.Join(", ", header.Value);
                }

                    string content = responseMessage.IsSuccessStatusCode ?
                    await responseMessage.Content.ReadAsStringAsync():
                    $"Error: {responseMessage.StatusCode}";
               
                byte[] buffer = Encoding.UTF8.GetBytes(content);
              await  CacheServerResponse.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                await CacheServerResponse.OutputStream.FlushAsync();
            }
        }






        private static async Task StartConnectionListener(HttpListener listener)
        {
            await ConfigureServer.sem.WaitAsync();

            try
            {
                HttpListenerContext context = await listener.GetContextAsync();
                var ActualServerResponse= await Request(context.Request);
                await  Response(context.Response , ActualServerResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                ConfigureServer.sem.Release(); // Ensure the semaphore is released even on error
            }




          
        }


        private static async Task RunServer(HttpListener listener)
        {
            Console.WriteLine("Server started...");
            while (true)
            
            {
          
               
               await StartConnectionListener(listener);
            }
        }

        private static void Start(HttpListener listener)
        {
            listener.Start();
            Task.Run(() => RunServer(listener));
        } 


        public static void Start(int port,string url)
        {
            try
            {
                ConfigureServer.AddPortAndUrl(port, url);
                List<IPAddress> localHostIPs = ConfigureServer.GetLocalHostIPs();
                HttpListener listener = ConfigureServer.InitializeListener(localHostIPs);
                Start(listener);
                Console.ReadLine();
               
            }
            catch(ArgumentException err)
            {
                Console.WriteLine($"Error parsing arguments:{err.Message}");
            }

            catch (Exception ex)
            {
                // General exception handler to catch unforeseen errors
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

        }
    }
}
