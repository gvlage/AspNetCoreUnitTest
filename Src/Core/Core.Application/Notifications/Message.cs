using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Notifications
{
    public class Message
    {
        public string Code { get; set; }
        public string MessageText { get; set; }
        public MessageType Type { get; set; }
    }
}
