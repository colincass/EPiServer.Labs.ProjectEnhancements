define([
    "epi/dependency",
    "epi-cms/project/ProjectDialogContent"
], function (
    dependency,
    ProjectDialogContent
) {
    // load project edit form metadata from ExtendedProjectViewModel class

    return function () {
        var originalPostMixinProperties = ProjectDialogContent.prototype.postMixInProperties;

        ProjectDialogContent.prototype.postMixInProperties = function () {
            var metadataManager = this._metadataManager || dependency.resolve("epi.shell.MetadataManager");
            this.metadata = metadataManager.getMetadataForType("EPiServer.Labs.ProjectEnhancements.ExtendedProjectViewModel");

            originalPostMixinProperties.apply(this, arguments);
        };

        ProjectDialogContent.prototype.postMixInProperties.nom = "postMixInProperties";
    };
});
