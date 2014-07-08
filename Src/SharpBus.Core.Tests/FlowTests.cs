namespace SharpBus.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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
        public void SendPayloadToFlowWithOutputObject()
        {
            AccumulatorOutput output = new AccumulatorOutput();

            var flow = Flow.Create()
                .Output(output);

            flow.Post(1);
            flow.Post(2);

            Assert.AreEqual(3, output.Total);
        }

        [TestMethod]
        public void ApplyTransformToInteger()
        {
            var flow = Flow.Create().Transform(x => ((int)x) + 1);

            Assert.AreEqual(2, flow.Send(1));
        }

        [TestMethod]
        public void ApplyTransformer()
        {
            var flow = Flow.Create().Transform(new IncrementTransformer());

            Assert.AreEqual(2, flow.Send(1));
        }

        [TestMethod]
        public void ApplyMessageTransformer()
        {
            var flow = Flow.Create().Transform(new IncrementMessageTransformer());

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
        public void ApplyProcessor()
        {
            AccumulatorProcessor processor = new AccumulatorProcessor();

            var flow = Flow.Create().Process(processor);

            flow.Send(1);
            flow.Send(2);

            Assert.AreEqual(3, processor.Total);
        }

        [TestMethod]
        public void ApplyMessageProcessor()
        {
            AccumulatorMessageProcessor processor = new AccumulatorMessageProcessor();

            var flow = Flow.Create().Process(processor);

            flow.Send(1);
            flow.Send(2);

            Assert.AreEqual(3, processor.Total);
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

        [TestMethod]
        public void InputsAndStart()
        {
            int result = 0;

            var flow = Flow.Create()
                .Input(fl => fl.Post(1))
                .Input(fl => fl.Post(2))
                .Process(x => result += (int)x);

            flow.Start();

            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void InputObjectAndStart()
        {
            int result = 0;

            var flow = Flow.Create()
                .Input(new IntegerGeneratorInput(3))
                .Process(x => result += (int)x);

            flow.Start();

            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void RouterWithTwoBranches()
        {
            var flow = Flow.Create()
                .Route(x => { return (int)x % 2 == 0 ? "Even" : "Odd"; })
                .Branch("Even")
                    .Transform(x => (int)x / 2)
                .EndBranch()
                .Branch("Odd")
                    .Transform(x => ((int)x * 3) + 1)
                .EndBranch();

            flow.Post(1);

            Assert.AreEqual(0, flow.Send(0));
            Assert.AreEqual(4, flow.Send(1));
            Assert.AreEqual(1, flow.Send(2));
            Assert.AreEqual(10, flow.Send(3));
        }

        [TestMethod]
        public void RouterObjectWithTwoBranches()
        {
            var flow = Flow.Create()
                .Route(new EvenOddRouter())
                .Branch("Even")
                    .Transform(x => (int)x / 2)
                .EndBranch()
                .Branch("Odd")
                    .Transform(x => ((int)x * 3) + 1)
                .EndBranch();

            flow.Post(1);

            Assert.AreEqual(0, flow.Send(0));
            Assert.AreEqual(4, flow.Send(1));
            Assert.AreEqual(1, flow.Send(2));
            Assert.AreEqual(10, flow.Send(3));
        }

        private class IncrementTransformer : ITransformer
        {
            public object Transform(object payload)
            {
                return (int)payload + 1;
            }
        }

        private class IncrementMessageTransformer : IMessageTransformer
        {
            public Message Transform(Message message)
            {
                return new Message((int)message.Payload + 1);
            }
        }

        private class AccumulatorProcessor : IProcessor
        {
            private int total = 0;

            public int Total { get { return this.total; } }

            public void Process(object payload)
            {
                this.total += (int)payload;
            }
        }

        private class AccumulatorMessageProcessor : IMessageProcessor
        {
            private int total = 0;

            public int Total { get { return this.total; } }

            public void Process(Message message)
            {
                this.total += (int)message.Payload;
            }
        }

        private class AccumulatorOutput : IOutput
        {
            private int total = 0;

            public int Total { get { return this.total; } }

            public void Consume(object payload)
            {
                this.total += (int)payload;
            }
        }

        private class IntegerGeneratorInput : IInput
        {
            private int number;

            public IntegerGeneratorInput(int number)
            {
                this.number = number;
            }

            public void Start(Flow flow)
            {
                for (int k = 1; k <= this.number; k++)
                    flow.Post(k);
            }
        }

        private class EvenOddRouter : IRouter
        {
            public string Route(object payload)
            {
                return ((int)payload) % 2 == 0 ? "Even" : "Odd";
            }
        }
    }
}
