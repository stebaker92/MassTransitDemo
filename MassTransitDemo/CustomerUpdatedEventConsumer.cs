using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MassTransitDemo
{
    public class CustomerUpdatedEventConsumer : IConsumer<CustomerUpdatedEvent>
    {
        public CustomerUpdatedEventConsumer(ILogger<CustomerUpdatedEventConsumer> logger)
        {
        }

        public async Task Consume(ConsumeContext<CustomerUpdatedEvent> context)
        {
            Console.WriteLine("Received an event");

            await Task.Delay(1000);
        }
    }
}
