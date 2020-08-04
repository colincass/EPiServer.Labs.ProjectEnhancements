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
    }
}
