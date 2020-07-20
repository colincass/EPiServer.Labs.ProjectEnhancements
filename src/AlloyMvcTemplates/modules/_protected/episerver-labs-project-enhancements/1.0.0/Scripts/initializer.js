define([
    "dojo/_base/declare",
    "epi/_Module",
    "./project-selector-initialization",
    "./rename-project-command-initializer"
], function (
    declare,
    _Module,
    projectSelectorInitialization,
    renameProjectCommandInitializer
) {
    return declare([_Module], {
        initialize: function () {
            this.inherited(arguments);

            projectSelectorInitialization();
            renameProjectCommandInitializer();
        }
    });
});
