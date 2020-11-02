using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using HangfireWorker;
using HangfireWorker.SQLDatabase;
using Hangfire.SqlServer;
using Autofac;
using RabbitMQEventBus;
using RabbitMQ.Client;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;

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

            var builder = new ContainerBuilder();
            builder.RegisterType<SqlDatabaseConnection>().As<ISqlDatabaseConnection>().InstancePerLifetimeScope();

            builder.RegisterInstance(new LoggerFactory())
                .As<ILoggerFactory>();

            builder.RegisterGeneric(typeof(Logger<>))
                   .As(typeof(ILogger<>))
                   .SingleInstance();

            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq",
                DispatchConsumersAsync = true
            };
            factory.UserName = "user";
            factory.Password = "password";

            builder.Register(c => new DefaultRabbitMqPersistentConnection(c.Resolve<ILogger<DefaultRabbitMqPersistentConnection>>(), factory))
                .As<IRabbitMqPersistentConnection>().SingleInstance();

            builder.Register(c => new RabbitMqClient(c.Resolve<IRabbitMqPersistentConnection>(), c.Resolve<ILogger<RabbitMqClient>>()))
                .As<IEventBus>().InstancePerBackgroundJob();


            builder.Register(x => new HangfireJobForCache(x.Resolve<IEventBus>())).AsSelf().InstancePerBackgroundJob();
            builder.Register(x => new HangfireJobForDatabase(x.Resolve<ISqlDatabaseConnection>())).AsSelf().InstancePerBackgroundJob();

            var container = builder.Build();


            GlobalConfiguration.Configuration.UseAutofacActivator(container);

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
