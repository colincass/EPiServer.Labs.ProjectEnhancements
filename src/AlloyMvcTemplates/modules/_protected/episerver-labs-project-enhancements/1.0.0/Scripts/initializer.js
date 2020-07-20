define([
    "dojo/_base/declare",
    "epi/_Module",
    "./project-selector-initialization"
], function (
    declare,
    _Module,
    projectSelectorInitialization
) {
    return declare([_Module], {
        initialize: function () {
            this.inherited(arguments);

            projectSelectorInitialization();
        }
    });
});
