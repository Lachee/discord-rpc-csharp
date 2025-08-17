using DiscordRPC.Message;
using System;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordRPC.Example
{
    class Options
	{
		public string Example { get; set; } = "Basic";
		public string ClientId { get; set; } = "424087019149328395";
		public int Pipe { get; set; } = -1;
        public Logging.LogLevel LogLevel { get; set; } = Logging.LogLevel.Info;
    }

	interface IExample
    {
        void Setup(Options opts);
        Task Run();
        void Teardown();
    }

	class Program
    {
        //Main Loop
        static void Main(string[] args)
        {
            // Parse arguments
            Options opts = new Options();
            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    if (arg.StartsWith("--pipe="))
                    {
                        opts.Pipe = int.Parse(arg.Substring("--pipe=".Length));
                    }
					else if (arg.StartsWith("--clientid="))
					{
						opts.ClientId = arg.Substring("--clientid=".Length);
					}
					else if (arg.StartsWith("--example="))
					{
						opts.Example = arg.Substring("--example=".Length);
					}
					else if (arg.StartsWith("--loglevel="))
                    {
                        if (Enum.TryParse<Logging.LogLevel>(arg.Substring("--loglevel=".Length), true, out var logLevel))
                        {
                            opts.LogLevel = logLevel;
                        }
                        else
                        {
                            Console.WriteLine($"Invalid log level: {arg.Substring("--loglevel=".Length)}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Unknown argument: {arg}");
					}
				}
			}

            // Run example
			IExample example = opts.Example switch
            {
                "Basic"     => new Basic(),
                "Joining"   => new Joining(),
                _           => throw new ArgumentException($"Unknown example: {opts.Example}")
            };

            example.Setup(opts);
            example.Run().GetAwaiter().GetResult();

            Console.WriteLine("Press any key to terminate");
            Console.ReadKey();
            example.Teardown();
        }
    }
}
