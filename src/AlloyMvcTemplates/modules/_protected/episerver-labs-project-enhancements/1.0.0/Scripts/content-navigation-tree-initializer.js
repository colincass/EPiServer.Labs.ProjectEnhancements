define([
    "epi-cms/component/_ContentNavigationTreeNode"
], function (
    _ContentNavigationTreeNode
) {
    // Show project in content tree

    return function () {
        var originalUpdateIndividualLayout = _ContentNavigationTreeNode.prototype._updateIndividualLayout;

        _ContentNavigationTreeNode.prototype._updateIndividualLayout = function () {
            var originalResult = originalUpdateIndividualLayout.apply(this, arguments);
            if (this.item.capabilities && this.item.capabilities.isPartOfCurrentProject) {
                var projectsContainerEl = this.labelNode.querySelector(".project-indicator-tree-container");
                if (projectsContainerEl) {
                    this.labelNode.removeChild(projectsContainerEl);
                }

                projectsContainerEl = document.createElement("span");
                projectsContainerEl.classList.add("project-indicator-tree-container");

                var projectEl = document.createElement("span");
                projectEl.classList.add("dijitInline", "epi-iconObjectProject");
                projectsContainerEl.appendChild(projectEl);
                projectEl.title = "part of current project";

                this.labelNode.appendChild(projectsContainerEl);
            }
            return originalResult;
        }

        _ContentNavigationTreeNode.prototype._updateIndividualLayout.nom = "_updateIndividualLayout";
    }
});

