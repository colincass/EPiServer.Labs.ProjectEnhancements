define([
    "epi/dependency",
    "epi-cms/project/ProjectDialogContent",
    "epi/shell/widget/FormContainer"
], function (
    dependency,
    ProjectDialogContent,
    FormContainer
) {
    // load project edit form metadata from ExtendedProjectViewModel class

    return function (showDescription) {
        var originalPostMixinProperties = ProjectDialogContent.prototype.postMixInProperties;

        ProjectDialogContent.prototype.postMixInProperties = function () {
            var metadataManager = this._metadataManager || dependency.resolve("epi.shell.MetadataManager");
            this.metadata = metadataManager.getMetadataForType("EPiServer.Labs.ProjectEnhancements.ExtendedProjectViewModel");

            originalPostMixinProperties.apply(this, arguments);
        };

        ProjectDialogContent.prototype.postMixInProperties.nom = "postMixInProperties";

        if (showDescription) {
            // we override postCreate method with base implementation
            // because ProjectDialogContent.prototype.postCreate contains code that close dialog on Enter key
            // and because of that Editor can't add new lines in textarea
            ProjectDialogContent.prototype.postCreate = FormContainer.prototype.postCreate;
        }
    };
});
