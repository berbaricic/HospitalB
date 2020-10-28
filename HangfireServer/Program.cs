using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using HangfireWorker;
using HangfireWorker.SQLDatabase;
using Hangfire.SqlServer;
using Autofac;

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
                services.AddTransient<ISqlDatabaseConnection, SqlDatabaseConnection>();
            });

            var builder = new ContainerBuilder();
            builder.RegisterType<SqlDatabaseConnection>().As<ISqlDatabaseConnection>().InstancePerLifetimeScope();
            builder.RegisterType<HangfireJobForAppointments>().AsSelf().InstancePerBackgroundJob();
            GlobalConfiguration.Configuration.UseAutofacActivator(builder.Build());

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
