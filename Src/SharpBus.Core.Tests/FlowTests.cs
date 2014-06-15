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
        public void ApplyTransformToInteger()
        {
            var flow = Flow.Create().Transform(x => ((int)x) + 1);

            Assert.AreEqual(2, flow.Send(1));
        }
    }
}
