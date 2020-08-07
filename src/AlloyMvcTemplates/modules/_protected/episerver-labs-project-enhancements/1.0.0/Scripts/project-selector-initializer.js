define([
    "dojo/_base/declare",
    "dojo/on",
    "dojo/when",
    "dijit/form/Button",
    "dijit/form/CheckBox",
    "dijit/TooltipDialog",
    "dijit/popup",
    "epi/dependency",
    "epi-cms/ApplicationSettings",
    "epi-cms/project/ProjectSelector",
    "epi-cms/project/ProjectSelectorList"
], function (
    declare,
    on,
    when,
    Button,
    CheckBox,
    TooltipDialog,
    popup,
    dependency,
    ApplicationSettings,
    ProjectSelector,
    ProjectSelectorList
) {
    function addCategories(item, parentEl, asLabels) {
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
            if (asLabels) {
                var nameEl = document.createTextNode(categoryItem.name);
                projectEl.appendChild(nameEl);
                if (categoryItem.description) {
                    projectEl.title = categoryItem.description;
                }
            }
            var description = categoryItem.description ? "\n" + categoryItem.description : "";
            projectEl.title = categoryItem.name + description;
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

            var categoriesEl = document.createElement("div");
            categoriesEl.classList.add("project-list-categories-container");
            originalResult.appendChild(categoriesEl);
            addCategories(item, categoriesEl, true);

            return originalResult;
        };
        ProjectSelectorList.prototype.renderRow.nom = "renderRow";
    }

    function initializeProjectSelector() {
        ProjectSelector.prototype.showTooltip = function (value) {
            // show tooltip after after page was refreshed

            var self = this;

            function showTooltip(projectName) {
                var description = value && value.description ? "<span class='project-description'>" + value.description + "</span>" : "";
                var dialog;
                var CustomDialog = declare([TooltipDialog], {
                    baseClass: "project-tooltip",

                    content:
                        "<div class='project-tooltip'>" +
                            "<span>You are now in project: <strong>" + projectName + "</strong></span>" + description +
                            "<div>" +
                                "<input data-dojo-attach-point='dontShowAgainChk' id='dontShowAgainChk' data-dojo-type='dijit/form/CheckBox' /> <label for='dontShowAgainChk'>Don't show this message again</label>" +
                            "</div>" +
                            "<button class='close-button' data-dojo-attach-point='closeBtn' data-dojo-type='dijit/form/Button' type='button'>Close</button>" +
                            "<div class='clearfix'></div>" +
                        "</div>",

                    startup: function () {
                        this.inherited(arguments);

                        var closeBtn = this.getChildren().filter(x => x.dojoAttachPoint === "closeBtn")[0];
                        var dontShowAgainChk = this.getChildren().filter(x => x.dojoAttachPoint === "dontShowAgainChk")[0];
                        this.own(
                            on(closeBtn, "click", function () {
                                popup.close();
                                dialog.destroyRecursive();
                                dialog = null;
                                if (dontShowAgainChk.get("checked")) {
                                    // save that Editor don't want to see tooltip again
                                    self.profile.set("projects.tooltip.visible", "false");
                                }
                            })
                        );
                    }
                });

                dialog = new CustomDialog();
                dialog.startup();

                popup.open({
                    popup: dialog,
                    around: self.domNode
                });
            }

            if (!this._showTooltip) {
                return;
            }

            // after the value was set we don't want to show tooltip when value seleted project changed
            this._showTooltip = false;

            // when project was not selected, then don't show the tooltip
            if (!value) {
                return;
            }

            this.profile = dependency.resolve("epi.shell.Profile");
            when(this.profile.get("projects.tooltip.visible")).then(function (tooltipVisible) {
                if (tooltipVisible === "false") {
                    return;
                }
                when(this.profile.get("projects.tooltip.last-show")).then(function (tooltipShowDateStr) {
                    this.profile.set("projects.tooltip.last-show", new Date().toString());

                    if (tooltipShowDateStr) {
                        var tooltipDate = new Date(tooltipShowDateStr);
                        var nextTooltipDate = 5 * 60000; // 5 minutes
                        if (new Date(new Date() - nextTooltipDate) > tooltipDate) {
                            showTooltip(value.name);
                        }
                    } else {
                        showTooltip(value.name);
                    }
                }.bind(this));
            }.bind(this));
        };

        // Show categories in project selector
        var originalSetValue = ProjectSelector.prototype._setValueAttr;

        ProjectSelector.prototype._setValueAttr = function (value) {
            var originalResult = originalSetValue.apply(this, arguments);
            this.updateCategories(this.value);
            this.showTooltip(value);
            return originalResult;
        };
        ProjectSelector.prototype._setValueAttr.nom = "_setValueAttr";

        // add new method to prototype `updateCategories` that refresh list of categories
        ProjectSelector.prototype.updateCategories = function (value) {
            this.containerNode.querySelectorAll(".project-indicator").forEach(function (el) {
                this.containerNode.removeChild(el);
            }, this);

            addCategories(value, this.containerNode);
        };

        // show message when refresh the page to let user know that he is in project
        var originalStartup = ProjectSelector.prototype.startup;
        ProjectSelector.prototype.startup = function () {
            // original startup method calls set("value", null), we don't want to show tooltip then
            // but after the velue was set

            this._showTooltip = false;
            originalStartup.apply(this, arguments);
            this._showTooltip = true;
        };
        ProjectSelector.prototype.startup.nom = "startup";
    }

    return function () {
        initializeProjectSelectorList();
        initializeProjectSelector();
    };
});
