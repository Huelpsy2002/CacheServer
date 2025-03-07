using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheServer
{




    public class Options
    {
        [Option('p', "port", Required = false,Default = 3000, HelpText = "Port number to run the server.")]
        public int Port { get; set; }

        [Option('u', "url", Required = false,Default ="http://googl.com", HelpText = "the actual server url")]
        public string url { get; set; }

        [Option('h', "help", HelpText = "Display help information.")]
        public bool Help { get; set; }
    }



    internal class Program
    {
        static void ShowHelp()
        {
            Console.WriteLine("Usage: CacheServer --port <port> --url <url>");
            Console.WriteLine("--port, -p      : Port number to run the server");
            Console.WriteLine("--url, -u  : url of the actual server ");
            Console.WriteLine("--help, -h      : Display this help information");
        }


        static void Main(string[] args)
        {
            
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(  options =>
                {
                    if (options.Help)
                    {
                        ShowHelp();
                    }
                    else
                    {
                        Server.Start(options.Port,options.url);
                       
                    }
                })
                .WithNotParsed(errors =>
                {
                    Console.WriteLine("Error parsing arguments.");
                    ShowHelp();
                });
        }
    }
}
