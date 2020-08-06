define([
    "dojo/when",
    "epi/dependency",
    "epi-cms/project/Overview",
    "epi-cms/project/viewmodels/OverviewViewModel"
], function (
    when,
    dependency,
    Overview,
    OverviewViewModel
) {
    function ensureProjectDescriptionNode (projectNode) {
        var parentElement = projectNode.parentElement.parentElement; // div.epi-project-overview__toolbar
        var descriptionEl = parentElement.querySelector(".project-overview-description");
        if (!descriptionEl) {
            descriptionEl = document.createElement("span");
            descriptionEl.classList.add("project-overview-description");
            parentElement.appendChild(descriptionEl);
        }
        return descriptionEl;
    }

    function overrideOverview () {
        Overview.prototype._setProjectDescriptionAttr = function (value) {
            var descriptionEl = ensureProjectDescriptionNode(this.projectNameNode);
            descriptionEl.innerText = value;
        };
        Overview.prototype._setProjectDescriptionAttr.nom = "_setProjectDescriptionAttr";

        Overview.prototype.modelBindingMap.projectDescription = ["projectDescription"];
    };

    function overrideOverviewViewModel () {
        var originalUpdateSelectedProjectDependencies = OverviewViewModel.prototype._updateSelectedProjectDependencies;

        OverviewViewModel.prototype._updateSelectedProjectDependencies = function (selectedProject) {
            originalUpdateSelectedProjectDependencies.apply(this, arguments);

            if (selectedProject) {
                this.extendedProjectStore = this.extendedProjectStore ||
                    dependency.resolve("epi.storeregistry").get("episerver.labs.projectenhancements");
                when(this.extendedProjectStore.get(selectedProject.id)).then(function (project) {
                    if (project) {
                        this.set("projectDescription", project.description);
                    }
                }.bind(this));
            }
        };

        OverviewViewModel.prototype._updateSelectedProjectDependencies.nom = "_updateSelectedProjectDependencies";
    };

    return function () {
        overrideOverview();
        overrideOverviewViewModel();
    };
});

