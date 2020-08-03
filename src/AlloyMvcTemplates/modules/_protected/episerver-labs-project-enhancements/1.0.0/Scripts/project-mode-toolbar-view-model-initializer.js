define([
    "epi/dependency",
    "epi-cms/project/viewmodels/ProjectModeToolbarViewModel"
], function (
    dependency,
    ProjectModeToolbarViewModel
) {
    // use extended project store to manage projects

    return function () {
        var originalPostscript = ProjectModeToolbarViewModel.prototype.postscript;

        ProjectModeToolbarViewModel.prototype.postscript = function () {
            this.projectStore = dependency.resolve("epi.storeregistry").get("episerver.labs.projectenhancements");

            originalPostscript.apply(this, arguments);
        }

        ProjectModeToolbarViewModel.prototype.postscript.nom = "postscript";


        var originalClearProjectStatus = ProjectModeToolbarViewModel.prototype._clearProjectStats;
        ProjectModeToolbarViewModel.prototype._clearProjectStats = function () {
            var result = originalClearProjectStatus.apply(this, arguments);

            result = Object.assign({}, result, { description: "", categories: []  });

            return result;
        }

        ProjectModeToolbarViewModel.prototype._clearProjectStats.nom = "_clearProjectStats";


        
    }
});
