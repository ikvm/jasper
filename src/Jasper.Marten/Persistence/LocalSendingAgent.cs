﻿using System;
using System.Threading.Tasks;
using Jasper.Bus.Runtime;
using Jasper.Bus.Transports.Sending;
using Jasper.Bus.WorkerQueues;
using Marten;

namespace Jasper.Marten
{
    public class LocalSendingAgent : ISendingAgent
    {
        private readonly IWorkerQueue _queues;
        private readonly IDocumentStore _store;
        public Uri Destination { get; }

        public LocalSendingAgent(Uri destination, IWorkerQueue queues, IDocumentStore store)
        {
            _queues = queues;
            _store = store;
            Destination = destination;
        }

        public void Dispose()
        {
            // nothing
        }

        public Uri DefaultReplyUri { get; set; }
        public Task EnqueueOutgoing(Envelope envelope)
        {
            envelope.Callback = new MartenCallback(envelope, _queues, _store);

            // TODO -- filter on delayed jobs here!!!

            return _queues.Enqueue(envelope);
        }

        public async Task StoreAndForward(Envelope envelope)
        {
            using (var session = _store.LightweightSession())
            {
                session.Store(envelope);
                await session.SaveChangesAsync();
            }

            await EnqueueOutgoing(envelope);
        }

        public void Start()
        {
            // Nothing
        }
    }
}