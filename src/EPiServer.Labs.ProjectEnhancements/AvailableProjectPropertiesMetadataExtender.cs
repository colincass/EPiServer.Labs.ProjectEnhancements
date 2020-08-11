using System;
using System.Collections.Generic;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ObjectEditing;

namespace EPiServer.Labs.ProjectEnhancements
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Cms.Shell.InitializableModule))]
    public class AvailableProjectPropertiesMetadataExtenderInitialization: IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var projectOptions = ServiceLocator.Current.GetInstance<ProjectOptions>();

            if (projectOptions.ShowCategories && projectOptions.ShowDescription)
            {
                // when both description and categories are active, then we don't need to filter properties
                return;
            }

            var registry = context.Locate.Advanced.GetInstance<MetadataHandlerRegistry>();
            registry.RegisterMetadataHandler(typeof(ExtendedProjectViewModel),
                new AvailableProjectPropertiesMetadataExtender(projectOptions));
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }

    public class AvailableProjectPropertiesMetadataExtender: IMetadataExtender
    {
        private readonly ProjectOptions _options;

        public AvailableProjectPropertiesMetadataExtender(ProjectOptions options)
        {
            _options = options;
        }

        public void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            if (metadata.ModelType != typeof(ExtendedProjectViewModel))
            {
                return;
            }

            foreach (var metadataProperty in metadata.Properties)
            {
                if (metadataProperty.PropertyName == nameof(ExtendedProjectViewModel.Description))
                {
                    if (!_options.ShowDescription)
                    {
                        metadataProperty.ShowForEdit = false;
                    }
                }

                if (metadataProperty.PropertyName == nameof(ExtendedProjectViewModel.Categories))
                {
                    if (!_options.ShowCategories)
                    {
                        metadataProperty.ShowForEdit = false;
                    }
                }

                if (metadataProperty.PropertyName == nameof(ExtendedProjectViewModel.VisibleTo))
                {
                    if (!_options.ShowVisibleTo)
                    {
                        metadataProperty.ShowForEdit = false;
                    }
                }
            }
        }
    }
}
