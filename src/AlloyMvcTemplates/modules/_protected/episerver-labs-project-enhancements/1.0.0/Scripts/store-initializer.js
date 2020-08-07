define([
    "epi/dependency",
    "epi/routes",
    "epi-cms/store/configurableQueryEngine"
], function (
    dependency,
    routes,
    configurableQueryEngine
) {
    // add new project store that allow to edit "description"

    return function () {
        var registry = dependency.resolve("epi.storeregistry");
        registry.create("episerver.labs.projectenhancements",
            routes.getRestPath({ moduleArea: "episerver-labs-project-enhancements", storeName: "extended-project" }),
            { queryEngine: configurableQueryEngine, realtimeInfo: { subscriptionKey: "/episerver/cms/project" } }
        );
    };
});
