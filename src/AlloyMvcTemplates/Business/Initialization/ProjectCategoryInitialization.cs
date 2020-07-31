using System.Collections.Generic;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Labs.ProjectEnhancements.ProjectCategory;
using EPiServer.ServiceLocation;

namespace AlloyTemplates.Business.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class ProjectCategoryInitialization : IConfigurableModule
    {
        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.AddTransient<IProjectCategoriesDataSource, DefaultProjectCategoriesDataSource>();
        }
    }

    public class DefaultProjectCategoriesDataSource: IProjectCategoriesDataSource
    {
        public IEnumerable<ProjectCategoryItem> List()
        {
            return new[]
            {
                new ProjectCategoryItem
                {
                    Id = "Campaigns",
                    Name = "Campaigns",
                    Color = "red",
                    Description = "Used to publish Alloy campaigns"
                },
                new ProjectCategoryItem
                {
                    Id = "Translations",
                    Name = "Translations",
                    Color = "blue",
                    Description = "Used by translation companies"
                },
                new ProjectCategoryItem
                {
                    Id = "Site",
                    Name = "Site branding",
                    Color = "green",
                    Description = "Used when editing content"
                }
            };
        }
    }
}
