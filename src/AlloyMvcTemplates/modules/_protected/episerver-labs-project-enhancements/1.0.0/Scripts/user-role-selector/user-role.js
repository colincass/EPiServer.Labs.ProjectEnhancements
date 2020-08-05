define([
    "dojo/_base/declare",
    "dojo/dom-class",
    "dojo/dom-construct",
    "dojo/dom-style",
    // Parent class and mixins
    "dijit/_WidgetBase",
    "dijit/Tooltip",
    "dijit/_TemplatedMixin",
    "dijit/_WidgetsInTemplateMixin",
    "epi/shell/widget/_ModelBindingMixin",
    "epi-cms/content-approval/groupMembersListFormatter",
    "epi-cms/content-approval/ApprovalEnums",
    //Resources
    "dojo/text!./user-role.html",
    "epi/i18n!epi/nls/episerver.cms.contentapproval.reviewer"
], function (
    declare,
    domClass,
    domConstruct,
    domStyle,
    // Parent class and mixins
    _WidgetBase,
    Tooltip,
    _TemplatedMixin,
    _WidgetsInTemplateMixin,
    _ModelBindingMixin,
    groupMembersListFormatter,
    ApprovalEnums,
    //Resources
    template,
    localization
) {
    return declare([_WidgetBase, _TemplatedMixin, _WidgetsInTemplateMixin, _ModelBindingMixin], {
        model: null,

        templateString: template,

        resources: localization,

        tooltip: null,

        buildRendering: function () {
            this.inherited(arguments);

            if (this.model.reviewerType === ApprovalEnums.reviewerType.role) {
                this.own(this.tooltip = new Tooltip({
                    connectId: [this.selectedReviewerNode],
                    position: ["below-centered", "above-centered"]
                }));
            }

            this.set("displayName", this.model.displayName);

            domClass.add(this.reviewerIcon, this.model.reviewerType === ApprovalEnums.reviewerType.role ? "epi-iconUsers" : "epi-iconUser");

            this.own(
                this.on(".epi-reviewer__remove-button:click", this._removeUserRole.bind(this)),
                this.on("keydown", this._removeUserRole.bind(this)),
            );
        },

        _removeUserRole: function (evt) {
            if (evt.type === "click") {
                evt.stopPropagation();
                this.onRemoveUserRole(this.model);
            }
        },

        onRemoveUserRole: function () {

        },

        _setDisplayNameAttr: { node: "userDisplayNameNode", type: "innerHTML" }
    });
});

