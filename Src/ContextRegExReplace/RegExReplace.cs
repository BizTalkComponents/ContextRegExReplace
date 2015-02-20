﻿using BizTalkComponents.Utils;
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

        public string PatternToReplace { get; set; }
        public string ValueToSet { get; set; }
        public string ContextNamespace { get; set; }

        public virtual void Load(IPropertyBag pb, int errlog)
        {
            PatternToReplace =
                PropertyBagHelper.ToStringOrDefault(
                    PropertyBagHelper.ReadPropertyBag(pb, PatternToReplacePropertyName), string.Empty);

            ValueToSet =
                PropertyBagHelper.ToStringOrDefault(
                    PropertyBagHelper.ReadPropertyBag(pb, ValueToSetPropertyName), string.Empty);

            ContextNamespace =
                PropertyBagHelper.ToStringOrDefault(
                    PropertyBagHelper.ReadPropertyBag(pb, ContextNamespacePropertyName), string.Empty);
        }

        public virtual void Save(IPropertyBag pb, bool fClearDirty,
            bool fSaveAllProperties)
        {
            PropertyBagHelper.WritePropertyBag(pb, PatternToReplacePropertyName,PatternToReplace);
            PropertyBagHelper.WritePropertyBag(pb, ValueToSetPropertyName, ValueToSet);
            PropertyBagHelper.WritePropertyBag(pb, ContextNamespacePropertyName, ContextNamespace);
        }
        
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
