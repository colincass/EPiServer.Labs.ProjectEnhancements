using System.Collections.Generic;

namespace EPiServer.Labs.ProjectEnhancements.ProjectCategory
{
    public interface IProjectCategoriesDataSource
    {
        IEnumerable<ProjectCategoryItem> List();
    }
}
