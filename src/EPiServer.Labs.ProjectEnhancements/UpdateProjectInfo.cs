using System;
using EPiServer.Cms.Shell.UI.Rest.Projects;
using EPiServer.Core;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Shell.UI.Internal;

namespace EPiServer.Labs.ProjectEnhancements
{
    [ServiceConfiguration(typeof(IEventListener), Lifecycle = ServiceInstanceScope.Singleton)]
    public class UpdateProjectInfo: IEventListener
    {
        private readonly IContentEvents _contentEvents;
        private readonly IProjectEnhancementsStore _projectEnhancementsStore;
        private readonly CurrentProject _currentProject;

        public UpdateProjectInfo(IContentEvents contentEvents, IProjectEnhancementsStore projectEnhancementsStore, CurrentProject currentProject)
        {
            _contentEvents = contentEvents;
            _projectEnhancementsStore = projectEnhancementsStore;
            _currentProject = currentProject;
        }

        public void Start()
        {
            _contentEvents.SavingContent += ContentEvents_SavingContent;
        }

        public void Stop()
        {
            _contentEvents.SavingContent -= ContentEvents_SavingContent;
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

            projectSettings.LastChangedBy = PrincipalInfo.CurrentPrincipal.Identity.Name;
            projectSettings.LastChangedDate = DateTime.Now;

            _projectEnhancementsStore.Save(_currentProject.ProjectId.Value, projectSettings);
        }
    }
}
