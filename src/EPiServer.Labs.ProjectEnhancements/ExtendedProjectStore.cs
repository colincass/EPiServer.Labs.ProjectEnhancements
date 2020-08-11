using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EPiServer.Approvals;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework.Serialization;
using EPiServer.Security;
using EPiServer.Shell.Security;
using EPiServer.Shell.Services.Rest;

namespace EPiServer.Labs.ProjectEnhancements
{
    [RestStore("extended-project")]
    public class ExtendedProjectStore : RestControllerBase
    {
        private readonly ProjectRepository _projectRepository;
        private readonly ViewModelConverter _viewModelConverter;
        private readonly IProjectEnhancementsStore _projectEnhancementsStore;
        private readonly IObjectSerializer _objectSerializer;
        private readonly UIRoleProvider _roleProvider;

        public ExtendedProjectStore(ProjectRepository projectRepository,
            ViewModelConverter viewModelConverter,
            IProjectEnhancementsStore projectEnhancementsStore,
            IObjectSerializerFactory objectSerializerFactory, UIRoleProvider roleProvider)
        {
            _projectRepository = projectRepository;
            _viewModelConverter = viewModelConverter;
            _projectEnhancementsStore = projectEnhancementsStore;
            _roleProvider = roleProvider;
            _objectSerializer = objectSerializerFactory.GetSerializer(KnownContentTypes.Json);
        }

        [HttpGet]
        public virtual ActionResult Get(int? id, ItemRange range, IEnumerable<SortColumn> sortColumns)
        {
            var currentUser = PrincipalInfo.CurrentPrincipal.Identity.Name;
            var currentUserRoles = _roleProvider.GetRolesForUser(currentUser).ToList();
            // If there is no id then return all the projects within the given range.
            if (!id.HasValue)
            {
                // Load all the items in order to apply sorting.
                var result = _projectRepository.List(0, int.MaxValue, out var totalCount);
                var projects = result.Select(_viewModelConverter.ToViewModel).ToList();
                AddExtendedFields(projects);

                if (sortColumns != null)
                {
                    projects = projects.AsQueryable().OrderBy(sortColumns).ToList();
                }

                return Rest(projects.Where(x => IsProjectAvailable(x, currentUser, currentUserRoles)), range);
            }

            // Otherwise get the project by id.
            var project = _projectRepository.Get(id.Value);
            if (project == null)
            {
                // Return a 404 if no project exists for the given id.
                return new RestStatusCodeResult(HttpStatusCode.NotFound);
            }

            var extendedProjectViewModel = _viewModelConverter.ToViewModel(project);
            AddExtendedFields(extendedProjectViewModel);
            if (!IsProjectAvailable(extendedProjectViewModel, currentUser, currentUserRoles))
            {
                return new RestStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return Rest(extendedProjectViewModel);
        }

        private bool IsProjectAvailable(ExtendedProjectViewModel projectViewModel, string currentUser, ICollection<string> currentUserRoles)
        {
            try
            {
                var visibleTo = _objectSerializer.Deserialize<IList<UserRole>>(projectViewModel.VisibleTo);
                foreach (var userRole in visibleTo)
                {
                    if (userRole.ReviewerType == ApprovalDefinitionReviewerType.User && userRole.Name == currentUser)
                    {
                        return true;
                    }

                    if (currentUserRoles.Contains(userRole.Name))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private void AddExtendedFields(ExtendedProjectViewModel projectViewModel)
        {
            var projectSettings = _projectEnhancementsStore.Load(projectViewModel.Id);
            if (projectSettings == null)
            {
                return;
            }

            projectViewModel.Description = projectSettings.Description;
            projectViewModel.Categories = projectSettings.Categories;
            projectViewModel.VisibleTo = projectSettings.VisibleTo;
        }

        private void AddExtendedFields(IReadOnlyCollection<ExtendedProjectViewModel> projects)
        {
            var settings = _projectEnhancementsStore.LoadAll().ToList();
            foreach (var extendedProjectViewModel in projects)
            {
                var projectSettings = settings.FirstOrDefault(x => x.ProjectId == extendedProjectViewModel.Id);
                if (projectSettings != null)
                {
                    extendedProjectViewModel.Description = projectSettings.Description;
                    extendedProjectViewModel.Categories = projectSettings.Categories;
                    extendedProjectViewModel.VisibleTo = projectSettings.VisibleTo;
                    if (projectSettings.LastChangedDate.HasValue)
                    {
                        extendedProjectViewModel.Created = projectSettings.LastChangedDate.Value;
                    }

                    if (projectSettings.LastChangedBy != null)
                    {
                        extendedProjectViewModel.CreatedBy = projectSettings.LastChangedBy;
                    }
                }
            }
        }

        public virtual ActionResult Put(int? id, ExtendedProjectViewModel projectViewModel)
        {
            // If no project data is given then it is a bad request.
            if (!id.HasValue || projectViewModel == null)
            {
                return new RestStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Ensure the ID is set on the project data to ensure an update occurs.
            projectViewModel.Id = id.Value;
            var project = _projectRepository.Get(projectViewModel.Id);
            if (project == null)
            {
                return new RestStatusCodeResult(HttpStatusCode.NotFound, "The project doesn't exist.")
                {
                    Data = new
                    {
                        Message = "The project doesn't exist."
                    }
                };
            }

            SaveProjectSettings(projectViewModel);

            _projectRepository.Save(_viewModelConverter.ToProject(projectViewModel));
            project = _projectRepository.Get(projectViewModel.Id);
            var extendedProjectViewModel = _viewModelConverter.ToViewModel(project);
            AddExtendedFields(extendedProjectViewModel);
            return new RestStatusCodeResult(HttpStatusCode.OK) {Data = extendedProjectViewModel};
        }

        [HttpPost]
        public virtual ActionResult Post(ExtendedProjectViewModel projectViewModel)
        {
            // If no project data is given then it is a bad request.
            if (projectViewModel == null)
            {
                return new RestStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var project = Create(_viewModelConverter.ToProject(projectViewModel));
                projectViewModel.Id = project.ID;

                SaveProjectSettings(projectViewModel);
                return new RestStatusCodeResult(HttpStatusCode.Created) {Data = projectViewModel};
            }
            catch (EPiServerException e)
            {
                return new RestStatusCodeResult(HttpStatusCode.Conflict, e.Message)
                {
                    Data = new
                    {
                        Message = e.Message
                    }
                };
            }
        }

        private void SaveProjectSettings(ExtendedProjectViewModel projectViewModel)
        {
            var projectSettings = new ProjectSettings
            {
                Description = projectViewModel.Description,
                Categories = projectViewModel.Categories,
                VisibleTo = projectViewModel.VisibleTo
            };
            _projectEnhancementsStore.Save(projectViewModel.Id, projectSettings);
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            var project = _projectRepository.Get(id);
            if (project == null)
            {
                // Return a 404 if no project exists for the given id.
                return new RestStatusCodeResult(HttpStatusCode.NotFound);
            }

            // TODO: Should this be wrapped in a try-catch?
            _projectRepository.Delete(id);
            _projectEnhancementsStore.Delete(id);

            // Return a 200 OK status code since the operation has completed.
            return new RestStatusCodeResult(HttpStatusCode.OK);
        }

        private Project Create(Project project)
        {
            // make sure the project with given id doesn't present
            if (project.ID > 0)
            {
                if (_projectRepository.Get(project.ID) != null)
                {
                    throw new ApplicationException("The project already exists.");
                }
            }

            _projectRepository.Save(project);

            return _projectRepository.Get(project.ID);
        }
    }
}
