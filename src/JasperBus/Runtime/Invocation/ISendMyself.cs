﻿namespace JasperBus.Runtime.Invocation
{
    public interface ISendMyself
    {
        Envelope CreateEnvelope(Envelope original);
    }
}