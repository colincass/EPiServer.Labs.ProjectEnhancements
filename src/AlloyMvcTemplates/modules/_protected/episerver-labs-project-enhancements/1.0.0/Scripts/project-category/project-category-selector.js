define([
    "dojo/_base/declare",

    "dijit/_CssStateMixin",
    "dijit/_Widget",
    "dijit/_TemplatedMixin",
    "dijit/_WidgetsInTemplateMixin",


    "epi/epi",
    "epi/shell/widget/_ValueRequiredMixin",

    "epi/shell/widget/CheckedMenuItem",

    "dijit/form/Button",
    "dijit/form/DropDownButton",
    "dijit/DropDownMenu",
    "dijit/TooltipDialog"
],
    function (
        declare,

        _CssStateMixin,
        _Widget,
        _TemplatedMixin,
        _WidgetsInTemplateMixin,

        epi,
        _ValueRequiredMixin,
        CheckedMenuItem
    ) {

        return declare([_Widget, _TemplatedMixin, _WidgetsInTemplateMixin, _CssStateMixin, _ValueRequiredMixin], {

            templateString: "<div class='dijitInline' tabindex='-1' role='presentation'>\
                            <div data-dojo-attach-point='stateNode, tooltipNode'>\
                                <div data-dojo-attach-point='selectButton' data-dojo-type='dijit.form.DropDownButton' data-dojo-attach-event='onMouseDown:onMenuShow'>\
                                    <div class='project-category-dialog' data-dojo-type='dijit/TooltipDialog'>\
                                        <div data-dojo-attach-point='menu' data-dojo-type='dijit.DropDownMenu' id='product-category-menu'>\
                                        </div>\
                                        <div><button data-dojo-type='dijit.form.Button' type='button' data-dojo-attach-event='onClick:onSaveCategories'>Ok</button></div>\
                                    </div>\
                                </div>\
                            </div>\
                        </div>",

            baseClass: "project-category",

            value: null,

            onSaveCategories: function () {
                var selectedItems = this.menu.getChildren().filter(function (menuItem) {
                    return menuItem.get("checked");
                }).map(function (menuItem) {
                    return menuItem.get("categoryId");
                });

                var value = selectedItems.join(",");
                this.set("value", value);
                this.onChange(value);

                this.selectButton.closeDropDown(true);
            },

            onMenuShow: function () {
                var selectedCategories = (this.get("value") || "").split(",");

                this.menu.getChildren().filter(function (menuItem) {
                    menuItem.set("checked", selectedCategories.indexOf(menuItem.categoryId) !== -1);
                });
            },

            onChange: function (value) {
                // Event
            },

            postCreate: function () {
                // call base implementation
                this.inherited(arguments);

                this.selectButton.set("label", "Select categories");

                if (this.categories) {
                    this.categories.forEach(function (c) {
                        var menuItem = new CheckedMenuItem({
                            label: c.name,
                            title: c.description,
                            categoryId: c.id
                        });
                        this.menu.addChild(menuItem);
                    }, this);
                }
            },

            // Setter for value property
            _setValueAttr: function (value) {
                this._set("value", value);

                if (value) {
                    //this.selectButton.set("label", value);
                    this.selectButton.set("label", "Selected " + value.split(",").length);
                } else {
                    this.selectButton.set("label", "Select categories");
                }
            },

            _setReadOnlyAttr: function (value) {
                this._set("readOnly", value);
                this.selectButton.set("readOnly", value);
            }
        });
    });
