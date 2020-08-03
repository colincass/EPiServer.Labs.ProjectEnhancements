using System.Linq;
using EPiServer.Cms.Shell.UI.Rest.Capabilities;
using EPiServer.Cms.Shell.UI.Rest.Projects;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;

namespace EPiServer.Labs.ProjectEnhancements.ProjectTreeIndicator
{
    [ServiceConfiguration(typeof(IContentCapability))]
    public class IsPartOfCurrentProject : IContentCapability
    {
        private readonly CurrentProject _currentProject;
        private readonly ProjectRepository _projectRepository;

        public IsPartOfCurrentProject(CurrentProject currentProject, ProjectRepository projectRepository)
        {
            _currentProject = currentProject;
            _projectRepository = projectRepository;
        }

        public string Key => "isPartOfCurrentProject";

        public virtual bool IsCapable(IContent content)
        {
            var projectItems = _projectRepository.GetItems(new [] {content.ContentLink.ToReferenceWithoutVersion()});
            var currentProjectId = _currentProject.ProjectId;
            var isCapable = projectItems.Any(x => x.ProjectID == currentProjectId);
            return isCapable;
        }

        public virtual int SortOrder => 1000;
    }
}
