define([
    "dijit/Tooltip",
    "epi/datetime",
    "epi/username",
    "epi-cms/project/ProjectNotification"
], function (
    Tooltip,
    epiDatetime,
    epiUsername,
    ProjectNotification
) {
    // Show tooltip with user information on project link

    return function () {
        if (ProjectNotification.prototype._attachProjectNameClickEvent) {
            var originalProjectNameClickEvent = ProjectNotification.prototype._attachProjectNameClickEvent;

            ProjectNotification.prototype._attachProjectNameClickEvent = function (projectEl, project) {
                if (projectEl) {
                    var originalResult = originalProjectNameClickEvent.apply(this, arguments);

                    var tooltipMessage = epiDatetime.toUserFriendlyString(new Date(project.created)) +
                        ", " +
                        epiUsername.toUserFriendlyString(project.createdBy);

                    var t = new Tooltip({
                        connectId: [projectEl],
                        label: tooltipMessage,
                        position: ["below"]
                    });

                    this.own(t);

                    return originalResult;
                }
            };

            ProjectNotification.prototype._attachProjectNameClickEvent.nom = "_attachProjectNameClickEvent";
        }
    };
});

