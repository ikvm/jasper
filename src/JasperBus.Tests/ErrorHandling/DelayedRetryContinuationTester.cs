﻿using System;
using Baseline;
using JasperBus.ErrorHandling;
using JasperBus.Runtime.Invocation;
using NSubstitute;
using Xunit;

namespace JasperBus.Tests.ErrorHandling
{
    public class DelayedRetryContinuationTester
    {
        [Fact]
        public void do_as_a_delay_w_the_timespan_given()
        {
            var continuation = new DelayedRetryContinuation(5.Minutes());
            var context = Substitute.For<IEnvelopeContext>();

            var envelope = ObjectMother.Envelope();

            var now = DateTime.Today.ToUniversalTime();

            continuation.Execute(envelope, context, now);

            envelope.Callback.Received().MoveToDelayedUntil(now.AddMinutes(5));
        }
    }
}