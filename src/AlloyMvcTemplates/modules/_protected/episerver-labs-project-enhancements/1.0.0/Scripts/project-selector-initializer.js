define([
    "epi-cms/project/ProjectSelectorList"
], function (
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
                var descriptionTextEl = document.createTextNode(item.description);
                descriptionEl.appendChild(descriptionTextEl);


                originalResult.appendChild(descriptionEl);
            }
            return originalResult;
        }

        ProjectSelectorList.prototype.renderRow.nom = "renderRow";
    }
});

