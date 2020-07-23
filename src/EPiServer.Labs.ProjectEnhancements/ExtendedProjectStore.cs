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

        public ExtendedProjectStore(ProjectRepository projectRepository, ViewModelConverter viewModelConverter)
        {
            _projectRepository = projectRepository;
            _viewModelConverter = viewModelConverter;
        }

        [HttpGet]
        public virtual ActionResult Get(int? id, ItemRange range, IEnumerable<SortColumn> sortColumns)
        {
            // If there is no id then return all the projects within the given range.
            if (!id.HasValue)
            {
                // Load all the items in order to apply sorting.
                var result = _projectRepository.List(0, int.MaxValue, out var totalCount);
                var projects = result.Select(_viewModelConverter.ToViewModel);

                if (sortColumns != null)
                {
                    projects = projects.AsQueryable().OrderBy(sortColumns);
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

            return Rest(_viewModelConverter.ToViewModel(project));
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

            //TODO: remove this code
            System.Web.HttpContext.Current.Cache["project____" + id.Value] = projectViewModel.Description;

            _projectRepository.Save(_viewModelConverter.ToProject(projectViewModel));
            return new RestStatusCodeResult(HttpStatusCode.OK) {Data = _viewModelConverter.ToViewModel(project)};
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
                
                return new RestStatusCodeResult(HttpStatusCode.Created) { Data = _viewModelConverter.ToViewModel(project) };
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
