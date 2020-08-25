using System;
using System.Linq;
using EPiServer.Cms.Shell.UI.Rest.Projects;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Shell.UI.Internal;

namespace EPiServer.Labs.ProjectEnhancements
{
    [ServiceConfiguration(typeof(IEventListener), Lifecycle = ServiceInstanceScope.Singleton)]
    public class UpdateProjectInfo : IEventListener
    {
        private readonly IContentEvents _contentEvents;
        private readonly IProjectEnhancementsStore _projectEnhancementsStore;
        private readonly CurrentProject _currentProject;
        private readonly ProjectOptions _projectOptions;

        public UpdateProjectInfo(IContentEvents contentEvents, IProjectEnhancementsStore projectEnhancementsStore,
            CurrentProject currentProject,
            ProjectOptions projectOptions)
        {
            _contentEvents = contentEvents;
            _projectEnhancementsStore = projectEnhancementsStore;
            _currentProject = currentProject;
            _projectOptions = projectOptions;
        }

        public void Start()
        {
            if (_projectOptions.ShowLastEditInfo)
            {
                _contentEvents.SavingContent += ContentEvents_SavingContent;
            }
        }

        public void Stop()
        {
            if (_projectOptions.ShowLastEditInfo)
            {
                _contentEvents.SavingContent -= ContentEvents_SavingContent;
            }
        }

        private void ContentEvents_SavingContent(object sender, ContentEventArgs e)
        {
            if (!_currentProject.ProjectId.HasValue)
            {
                return;
            }

            var projectSettings = _projectEnhancementsStore.Load(_currentProject.ProjectId.Value);
            if (projectSettings == null)
            {
                projectSettings = new ProjectSettings();
            }

            projectSettings.UpdateProjectEditInfo();

            _projectEnhancementsStore.Save(_currentProject.ProjectId.Value, projectSettings);
        }
    }

    internal static class UpdateProjectEditInfoExtensions
    {
        public static void UpdateProjectEditInfo(this ProjectSettings projectSettings)
        {
            projectSettings.LastChangedBy = PrincipalInfo.CurrentPrincipal.Identity.Name;
            projectSettings.LastChangedDate = DateTime.Now;
        }
    }

    /// <summary>
    /// Initializer that update project edit information
    /// when item is added or deleted from project
    /// </summary>
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Cms.Shell.InitializableModule))]
    public class LastEditInfoInitializer : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var projectOptions = ServiceLocator.Current.GetInstance<ProjectOptions>();

            if (!projectOptions.ShowLastEditInfo)
            {
                return;
            }

            ProjectRepository.ProjectItemsSaved += ProjectRepository_ProjectItemsSaved;
            ProjectRepository.ProjectItemsDeleted += ProjectRepository_ProjectItemsDeleted;
        }

        private void ProjectRepository_ProjectItemsDeleted(object sender, ProjectItemsEventArgs e)
        {
            UpdateProjectEditInfo(e);
        }

        private void ProjectRepository_ProjectItemsSaved(object sender, ProjectItemsEventArgs e)
        {
            UpdateProjectEditInfo(e);
        }

        private void UpdateProjectEditInfo(ProjectItemsEventArgs e)
        {
            var projectIds = e.ProjectItems.Select(x => x.ProjectID).Distinct();
            foreach (var projectId in projectIds)
            {
                UpdateProjectEditInfo(projectId);
            }
        }

        private void UpdateProjectEditInfo(int projectId)
        {
            var projectEnhancementsStore = ServiceLocator.Current.GetInstance<IProjectEnhancementsStore>();

            var projectSettings = projectEnhancementsStore.Load(projectId);
            if (projectSettings == null)
            {
                projectSettings = new ProjectSettings();
            }

            projectSettings.UpdateProjectEditInfo();

            projectEnhancementsStore.Save(projectId, projectSettings);
        }

        public void Uninitialize(InitializationEngine context)
        {
            var projectOptions = ServiceLocator.Current.GetInstance<ProjectOptions>();

            if (!projectOptions.ShowLastEditInfo)
            {
                return;
            }

            ProjectRepository.ProjectItemsSaved -= ProjectRepository_ProjectItemsSaved;
        }
    }
}
