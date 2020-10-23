using Hangfire;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace HangfireServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting Hangfire server.");

            GlobalConfiguration.Configuration.UseSqlServerStorage("Server = database; Database = HangfireDatabase; User = sa; Password = Pa&&word2020;");

            var hostBuilder = new HostBuilder().ConfigureServices((hostContext, services) =>
            {
            });

            using (var server = new BackgroundJobServer(new BackgroundJobServerOptions()
            {

                WorkerCount = Environment.ProcessorCount * 5
            }))
            {
                await hostBuilder.RunConsoleAsync();
            }
            
        }
    }
}
