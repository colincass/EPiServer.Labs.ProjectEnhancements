define([
    "dojo/_base/declare",
    "dojo/when",

    "epi/dependency",
    "epi-cms/project/command/AddProject",
    "epi-cms/project/command/RenameProject",
    "epi-cms/project/viewmodels/ProjectModeToolbarViewModel",
    "epi/i18n!epi/cms/nls/episerver.shared.action"
], function (
    declare,
    when,

    dependency,
    AddProject,
    RenameProject,
    ProjectToolbarViewModel,
    actionStrings
) {
    // Rename "Rename" command to "Edit"
    var ExtendedProjectRenameCommand = declare([RenameProject], {
        label: actionStrings.edit,
        iconClass: "epi-iconPen",
        dialogClass: "epi-dialog-portraint extended-projects-dialog",

        postscript: function () {
            this.inherited(arguments);

            this.projectStore = dependency.resolve("epi.storeregistry").get("episerver.labs.projectenhancements");
        },

        _execute: function () {
            var currentArgs = arguments;
            when(this.projectStore.get(this.value.id)).then(function (extendedProject) {
                this.set("value", extendedProject);
                this.inherited(currentArgs);
            }.bind(this));
        }
    });

    var ExtendedAddProjectCommand = declare([AddProject], {
        dialogClass: "epi-dialog-portraint extended-projects-dialog"
    });


    return function () {
        var originalCreateCommands = ProjectToolbarViewModel.prototype._createCommands;
        ProjectToolbarViewModel.prototype._createCommands = function () {
            var originalResult = originalCreateCommands.apply(this, arguments);

            var commandArgs = {
                model: this,
                category: "context",
                propertiesToWatch: ["currentProject"]
            };

            var commands = this.get("commands");
            commands[0] = new ExtendedAddProjectCommand(commandArgs);
            commands[1] = new ExtendedProjectRenameCommand(commandArgs);

            return originalResult;
        };
        ProjectToolbarViewModel.prototype._createCommands.nom = "_createCommands";
    };
});

