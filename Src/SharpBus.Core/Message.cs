namespace SharpBus.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Message
    {
        private object payload;

        public Message(object payload)
        {
            this.payload = payload;
        }

        public object Payload { get { return this.payload; } internal set { this.payload = value; } }
    }
}
