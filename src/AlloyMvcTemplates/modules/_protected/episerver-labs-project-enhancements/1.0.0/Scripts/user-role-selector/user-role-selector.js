define([
    "dojo/_base/declare",
    "dojo/on",
    "dojo/aspect",
    "dojo/dom-class",
    // Store modules
    "dojo/store/Memory",
    "dojo/store/Observable",

    "dijit/_CssStateMixin",
    "dijit/_Widget",
    "dijit/_TemplatedMixin",
    "dijit/_WidgetsInTemplateMixin",
    // List and mixins
    "dgrid/List",
    "epi/shell/dgrid/SingleQuery",
    "epi/shell/dgrid/Formatter",
    "dgrid/Keyboard",
    "dgrid/extensions/DijitRegistry",
    "epi/shell/dgrid/WidgetRow",

    "epi/dependency",
    "epi/shell/DestroyableByKey",
    "epi/shell/widget/_ValueRequiredMixin",
    "./user-role",
    "dojo/text!./user-role-selector.html",
    "./combo-box"
], function (
    declare,
    on,
    aspect,
    domClass,
    // Store modules
    Memory,
    Observable,
    _CssStateMixin,
    _Widget,
    _TemplatedMixin,
    _WidgetsInTemplateMixin,
    // List and mixins
    List,
    SingleQuery,
    Formatter,
    Keyboard,
    DijitRegistry,
    WidgetRow,

    dependency,
    DestroyableByKey,
    _ValueRequiredMixin,
    UserRole,
    template,
    ComboBox
) {

    var UserRoleList = declare([List, Keyboard, DijitRegistry, Formatter, SingleQuery, WidgetRow]);

    return declare([_Widget, _TemplatedMixin, _WidgetsInTemplateMixin, _CssStateMixin, _ValueRequiredMixin, DestroyableByKey], {

        templateString: template,

        userRoles: null,

        value: null,

        buildRendering: function () {
            this.inherited(arguments);

            this.list = new UserRoleList({
                className: "epi-chromeless",
                maintainOddEven: false,
                store: this.userRoles,
                //Added sort because without sort you get console logs
                sort: "default",
                renderRow: this._renderUserRole.bind(this)
            }, this.listNode);

            var userStore = dependency.resolve("epi.storeregistry").get("epi.cms.notification.users");
            this.searchBox = new ComboBox({
                labelAttr: "displayName",
                queryExpr: "${0}",
                query: {includeRoles: true},
                store: userStore,
                searchAttr: "name",
                autoComplete: false,
                maxHeight: 294
            }, this.searchNode);

            this.own(
                this.searchBox.on("change", this._searchBoxOnChange.bind(this)),
                aspect.before(this.searchBox, "onSearch", this._searchBoxOnSearch.bind(this))
            );
        },

        startup: function () {
            // summary:
            //      Processing after the DOM fragment is added to the document.
            // tags:
            //      protected

            if (this._started) {
                return;
            }
            this.inherited(arguments);

            this.list.startup();
            this.searchBox.startup();

            // Stop the contentNode from being a tab stop as we don't want to focus an empty list.
            this.list.contentNode.tabIndex = -1;
        },

        _renderUserRole: function (model) {
            var userRole = new UserRole({
                model: model
            });

            this.own(
                on(userRole, "removeUserRole", function (userRole) {
                    this.userRoles.remove(userRole.id);
                }.bind(this))
            );

            return userRole.domNode;
        },

        onChange: function (value) {
            // Event
        },

        _searchBoxOnChange: function (value) {
            if (!value || !this.searchBox.item) {
                return;
            }

            var newItem = this.searchBox.item;
            newItem.id = newItem.name;
            this.userRoles.add(newItem);
            this.searchBox.set("value", "");
        },

        _searchBoxOnSearch: function (users, query, options) {
            users = this._filterOutExistingUsers(users);
            return [users, query, options];
        },

        _filterOutExistingUsers: function (users) {
            var current = this.userRoles.query();

            return users = users.filter(function (user) {
                return !current.some(function (reviewer) {
                    return (user.name.toLowerCase() === reviewer.name.toLowerCase() && user.reviewerType === reviewer.reviewerType);
                });
            }, this);
        },

        _setValueAttr: function (value) {
            this._set("value", value);

            this.userRoles = Observable(new Memory({
                data: value ? JSON.parse(value).map(function (item) {
                    item.id = item.name;
                    return item;
                }) : null
            }));

            this.destroyByKey("observerHandle");
            this.ownByKey("observerHandle", this.userRoles.query().observe(function () {
                var items = JSON.stringify(this.userRoles.query());
                this.onChange(items);
                this.set("value", items);
            }.bind(this)));

            this.list.set("store", this.userRoles);
            var items = this.userRoles.query();
            if (items.length === 0) {
                domClass.add(this.list.domNode, "dijitHidden");
            } else {
                domClass.remove(this.list.domNode, "dijitHidden");
            }
        },

        _setReadOnlyAttr: function (value) {
            this._set("readOnly", value);

            if (this.searchBox) {
                this.searchBox.set("readOnly", value);
            }
        }
    });
});
