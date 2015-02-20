using System;
using System.Collections;
using System.Linq;
using BizTalkComponents.Utils;

namespace BizTalkComponents.PipelineComponents.ContextRegExReplace
{
    public partial class RegExReplace
    {
        public string Name { get { return "Context regex replace component"; } }
        public string Version { get { return "0.1"; } }
        public string Description { get { return @"Replaces a context value that matches a specific pattern"; } }

        public void GetClassID(out Guid classid)
        {
            classid = new Guid("B26F6F82-1349-4E93-8BFA-F80C72878FE0");
        }

        public void InitNew() { }

        public IntPtr Icon
        {
            get
            {
                return IntPtr.Zero;
            }
        }

        public IEnumerator Validate(object projectSystem)
        {
            return ValidationHelper.Validate(this, false).ToArray().GetEnumerator();
        }

        public bool Validate(out string errorMessage)
        {
            var errors = ValidationHelper.Validate(this, true).ToArray();

            if (errors.Any())
            {
                errorMessage = string.Join(",", errors);

                return false;
            }

            errorMessage = string.Empty;

            return true;
        }
    }
}