using System;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Labs.ProjectEnhancements.ProjectCategory
{
    public class NullProjectCategoriesDataSource: IProjectCategoriesDataSource
    {
        public IEnumerable<ProjectCategoryItem> List()
        {
            return Enumerable.Empty<ProjectCategoryItem>();
        }
    }
}
