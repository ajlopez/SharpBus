namespace SharpBus.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MessageTests
    {
        [TestMethod]
        public void GetUnknownHeader()
        {
            Message message = new Message(null);

            Assert.IsNull(message.Headers["Foo"]);
        }

        [TestMethod]
        public void SetGetHeader()
        {
            Message message = new Message(null);

            message.Headers["Foo"] = "Bar";

            Assert.AreEqual("Bar", message.Headers["Foo"]);
        }
    }
}
