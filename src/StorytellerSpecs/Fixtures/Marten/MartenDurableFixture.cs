﻿using System;
using System.Threading.Tasks;
using IntegrationTests;
using Jasper;
using Jasper.Messaging;
using Jasper.Messaging.Durability;
using Jasper.Messaging.Runtime;
using Jasper.Messaging.Runtime.Invocation;
using Jasper.Persistence;
using Jasper.Persistence.Marten;
using Jasper.Persistence.Marten.Persistence;
using Jasper.Persistence.Marten.Persistence.Operations;
using Marten;
using StorytellerSpecs.Fixtures.Durability;

namespace StorytellerSpecs.Fixtures.Marten
{
    public class MartenDurableFixture : DurableFixture<TriggerMessageReceiver, ItemCreatedHandler>
    {
        public MartenDurableFixture()
        {
            Title = "Marten Outbox & Scheduled Message Mechanics";
        }

        protected override void configureReceiver(JasperRegistry receiverRegistry)
        {
            receiverRegistry.Settings.ConfigureMarten(marten =>
            {
                marten.Connection(Servers.PostgresConnectionString);
                marten.DatabaseSchemaName = "outbox_receiver";
            });

            receiverRegistry.Include<MartenBackedPersistence>();
        }

        protected override void configureSender(JasperRegistry senderRegistry)
        {
            senderRegistry.Settings.ConfigureMarten(marten =>
            {
                marten.Connection(Servers.PostgresConnectionString);
                marten.DatabaseSchemaName = "outbox_sender";
            });

            senderRegistry.Include<MartenBackedPersistence>();
        }


        protected override void initializeStorage(IJasperHost theSender, IJasperHost theReceiver)
        {
            theSender.RebuildMessageStorage();

            theReceiver.RebuildMessageStorage();

        }

        protected override ItemCreated loadItem(IJasperHost receiver, Guid id)
        {
            using (var session = receiver.Get<IDocumentStore>().QuerySession())
            {
                return session.Load<ItemCreated>(id);
            }
        }


        protected override async Task withContext(IJasperHost sender, IMessageContext context,
            Func<IMessageContext, Task> action)
        {
            var senderStore = sender.Get<IDocumentStore>();

            using (var session = senderStore.LightweightSession())
            {
                await context.EnlistInTransaction(session);

                await action(context);

                await session.SaveChangesAsync();
            }
        }

        protected override Envelope[] loadAllOutgoingEnvelopes(IJasperHost sender)
        {
            var admin = sender.Get<IEnvelopePersistence>().Admin;
            return admin.AllOutgoingEnvelopes().GetAwaiter().GetResult();
        }
    }

    public class TriggerMessageReceiver
    {
        [Transactional]
        public object Handle(TriggerMessage message, IDocumentSession session, IMessageContext context)
        {
            var response = new CascadedMessage
            {
                Name = message.Name
            };

            return new RespondToSender(response);
        }
    }

    public class ItemCreatedHandler
    {
        [Transactional]
        public static void Handle(ItemCreated created, IDocumentSession session,
            Jasper.Messaging.Tracking.MessageTracker tracker,
            Envelope envelope)
        {
            session.Store(created);
            tracker.Record(created, envelope);
        }
    }
}
