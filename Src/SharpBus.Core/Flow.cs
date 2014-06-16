namespace SharpBus.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Flow
    {
        private IList<Func<object, object>> steps = new List<Func<object, object>>();

        private Flow()
        {
        }

        public Flow Transform(Func<object, object> transform)
        {
            this.steps.Add(transform);
            return this;
        }

        public Flow Process(Action<object> process)
        {
            this.steps.Add(x => { process(x); return x; });
            return this;
        }

        public object Send(object payload)
        {
            foreach (var step in this.steps)
                payload = step(payload);

            return payload;
        }

        public static Flow Create()
        {
            return new Flow();
        }
    }
}
