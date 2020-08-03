define([
    "dojo/_base/declare",
    "epi/_Module",
    "epi-cms/ApplicationSettings",
    "./store-initializer",
    "./project-selector-initializer",
    "./project-toolbar-commands-initializer",
    "./project-dialog-content-initializer",
    "./project-mode-toolbar-view-model-initializer",
    "./project-notification-initializer",
    "./content-navigation-tree-initializer",

    "xstyle/css!./extended-projects.css"
], function (
    declare,
    _Module,
    ApplicationSettings,

    storeInitializer,
    projectSelectorInitializer,
    projectToolbarCommandsInitializer,
    projectDialogContentInitializer,
    projectModeToolbarViewModelInitializer,
    projectNotificationInitializer,
    contentNavigationTreeInitializer
) {
    return declare([_Module], {
        initialize: function () {
            this.inherited(arguments);

            ApplicationSettings.projectCategories = this._settings.projectCategories;

            storeInitializer();
            projectSelectorInitializer();
            projectToolbarCommandsInitializer();
            projectDialogContentInitializer();
            projectModeToolbarViewModelInitializer();
            projectNotificationInitializer();
            contentNavigationTreeInitializer();
        }
    });
});
