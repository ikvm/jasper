using Jasper;
using Jasper.Configuration;
using Jasper.Persistence.Marten;
using Jasper.Persistence.Marten.Codegen;
using Jasper.Persistence.Postgresql;
using Marten;
using Microsoft.Extensions.DependencyInjection;

// SAMPLE: MartenExtension
[assembly:JasperModule(typeof(MartenExtension))]

namespace Jasper.Persistence.Marten
{
    public class MartenExtension : IJasperExtension
    {
        public void Configure(JasperRegistry registry)
        {
            registry.Services.AddSingleton<IDocumentStore>(x =>
            {
                var storeOptions = x.GetService<StoreOptions>();
                var documentStore = new DocumentStore(storeOptions);
                return documentStore;
            });

            registry.Handlers.GlobalPolicy<FineGrainedSessionCreationPolicy>();


            registry.Services.AddScoped(c => c.GetService<IDocumentStore>().OpenSession());
            registry.Services.AddScoped(c => c.GetService<IDocumentStore>().QuerySession());

            registry.CodeGeneration.Sources.Add(new SessionVariableSource());

            registry.Services.AddSingleton(x =>
            {
                var options = x.GetRequiredService<StoreOptions>();
                return new PostgresqlSettings
                {
                    // Super hacky, look away!!!
                    ConnectionString = options.Tenancy.Default.CreateConnection().ConnectionString,
                    SchemaName = options.DatabaseSchemaName
                };
            });
        }
    }
}
// ENDSAMPLE
