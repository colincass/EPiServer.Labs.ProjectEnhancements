define([
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/keys",
    "epi-cms/content-approval/viewmodels/ReviewerViewModel",
    "dijit/form/ComboBox"
], function (
    declare,
    lang,
    keys,
    ReviewerViewModel,
    ComboBox
) {
    return declare([ComboBox], {

        labelType: "html",

        _onKey: function (evt) {

            if (evt.keyCode === keys.UP_ARROW && !this._opened) {
                this.onMoveUp(evt);
                return;
            }

            if (evt.keyCode === keys.ENTER &&
                this._hasSuggestions() &&
                this._hasNoItemSelected()) {
                this.set("item", this.dropDown.items[0]);
            }

            this.inherited(arguments);
        },

        labelFunc: function (item) {
            var iconTemplate = "<span class=\"dijitInline dijitIcon {icon}\"></span>{displayName}";

            var viewModel = new ReviewerViewModel(item);
            return lang.replace(iconTemplate, {
                icon: viewModel.get("icon"),
                displayName: viewModel.get("displayName")
            });
        },

        onMoveUp: function () {
        },

        _hasSuggestions: function () {
            return this._opened &&
                !!this.get("value") &&
                this.dropDown.items.length > 0;
        },

        _hasNoItemSelected: function () {
            return !this.item;
        }
    });
});
