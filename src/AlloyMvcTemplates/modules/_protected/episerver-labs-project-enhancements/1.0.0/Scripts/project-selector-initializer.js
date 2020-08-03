define([
    "epi-cms/ApplicationSettings",
    "epi-cms/project/ProjectSelectorList"
], function (
    ApplicationSettings,
    ProjectSelectorList
) {
    // Add description to project selector

    return function () {
        var originalRenderRow = ProjectSelectorList.prototype.renderRow;

        ProjectSelectorList.prototype.renderRow = function (item, options) {
            var originalResult = originalRenderRow.apply(this, arguments);
            if (item.description) {
                var descriptionEl = document.createElement("div");
                descriptionEl.classList.add("epi-selector-list__description", "dojoxEllipsis");
                descriptionEl.title = item.description;
                var descriptionTextEl = document.createTextNode(item.description);
                descriptionEl.appendChild(descriptionTextEl);

                if (item.categories) {
                    var categories = item.categories.split(",");
                    categories.forEach(function (category) {
                        var categoryItem = ApplicationSettings.projectCategories.filter(function (c) {
                            return c.id === category;
                        })[0];
                        if (!categoryItem) {
                            return;
                        }
                        var projectEl = document.createElement("span");
                        projectEl.classList.add("project-indicator");
                        projectEl.style.backgroundColor = categoryItem.color;
                        projectEl.title = categoryItem.name;
                        var labelEl = originalResult.querySelector("label.epi-selector-list__title");
                        labelEl.prepend(projectEl);
                    });
                }

                originalResult.appendChild(descriptionEl);
            }
            return originalResult;
        }

        ProjectSelectorList.prototype.renderRow.nom = "renderRow";
    }
});

