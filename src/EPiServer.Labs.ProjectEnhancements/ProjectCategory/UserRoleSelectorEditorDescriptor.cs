using System;
using System.Collections.Generic;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace EPiServer.Labs.ProjectEnhancements.ProjectCategory
{
    /// <summary>
    /// Register an editor for StringList properties
    /// </summary>
    [EditorDescriptorRegistration(TargetType = typeof(string), UIHint = "user-role-selector")]
    public class UserRoleSelectorEditorDescriptor : EditorDescriptor
    {
        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            ClientEditingClass = "episerver-labs-project-enhancements/user-role-selector/user-role-selector";

            base.ModifyMetadata(metadata, attributes);
        }
    }
}
