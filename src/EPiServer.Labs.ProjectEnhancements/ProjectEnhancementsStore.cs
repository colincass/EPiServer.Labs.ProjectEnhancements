using System;
using System.Collections.Generic;
using System.Linq;
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

    public class ProjectSettings
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
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
                var approvalReview = LoadApprovalReview(projectId) ?? new ProjectSettings
                {
                    Id = projectId
                };
                approvalReview.Description = projectSettings.Description;
                approvalReview.Color = projectSettings.Color;
                approvalReview.LastChangedBy = projectSettings.LastChangedBy;
                approvalReview.Category = projectSettings.Category;
                approvalReview.LastChangedDate = projectSettings.LastChangedDate;
                GetStore().Save(approvalReview);
            }
        }

        public ProjectSettings Load(int projectId)
        {
            return LoadApprovalReview(projectId);
        }

        public IEnumerable<ProjectSettings> LoadAll()
        {
            return GetStore().Items<ProjectSettings>();
        }

        public void Delete(int projectId)
        {
            lock (_lock)
            {
                var settings = LoadApprovalReview(projectId);
                if (settings == null)
                {
                    return;
                }

                GetStore().Delete(settings.Id);
            }
        }

        private ProjectSettings LoadApprovalReview(int projectId)
        {
            return GetStore().Items<ProjectSettings>().FirstOrDefault(x => x.Id == projectId);
        }

        private DynamicDataStore GetStore()
        {
            return _dataStoreFactory.GetStore(typeof(ProjectSettings)) ?? _dataStoreFactory.CreateStore(typeof(ProjectSettings));
        }
    }
}
