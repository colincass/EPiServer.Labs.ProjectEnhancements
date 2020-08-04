using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Cms.Shell.Service.Internal;
using EPiServer.Cms.Shell.UI.Rest.Models.Internal;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Security;

namespace EPiServer.Labs.ProjectEnhancements
{
    public class ViewModelConverter
    {
        private readonly  ContentLoaderService _contentLoaderService;
        private readonly ProjectRepository _projectRepository;
        private readonly IContentLoader _contentLoader;
        private static readonly IDictionary<string, int> StatusList = CreateStatusList();

        public const string NoAccesKey = "_nopublishaccess";

        public ViewModelConverter(ProjectRepository projectRepository,
            IContentLoader contentLoader,
            ContentLoaderService contentLoaderService)
        {
            _contentLoaderService = contentLoaderService;
            _projectRepository = projectRepository;
            _contentLoader = contentLoader;
        }

        public ExtendedProjectViewModel ToViewModel(Project project)
        {
            return new ExtendedProjectViewModel
            {
                Id = project.ID,
                Name = project.Name,
                Status = project.Status.ToString().ToLowerInvariant(),
                Created = project.Created,
                CreatedBy = project.CreatedBy,
                DelayPublishUntil = project.DelayPublishUntil,
                ItemStatusCount = GetStatusCount(project.ID),
            };
        }

        public Project ToProject(ExtendedProjectViewModel extendedProjectViewModel)
        {
            return new Project
            {
                ID = extendedProjectViewModel.Id,
                Name = extendedProjectViewModel.Name
            };
        }

        private static IDictionary<string, int> CreateStatusList()
        {
            return Enum.GetNames(typeof(ExtendedVersionStatus)).SelectMany(key =>
            {
                key = key.ToLowerInvariant();
                return new List<string> { key, key + NoAccesKey };
            }).ToDictionary(k => k, k => 0);
        }

        private static bool HasExpired(IVersionable content)
        {
            if (content.Status == VersionStatus.Published && content.StopPublish < DateTime.Now)
            {
                return true;
            }
            return false;
        }

        private IDictionary<string, int> GetStatusCount(int projectId)
        {
            var statuses = StatusList.ToDictionary(di => di.Key, di => di.Value); //clone the dictionary
            var versionableItems = _contentLoader.GetItems(_projectRepository.ListItems(projectId).Select(i => i.ContentLink),
                LanguageSelector.AutoDetect()).OfType<IVersionable>();

            foreach (var versionableItem in versionableItems)
            {
                var status = HasExpired(versionableItem) ? ExtendedVersionStatus.Expired : (ExtendedVersionStatus)versionableItem.Status;
                var key = status.ToString().ToLowerInvariant();

                var contentItem = (IContent)versionableItem;
                var isDeleted = contentItem.IsDeleted;
                var hasEditAccess = _contentLoaderService.HasEditAccess(contentItem, AccessLevel.Publish);

                if (isDeleted || !hasEditAccess)
                {
                    key += NoAccesKey;
                }

                statuses[key]++;
            }

            return statuses;
        }
    }
}
