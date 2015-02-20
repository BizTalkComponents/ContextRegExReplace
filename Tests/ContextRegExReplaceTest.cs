using System.IO;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Winterdom.BizTalk.PipelineTesting;
using BizTalkComponents.PipelineComponents.ContextRegExReplace;

namespace BizTalkComponents.PipelineComponents.ContextRegExReplace.Tests
{
    [TestClass]
    public class ContextRegExMatchTest
    {
        [TestMethod]
        public void TestReplacePatterWithGroups()
        {
            IBaseMessage message = MessageHelper.CreateFromStream(new MemoryStream());
            message.Context.Promote("test", "http://test.se/prop", @"c:\test\test1\test2.txt");

            SendPipelineWrapper sendPipeline = PipelineFactory.CreateEmptySendPipeline();

            IBaseComponent component = new RegExReplaceComponent();

            ((RegExReplaceComponent)component).ContextNamespace = "http://test.se/prop#test";
            ((RegExReplaceComponent)component).PatternToReplace = @"(^.*\\)(.*$)";
            ((RegExReplaceComponent)component).ValueToSet = "$2";

            sendPipeline.AddComponent(component, PipelineStage.Encode);

            var result = sendPipeline.Execute(message);

            Assert.IsTrue(result.Context.IsPromoted("test", "http://test.se/prop"));
            Assert.IsTrue(result.Context.Read("test", "http://test.se/prop").ToString() == "test2.txt");
        }

        [TestMethod]
        public void TestReplacePatter()
        {
            IBaseMessage message = MessageHelper.CreateFromStream(new MemoryStream());
            message.Context.Write("test", "http://test.se/prop", @"c:\test\test1\test2.txt");

            SendPipelineWrapper sendPipeline = PipelineFactory.CreateEmptySendPipeline();

            IBaseComponent component = new RegExReplaceComponent();

            ((RegExReplaceComponent)component).ContextNamespace = "http://test.se/prop#test";
            ((RegExReplaceComponent)component).PatternToReplace = @"txt";
            ((RegExReplaceComponent)component).ValueToSet = "xml";

            sendPipeline.AddComponent(component, PipelineStage.Encode);

            var result = sendPipeline.Execute(message);

            Assert.IsFalse(result.Context.IsPromoted("test", "http://test.se/prop"));
            Assert.IsTrue(result.Context.Read("test", "http://test.se/prop").ToString() == @"c:\test\test1\test2.xml");
        }

       
    }
}
