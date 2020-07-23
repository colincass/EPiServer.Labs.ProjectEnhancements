define([
    "epi-cms/project/command/RenameProject",
    "epi/i18n!epi/cms/nls/episerver.shared.action"
], function (
    RenameProject,
    actionStrings
) {
    // Rename "Rename" command to "Edit"

    return function () {
        RenameProject.prototype.label = actionStrings.edit;
        RenameProject.prototype.iconClass = "epi-iconPen";
    }
});

