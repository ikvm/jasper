﻿using System;
using System.Threading.Tasks;
using Baseline.Dates;
using Jasper.Bus;
using Jasper.Testing.Bus.Runtime;
using Shouldly;
using Xunit;

namespace Jasper.Testing.Bus.Transports.InMemory
{
    public class inmemory_integration_specs : IntegrationContext
    {
        private readonly MessageTracker theTracker = new MessageTracker();

        public inmemory_integration_specs()
        {
            with(_ =>
            {
                _.ListenForMessagesFrom("memory://incoming");
                _.SendMessage<Message1>().To("memory://incoming");

                _.Services.For<MessageTracker>().Use(theTracker);

                _.Services.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.WithDefaultConventions();
                });
            });
        }


        [Fact]
        public async Task send_a_message_and_get_the_response()
        {
            var bus = Runtime.Container.GetInstance<IServiceBus>();

            var task = theTracker.WaitFor<Message1>();

            await bus.Send(new Message1());

            task.Wait(20.Seconds());

            if (!task.IsCompleted)
            {
                throw new Exception("Got no envelope!");
            }

            var envelope = task.Result;

            ShouldBeNullExtensions.ShouldNotBeNull(envelope);
        }
        
    }
}