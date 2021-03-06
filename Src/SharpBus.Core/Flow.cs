﻿namespace SharpBus.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Flow
    {
        private Flow parent;
        private IList<Func<Message, Message>> steps = new List<Func<Message, Message>>();
        private IDictionary<string, Flow> branches = new Dictionary<string, Flow>();
        private IList<Action<Flow>> inputs = new List<Action<Flow>>();

        private Flow()
        {
        }

        private Flow(Flow parent)
        {
            this.parent = parent;
        }

        public static Flow Create()
        {
            return new Flow();
        }

        public Flow Input(Action<Flow> input)
        {
            this.inputs.Add(input);
            return this;
        }

        public Flow Input(IInput input)
        {
            return this.Input(input.Start);
        }

        public void Start()
        {
            foreach (var input in this.inputs)
                input(this);
        }

        public Flow Transform(Func<object, object> transform)
        {
            this.steps.Add(msg => { msg.Payload = transform(msg.Payload); return msg; });
            return this;
        }

        public Flow Transform(ITransformer transformer)
        {
            return this.Transform(transformer.Transform);
        }

        public Flow Transform(IMessageTransformer transformer)
        {
            this.steps.Add(msg => transformer.Transform(msg));
            return this;
        }

        public Flow Process(Action<object> process)
        {
            this.steps.Add(msg => { process(msg.Payload); return msg; });
            return this;
        }

        public Flow Process(IProcessor processor)
        {
            return this.Process(processor.Process);
        }

        public Flow Process(IMessageProcessor processor)
        {
            this.steps.Add(msg => { processor.Process(msg); return msg; });
            return this;
        }

        public Flow Route(Func<object, string> router)
        {
            this.Transform(payload =>
            {
                var branchname = router(payload);
                return this.SendToBranch(branchname, payload);
            });

            return this;
        }

        public Flow Route(IRouter router)
        {
            return this.Route(router.Route);
        }

        public Flow Route(IMessageRouter router)
        {
            this.steps.Add(message =>
            {
                var branchname = router.Route(message);
                return this.SendToBranch(branchname, message);
            });

            return this;
        }

        public Flow Output(Action<object> process)
        {
            this.steps.Add(msg => { process(msg.Payload); return null; });
            return this;
        }

        public Flow Output(IOutput output)
        {
            return this.Output(output.Consume);
        }

        public Flow Branch(string branchname)
        {
            Flow flow = new Flow(this);
            this.branches[branchname] = flow;
            return flow;
        }

        public Flow EndBranch()
        {
            return this.parent;
        }

        public object Send(object payload)
        {
            Message message = new Message(payload);

            var result = this.Send(message);

            if (result == null)
                return null;

            return result.Payload;
        }

        public Message Send(Message message)
        {
            foreach (var step in this.steps)
            {
                message = step(message);
                if (message == null)
                    break;
            }

            if (message == null)
                return null;

            return message;
        }

        public void Post(object payload)
        {
            Message message = new Message(payload);

            this.Post(message);
        }

        public void Post(Message message)
        {
            foreach (var step in this.steps)
            {
                message = step(message);
                if (message == null)
                    break;
            }
        }

        private object SendToBranch(string branchname, object payload)
        {
            return this.branches[branchname].Send(payload);
        }

        private Message SendToBranch(string branchname, Message message)
        {
            return this.branches[branchname].Send(message);
        }
    }
}
