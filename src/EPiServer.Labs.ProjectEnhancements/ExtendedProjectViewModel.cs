using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using EPiServer.Web;

namespace EPiServer.Labs.ProjectEnhancements
{
    public class ExtendedProjectViewModel
    {
        [ScaffoldColumn(false)]
        public virtual int Id { get; set; }

        [Required]
        [DisplayName("/episerver/shared/header/name")]
        public virtual string Name { get; set; }

        [ScaffoldColumn(false)]
        public virtual string Status { get; set; }

        [ScaffoldColumn(false)]
        public virtual DateTime Created { get; set; }

        [ScaffoldColumn(false)]
        public virtual string CreatedBy { get; set; }

        [ScaffoldColumn(false)]
        public virtual bool CanBePublished { get; set; }

        [ScaffoldColumn(false)]
        public virtual DateTime? DelayPublishUntil { get; set; }

        /// <summary>
        /// Gets or sets the number of ProjectItems which were failed to publish
        /// </summary>
        [ScaffoldColumn(false)]
        public int? FailedItemsCount { get; set; }

        [ScaffoldColumn(false)]
        public virtual IDictionary<string, int> ItemStatusCount { get; set; }

        [UIHint(UIHint.Textarea)]
        public virtual string Description { get; set; }
    }
}
