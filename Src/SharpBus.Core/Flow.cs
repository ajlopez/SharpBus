namespace SharpBus.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Flow
    {
        private IList<Func<Message, Message>> steps = new List<Func<Message, Message>>();

        private Flow()
        {
        }

        public static Flow Create()
        {
            return new Flow();
        }

        public Flow Transform(Func<object, object> transform)
        {
            this.steps.Add(msg => { msg.Payload = transform(msg.Payload); return msg; });
            return this;
        }

        public Flow Transform(ITransformer transformer)
        {
            return Transform(transformer.Transform);
        }

        public Flow Process(Action<object> process)
        {
            this.steps.Add(msg => { process(msg.Payload); return msg; });
            return this;
        }

        public Flow Output(Action<object> process)
        {
            this.steps.Add(msg => { process(msg.Payload); return null; });
            return this;
        }

        public object Send(object payload)
        {
            Message message = new Message(payload);

            foreach (var step in this.steps)
            {
                message = step(message);
                if (message == null)
                    break;
            }

            if (message == null)
                return null;

            return message.Payload;
        }

        public void Post(object payload)
        {
            Message message = new Message(payload);

            foreach (var step in this.steps)
            {
                message = step(message);
                if (message == null)
                    break;
            }
        }
    }
}
