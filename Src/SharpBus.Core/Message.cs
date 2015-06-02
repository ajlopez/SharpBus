namespace SharpBus.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Message
    {
        private MessageHeaders headers = new MessageHeaders();
        private object payload;

        public Message(object payload)
        {
            this.payload = payload;
            this.headers = new MessageHeaders();
        }

        public Message(object payload, MessageHeaders headers)
        {
            this.payload = payload;
            this.headers = headers.Clone();
        }

        public object Payload { get { return this.payload; } internal set { this.payload = value; } }

        public MessageHeaders Headers { get { return this.headers; } }

        public class MessageHeaders
        {
            private IDictionary<string, object> values = new Dictionary<string, object>();

            public object this[string name]
            {
                get
                {
                    if (this.values.ContainsKey(name))
                        return this.values[name];

                    return null;
                }

                set
                {
                    this.values[name] = value;
                }
            }

            internal MessageHeaders Clone()
            {
                MessageHeaders headers = new MessageHeaders();

                foreach (var key in this.values.Keys)
                    headers.values[key] = this.values[key];

                return headers;
            }
        }
    }
}
