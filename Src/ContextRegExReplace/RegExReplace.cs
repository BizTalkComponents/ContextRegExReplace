using System.ComponentModel.DataAnnotations;
using BizTalkComponents.Utils;
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

    public partial class RegExReplace : IBaseComponent,
        Microsoft.BizTalk.Component.Interop.IComponent,
        IComponentUI,
        IPersistPropertyBag
    {
        private const string PatternToReplacePropertyName = "PatternToReplace";
        private const string ValueToSetPropertyName = "ValueToSet";
        private const string ContextNamespacePropertyName = "ContextNamespace";

        [DisplayName("Pattern To Replace")]
        [Description("Regular expression to use when replacing.")]
        [RequiredRuntime]
        public string PatternToReplace { get; set; }

        [DisplayName("Value to Set")]
        [Description("The value to replace any matches on the RegEx pattern with.")]
        public string ValueToSet { get; set; }

        [DisplayName("Context Namespace")]
        [Description("The path to the context property to replace.")]
        [RegularExpression(@"^.*#.*$",
       ErrorMessage = "A property path should be formatted as namespace#property.")]
        public string ContextNamespace { get; set; }

        public virtual void Load(IPropertyBag pb, int errlog)
        {
            PatternToReplace =
                    PropertyBagHelper.ReadPropertyBag(pb, PatternToReplacePropertyName, PatternToReplace);

            ValueToSet =
                    PropertyBagHelper.ReadPropertyBag(pb, ValueToSetPropertyName, ValueToSet);

            ContextNamespace =
                    PropertyBagHelper.ReadPropertyBag(pb, ContextNamespacePropertyName, ContextNamespace);
        }

        public virtual void Save(IPropertyBag pb, bool fClearDirty,
            bool fSaveAllProperties)
        {
            PropertyBagHelper.WritePropertyBag(pb, PatternToReplacePropertyName, PatternToReplace);
            PropertyBagHelper.WritePropertyBag(pb, ValueToSetPropertyName, ValueToSet);
            PropertyBagHelper.WritePropertyBag(pb, ContextNamespacePropertyName, ContextNamespace);
        }

        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            string errorMessage;

            if (!Validate(out errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }

            pInMsg.BodyPart.Data = ReadStreamToEndAndSeekToBeginning(pInMsg.BodyPart.Data, true, 1048576);

            var regex = new Regex(PatternToReplace);

            string valueToReplace;

            if (!pInMsg.Context.TryRead(new ContextProperty(ContextNamespace), out valueToReplace))
            {
                throw new ArgumentException("Property to replace can be null");
            }

            var isPromoted = pInMsg.Context.IsPromoted(new ContextProperty(ContextNamespace));

            var result = regex.Replace(valueToReplace, ValueToSet);

            if (isPromoted)
            {
                pInMsg.Context.Promote(new ContextProperty(ContextNamespace), result);
            }
            else
            {
                pInMsg.Context.Write(new ContextProperty(ContextNamespace), result);
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
    }
}
