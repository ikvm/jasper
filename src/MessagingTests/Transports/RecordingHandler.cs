﻿using Jasper.Messaging.Runtime;
using Jasper.Messaging.Tracking;
using TestMessages;

namespace MessagingTests.Transports
{
    public class RecordingHandler
    {
        private readonly MessageTracker _tracker;

        public RecordingHandler(MessageTracker tracker)
        {
            _tracker = tracker;
        }

        public void Handle(Message1 message, Envelope envelope)
        {
            _tracker.Record(message, envelope);
        }
    }
}
