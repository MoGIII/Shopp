﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopp.MessageBus
{
    public interface IMessageBus
    {
        Task PublishMessage(object message, string topic_queue_Name);
    }
}
