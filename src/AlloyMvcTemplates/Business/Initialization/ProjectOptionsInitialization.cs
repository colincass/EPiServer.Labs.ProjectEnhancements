using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Labs.ProjectEnhancements;

namespace AlloyTemplates.Business.Initialization
{
    [InitializableModule]
    public class ProjectOptionsInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.ConfigurationComplete += (o, e) =>
            {
                context.Services.Configure<ProjectOptions>(x =>
                {
                    x.ShowPageTreeIndicator = false;
                });
            };
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }
    }
}
