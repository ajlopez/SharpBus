﻿namespace SharpBus.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IRouter
    {
        string Route(object payload);
    }
}
