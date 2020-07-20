define([
    "epi-cms/project/ProjectSelectorList"
], function (
    ProjectSelectorList
) {
    return function () {
        var originalRenderRow = ProjectSelectorList.prototype.renderRow;

        ProjectSelectorList.prototype.renderRow = function (item, options) {
            var originalResult = originalRenderRow.apply(this, arguments);

            var descriptionEl = document.createElement("div");
            descriptionEl.classList.add("epi-selector-list__description", "dojoxEllipsis");
            var descriptionTextEl = document.createTextNode("DESCRIPTION: " + item.name + "abcdefghij abcdefghij abcdefghij abcdefghij abcdefghij");
            descriptionEl.appendChild(descriptionTextEl);  


            originalResult.appendChild(descriptionEl);  

            return originalResult;
        }

        ProjectSelectorList.prototype.renderRow.nom = "renderRow";
    }
});

