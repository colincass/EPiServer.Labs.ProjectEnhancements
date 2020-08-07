define([
    "epi-cms/ApplicationSettings",
    "epi-cms/project/ProjectSelector",
    "epi-cms/project/ProjectSelectorList"
], function (
    ApplicationSettings,
    ProjectSelector,
    ProjectSelectorList
) {
    function addCategories(item, parentEl) {
        if (!item || !item.categories) {
            return;
        }
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
            parentEl.prepend(projectEl);
        });
    }


    // Add description to project selector
    function initializeProjectSelectorList() {
        var originalRenderRow = ProjectSelectorList.prototype.renderRow;

        ProjectSelectorList.prototype.renderRow = function (item, options) {
            var originalResult = originalRenderRow.apply(this, arguments);
            if (item.description) {
                var descriptionEl = document.createElement("div");
                descriptionEl.classList.add("epi-selector-list__description", "dojoxEllipsis");
                descriptionEl.title = item.description;
                var descriptionTextEl = document.createTextNode(item.description);
                descriptionEl.appendChild(descriptionTextEl);

                originalResult.appendChild(descriptionEl);
            }

            var labelEl = originalResult.querySelector("label.epi-selector-list__title");
            addCategories(item, labelEl);

            return originalResult;
        };

        ProjectSelectorList.prototype.renderRow.nom = "renderRow";
    }

    // Show categories in project selector
    function initializeProjectSelector() {
        var originalSetValue = ProjectSelector.prototype._setValueAttr;

        ProjectSelector.prototype._setValueAttr = function (item, options) {
            var originalResult = originalSetValue.apply(this, arguments);

            this.updateCategories(this.value);

            return originalResult;
        };

        ProjectSelector.prototype._setValueAttr.nom = "_setValueAttr";

        ProjectSelector.prototype.updateCategories = function (value) {
            this.containerNode.querySelectorAll(".project-indicator").forEach(function (el) {
                this.containerNode.removeChild(el);
            }, this);

            addCategories(value, this.containerNode);
        };
    }

    return function () {
        initializeProjectSelectorList();
        initializeProjectSelector();
    };
});

