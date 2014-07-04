namespace SharpBus.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IMessageProcessor
    {
        void Process(Message message);
    }
}
