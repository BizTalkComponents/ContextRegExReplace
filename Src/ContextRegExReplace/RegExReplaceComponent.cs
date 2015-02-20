using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;

namespace BizTalkComponents.PipelineComponents.ContextRegExReplace
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Receiver)]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    [System.Runtime.InteropServices.Guid("F691AB78-716A-4EEE-888D-8292F54812D3")]

    public class RegExReplaceComponent : IBaseComponent,
        Microsoft.BizTalk.Component.Interop.IComponent,
        IComponentUI,
        IPersistPropertyBag
    {
        public string PatternToReplace { get; set; }
        public string ValueToSet { get; set; }
        public string ContextNamespace { get; set; }

        #region IBaseComponent members

        [Browsable(false)]
        public string Name
        {
            get
            {
                return "Context regex replace component";
            }
        }

        [Browsable(false)]
        public string Version
        {
            get
            {
                return "0.1";
            }
        }

        [Browsable(false)]
        public string Description
        {
            get
            {
                return @"Replaces a context value that matches a specific pattern";
            }
        }

        #endregion

        #region IPersistPropertyBag members

        public void GetClassID(out Guid classid)
        {
            classid = new Guid("B26F6F82-1349-4E93-8BFA-F80C72878FE0");
        }

        public void InitNew() { }

        public virtual void Load(IPropertyBag pb, int errlog)
        {

            var val = ReadPropertyBag(pb, "PatternToReplace");
            if ((val != null))
            {
                PatternToReplace = ((string)(val));
            }

            val = ReadPropertyBag(pb, "ValueToSet");
            if ((val != null))
            {
                ValueToSet = ((string)(val));
            }

            val = ReadPropertyBag(pb, "ContextNamespace");
            if ((val != null))
            {
                ContextNamespace = ((string)(val));
            }
        }

        public virtual void Save(IPropertyBag pb, bool fClearDirty,
            bool fSaveAllProperties)
        {
            WritePropertyBag(pb, "PatternToReplace", PatternToReplace);
            WritePropertyBag(pb, "ValueToSet", ValueToSet);
            WritePropertyBag(pb, "ContextNamespace", ContextNamespace);
        }

        #endregion

        #region Utility functionality

        private static void WritePropertyBag(IPropertyBag pb, string propName, object val)
        {
            try
            {
                pb.Write(propName, ref val);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        private static object ReadPropertyBag(IPropertyBag pb, string propName)
        {
            object val = null;
            try
            {
                pb.Read(propName, out val, 0);
            }

            catch (ArgumentException)
            {
                return val;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return val;
        }

        #endregion

        #region IComponentUI members

        [Browsable(false)]
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

        #endregion

        #region IComponent members

        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            pInMsg.BodyPart.Data = ReadStreamToEndAndSeekToBeginning(pInMsg.BodyPart.Data, true, 1048576);

            var regex = new Regex(PatternToReplace);


            var value = pInMsg.Context.Read(ContextNamespace.PropertyName(), ContextNamespace.PropertyNamespace());

            if (value == null || string.IsNullOrEmpty(value.ToString()))
                throw new ArgumentException("Property to replace can be null");

            var isPromoted = pInMsg.Context.IsPromoted(ContextNamespace.PropertyName(), ContextNamespace.PropertyNamespace());


            var result = regex.Replace(value.ToString(), ValueToSet);

            if (isPromoted)
            {
                pInMsg.Context.Promote(ContextNamespace.PropertyName(), ContextNamespace.PropertyNamespace(), result);
            }
            else
            {
                pInMsg.Context.Write(ContextNamespace.PropertyName(), ContextNamespace.PropertyNamespace(), result);
            }

            return pInMsg;
        }

        internal static Stream ReadStreamToEndAndSeekToBeginning(Stream data, bool seekToBeginning, int bufferSize)
        {
            var buffer = new byte[bufferSize];
            Stream outputStream = data;

            outputStream = new ReadOnlySeekableStream(outputStream);

            while (0 != outputStream.Read(buffer, 0, buffer.Length)) { }

            if (seekToBeginning)
                outputStream.Seek(0, SeekOrigin.Begin);

            return outputStream;
        }


        #endregion
    }
}
