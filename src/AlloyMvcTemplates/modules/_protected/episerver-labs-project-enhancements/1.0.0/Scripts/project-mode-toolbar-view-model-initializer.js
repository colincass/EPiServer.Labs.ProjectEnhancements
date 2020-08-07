define([
    "epi/dependency",
    "epi-cms/project/ProjectModeToolbar",
    "epi-cms/project/viewmodels/ProjectModeToolbarViewModel"
], function (
    dependency,
    ProjectModeToolbar,
    ProjectModeToolbarViewModel
) {
    // change project store to use extended version
    function overridePostscript() {
        var originalPostscript = ProjectModeToolbarViewModel.prototype.postscript;

        ProjectModeToolbarViewModel.prototype.postscript = function () {
            this.projectStore = dependency.resolve("epi.storeregistry").get("episerver.labs.projectenhancements");

            originalPostscript.apply(this, arguments);
        };

        ProjectModeToolbarViewModel.prototype.postscript.nom = "postscript";
    }

    // do not compare description and categorieswhen assigning project
    function overrideClearProjectStatus() {
        var originalClearProjectStatus = ProjectModeToolbarViewModel.prototype._clearProjectStats;
        ProjectModeToolbarViewModel.prototype._clearProjectStats = function () {
            var result = originalClearProjectStatus.apply(this, arguments);

            result = Object.assign({}, result, { created: null, createdBy: null, description: "", categories: [], visibleTo: [] });

            return result;
        };

        ProjectModeToolbarViewModel.prototype._clearProjectStats.nom = "_clearProjectStats";
    }

    // override currentProject with object that contains desription and categories
    function overrideInitialize() {
        var originalInitialize = ProjectModeToolbarViewModel.prototype.initialize;
        ProjectModeToolbarViewModel.prototype.initialize = function () {
            var result = originalInitialize.apply(this, arguments);

            return result.then(function (project) {
                if (!this.currentProject) {
                    return;
                }
                return this.projectStore.get(this.currentProject.id).then(function (project) {
                    this.currentProject = project;
                }.bind(this));
            }.bind(this));
        };

        ProjectModeToolbarViewModel.prototype.initialize.nom = "initialize";
    }

    // override projectModeToolbar
    function overrideProjectModeToolbar() {
        var originalStartup = ProjectModeToolbar.prototype.startup;
        ProjectModeToolbar.prototype.startup = function () {
            originalStartup.apply(this, arguments);

            // categories should be reloaded when editing project
            this.own(this.model.watch("currentProject", function (property, oldValue, newValue) {
                if (newValue && typeof newValue.categories !== "undefined") {
                    this.projectSelector.updateCategories(newValue);
                }
            }.bind(this)));
        };

        ProjectModeToolbar.prototype.startup.nom = "startup";
    }

    return function () {
        overridePostscript();
        overrideClearProjectStatus();
        overrideInitialize();
        overrideProjectModeToolbar();
    };
});
