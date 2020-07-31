using System;
using System.Collections.Generic;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace EPiServer.Labs.ProjectEnhancements.ProjectCategory
{
    /// <summary>
    /// Register an editor for StringList properties
    /// </summary>
    [EditorDescriptorRegistration(TargetType = typeof(string), UIHint = "project-category")]
    public class ProjectCategoryEditorDescriptor : EditorDescriptor
    {
        private readonly IProjectCategoriesDataSource _projectCategoriesDataSource;

        public ProjectCategoryEditorDescriptor(IProjectCategoriesDataSource projectCategoriesDataSource)
        {
            _projectCategoriesDataSource = projectCategoriesDataSource;
        }

        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            ClientEditingClass = "episerver-labs-project-enhancements/project-category/project-category-selector";
            this.EditorConfiguration["categories"] = _projectCategoriesDataSource.List();

            base.ModifyMetadata(metadata, attributes);
        }
    }
}
