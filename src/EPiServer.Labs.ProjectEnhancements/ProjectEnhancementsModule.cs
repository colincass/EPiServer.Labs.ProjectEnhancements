using System.Collections.Generic;
using System.Linq;
using EPiServer.Framework.Web.Resources;
using EPiServer.Labs.ProjectEnhancements.ProjectCategory;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Modules;

namespace EPiServer.Labs.ProjectEnhancements
{
    public class ProjectEnhancementsModule : ShellModule
    {
        public ProjectEnhancementsModule(string name, string routeBasePath, string resourceBasePath)
            : base(name, routeBasePath, resourceBasePath)
        {
        }

        /// <inheritdoc />
        public override ModuleViewModel CreateViewModel(ModuleTable moduleTable, IClientResourceService clientResourceService)
        {
            var dataSource = ServiceLocator.Current.GetInstance<IProjectCategoriesDataSource>();
            return new ProjectEnhancementsModuleViewModel(this, clientResourceService, dataSource.List().ToList());
        }
    }

    public class ProjectEnhancementsModuleViewModel : ModuleViewModel
    {
        public ProjectEnhancementsModuleViewModel(ShellModule shellModule, IClientResourceService clientResourceService,
            List<ProjectCategoryItem> projectCategories) :
            base(shellModule, clientResourceService)
        {
            ProjectCategories = projectCategories;
        }

        public IEnumerable<ProjectCategoryItem> ProjectCategories { get; }
    }
}
