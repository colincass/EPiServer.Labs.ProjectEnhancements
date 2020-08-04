using EPiServer.ServiceLocation;

namespace EPiServer.Labs.ProjectEnhancements
{
    [Options]
    public class ProjectOptions
    {
        /// <summary>
        /// When true, then page tree will display indicator 
        /// for pages that are part of current project
        /// </summary>
        public bool ShowPageTreeIndicator { get; set; } = true;

        /// <summary>
        /// When true, then project description is available
        /// </summary>
        public bool ShowDescription { get; set; }

        /// <summary>
        /// When true, then project categories are available
        /// </summary>
        public bool ShowCategories { get; set; }

        /// <summary>
        /// When true, then notification tooltip with project information is available
        /// </summary>
        public bool ShowNotificationTooltip { get; set; }
    }
}
