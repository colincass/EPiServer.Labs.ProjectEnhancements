define([
    "dojo/_base/declare",
    "epi/_Module",
    "./store-initializer",
    "./project-selector-initializer",
    "./project-toolbar-commands-initializer",
    "./project-dialog-content-initializer",
    "./project-mode-toolbar-view-model-initializer",
    "./project-notification-initializer",

    "xstyle/css!./extended-projects.css"
], function (
    declare,
    _Module,
    storeInitializer,
    projectSelectorInitializer,
    projectToolbarCommandsInitializer,
    projectDialogContentInitializer,
    projectModeToolbarViewModelInitializer,
    projectNotificationInitializer
) {
    return declare([_Module], {
        initialize: function () {
            this.inherited(arguments);

            storeInitializer();
            projectSelectorInitializer();
            projectToolbarCommandsInitializer();
            projectDialogContentInitializer();
            projectModeToolbarViewModelInitializer();
            projectNotificationInitializer();
        }
    });
});
