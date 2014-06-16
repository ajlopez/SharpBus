namespace SharpBus.Core.Tests
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FlowTests
    {
        [TestMethod]
        public void CreateFlow()
        {
            var flow = Flow.Create();

            Assert.IsNotNull(flow);
        }

        [TestMethod]
        public void SendPayloadToEmptyFlow()
        {
            var flow = Flow.Create();

            Assert.AreEqual(1, flow.Send(1));
        }

        [TestMethod]
        public void SendPayloadToFlowWithOutput()
        {
            string result = null;

            var flow = Flow.Create()
                .Output(x => result = (string)x);

            Assert.IsNull(flow.Send("foo"));
            Assert.AreEqual("foo", result);
        }

        [TestMethod]
        public void ApplyTransformToInteger()
        {
            var flow = Flow.Create().Transform(x => ((int)x) + 1);

            Assert.AreEqual(2, flow.Send(1));
        }

        [TestMethod]
        public void ApplyProcess()
        {
            int total = 0;

            var flow = Flow.Create().Process(x => total += (int)x);

            flow.Send(1);
            flow.Send(2);

            Assert.AreEqual(3, total);
        }

        [TestMethod]
        public void PostTransformWithOutput()
        {
            int result = 0;

            var flow = Flow.Create()
                .Transform(x => (int)x + 1)
                .Output(x => result = (int)x);

            flow.Post(1);

            Assert.AreEqual(2, result);
        }
    }
}
