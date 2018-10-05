using GreenPipes;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransitDemo
{
    class Program
    {
        private static readonly string _hostName = "localhost";
        private static readonly string _virtualHost = "/";
        private static readonly string _queueName = "MyConsumer";

        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddLogging();

            services.AddScoped<CustomerUpdatedEventConsumer>();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<CustomerUpdatedEventConsumer>();
            });

            services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(_hostName, _virtualHost, h => { });

                cfg.ReceiveEndpoint(host, _queueName, e =>
                {
                    e.PrefetchCount = 16;

                    e.UseMessageRetry(x => x.Interval(2, 100));

                    e.LoadFrom(provider);
                });
            }));

            services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());

            // If we're running this in the background of an ASP.NET Core application, we should run as a HostedService
            //services.AddSingleton<IHostedService, BusService>();

            var serviceProvider = services.BuildServiceProvider();

            var bus = serviceProvider.GetRequiredService<IBusControl>();

            bus.Start();


            // Simulate an Event being received
            Thread.Sleep(1_000);

            bus.Publish(new CustomerUpdatedEvent
            {
                CustomerId = 1
            });
        }
    }
}
