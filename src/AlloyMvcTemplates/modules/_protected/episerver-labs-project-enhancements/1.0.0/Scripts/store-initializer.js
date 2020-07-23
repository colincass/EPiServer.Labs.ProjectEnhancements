define([
    "epi/dependency",
    "epi/routes",
    "epi/shell/store/JsonRest",
    "epi/shell/store/Throttle"
], function (
    dependency,
    routes,
    JsonRest,
    Throttle
) {
    // add new project store that allow to edit "description"

    return function () {
        var registry = dependency.resolve("epi.storeregistry");
        registry.add("episerver.labs.projectenhancements",
            new Throttle(
                new JsonRest({
                    target: routes.getRestPath({ moduleArea: "episerver-labs-project-enhancements", storeName: "extended-project" }),
                    idProperty: "id"
                })
            )
        );
    }
});
