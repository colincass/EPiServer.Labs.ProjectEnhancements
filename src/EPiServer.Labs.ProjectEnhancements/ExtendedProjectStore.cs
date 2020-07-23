using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Shell.Services.Rest;

namespace EPiServer.Labs.ProjectEnhancements
{
    [RestStore("extended-project")]
    public class ExtendedProjectStore : RestControllerBase
    {
        private readonly ProjectRepository _projectRepository;
        private readonly ViewModelConverter _viewModelConverter;
        private readonly IProjectEnhancementsStore _projectEnhancementsStore;

        public ExtendedProjectStore(ProjectRepository projectRepository,
            ViewModelConverter viewModelConverter,
            IProjectEnhancementsStore projectEnhancementsStore)
        {
            _projectRepository = projectRepository;
            _viewModelConverter = viewModelConverter;
            _projectEnhancementsStore = projectEnhancementsStore;
        }

        [HttpGet]
        public virtual ActionResult Get(int? id, ItemRange range, IEnumerable<SortColumn> sortColumns)
        {
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

                return Rest(projects, range);
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
            return Rest(extendedProjectViewModel);
        }

        private void AddExtendedFields(ExtendedProjectViewModel project)
        {
            var projectSettings = _projectEnhancementsStore.Load(project.Id);
            if (projectSettings == null)
            {
                return;
            }

            project.Description = projectSettings.Description;
        }

        private void AddExtendedFields(IReadOnlyCollection<ExtendedProjectViewModel> projects)
        {
            var settings = _projectEnhancementsStore.LoadAll().ToList();
            foreach (var extendedProjectViewModel in projects)
            {
                var projectSettings = settings.FirstOrDefault(x=>x.ProjectId == extendedProjectViewModel.Id);
                if (projectSettings != null)
                {
                    extendedProjectViewModel.Description = projectSettings.Description;
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

            var projectSettings = new ProjectSettings
            {
                Description = projectViewModel.Description
            };
            _projectEnhancementsStore.Save(id.Value, projectSettings);

            _projectRepository.Save(_viewModelConverter.ToProject(projectViewModel));
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

                //TODO: remove this code
                System.Web.HttpContext.Current.Cache["project____" + projectViewModel.Id] = projectViewModel.Description;

                var extendedProjectViewModel = _viewModelConverter.ToViewModel(project);
                AddExtendedFields(extendedProjectViewModel);
                return new RestStatusCodeResult(HttpStatusCode.Created) { Data = extendedProjectViewModel };
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
