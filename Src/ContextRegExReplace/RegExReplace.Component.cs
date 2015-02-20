using System;

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

        public System.Collections.IEnumerator Validate(object obj)
        {
            return null;
        }
    }
}