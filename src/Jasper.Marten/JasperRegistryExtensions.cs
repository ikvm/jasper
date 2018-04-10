﻿using System;
using Jasper.Settings;
using Marten;
using Microsoft.Extensions.Configuration;

namespace Jasper.Marten
{
    public static class JasperRegistryExtensions
    {
        public static void MartenConnectionStringIs(this JasperSettings settings, string connectionString)
        {
            settings.Alter<StoreOptions>(x => x.Connection(connectionString));
        }

        public static void MartenConnectionStringIs(this JasperRegistry registry, string connectionString)
        {
            registry.Settings.MartenConnectionStringIs(connectionString);
        }

        public static void ConfigureMarten(this JasperRegistry registry, Action<StoreOptions> configuration)
        {
            registry.Settings.ConfigureMarten(configuration);
        }

        public static void ConfigureMarten(this JasperSettings settings, Action<StoreOptions> configuration)
        {
            settings.Alter(configuration);
        }

        public static void ConfigureMarten(this JasperSettings settings, Action<IConfiguration, StoreOptions> configuration)
        {
            settings.Alter<StoreOptions>((c, o) => configuration(c.Configuration, o));
        }
    }
}
