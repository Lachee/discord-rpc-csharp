using DiscordRPC.Message;
using System;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordRPC.Example
{
    interface IExample
    {
        void Setup();
        Task Run();
        void Teardown();
    }

    class Program
    {
        //Main Loop
        static void Main(string[] args)
        {
            IExample example;

            // example = new Basic();
            example = new Joining();

            example.Setup();
            example.Run().GetAwaiter().GetResult();

            Console.WriteLine("Press any key to terminate");
            Console.ReadKey();
            example.Teardown();
        }
    }
}
