﻿using System;

namespace Jasper.Bus
{
    /// <summary>
    /// Just directs JasperBus to ignore this method as a potential
    /// message handler
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class NotHandlerAttribute : Attribute
    {
         
    }
}