using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using EPiServer.ServiceLocation;

namespace EPiServer.Labs.ProjectEnhancements
{
    public interface IProjectEnhancementsStore
    {
        void Save(int projectId, ProjectSettings projectSettings);

        void Delete(int projectId);

        ProjectSettings Load(int projectId);

        IEnumerable<ProjectSettings> LoadAll();
    }

    [EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]
    public class ProjectSettings: IDynamicData
    {
        public Identity Id { get; set; }

        [EPiServerDataIndex]
        public int ProjectId { get; set; }

        public string Description { get; set; }
        public string Categories { get; set; }
        public string Color { get; set; }
        public string LastChangedBy { get; set; }
        public DateTime LastChangedDate { get; set; }
    }

    [ServiceConfiguration(typeof(IProjectEnhancementsStore), Lifecycle = ServiceInstanceScope.Singleton)]
    public class DdsProjectEnhancementsStore: IProjectEnhancementsStore
    {
        private readonly object _lock = new object();

        private readonly DynamicDataStoreFactory _dataStoreFactory;

        public DdsProjectEnhancementsStore(DynamicDataStoreFactory dataStoreFactory)
        {
            _dataStoreFactory = dataStoreFactory;
        }

        public void Save(int projectId, ProjectSettings projectSettings)
        {
            lock (_lock)
            {
                var approvalReview = LoadProjectSettings(projectId) ?? new ProjectSettings
                {
                    ProjectId = projectId
                };
                approvalReview.Description = projectSettings.Description;
                approvalReview.Color = projectSettings.Color;
                approvalReview.LastChangedBy = projectSettings.LastChangedBy;
                approvalReview.Categories = projectSettings.Categories;
                approvalReview.LastChangedDate = projectSettings.LastChangedDate;
                GetStore().Save(approvalReview);
            }
        }

        public ProjectSettings Load(int projectId)
        {
            return LoadProjectSettings(projectId);
        }

        public IEnumerable<ProjectSettings> LoadAll()
        {
            return GetStore().Items<ProjectSettings>();
        }

        public void Delete(int projectId)
        {
            lock (_lock)
            {
                var settings = LoadProjectSettings(projectId);
                if (settings == null)
                {
                    return;
                }

                GetStore().Delete(settings.Id);
            }
        }

        private ProjectSettings LoadProjectSettings(int projectId)
        {
            return GetStore().Items<ProjectSettings>().FirstOrDefault(x => x.ProjectId == projectId);
        }

        private DynamicDataStore GetStore()
        {
            return _dataStoreFactory.GetStore(typeof(ProjectSettings)) ?? _dataStoreFactory.CreateStore(typeof(ProjectSettings));
        }
    }
}
