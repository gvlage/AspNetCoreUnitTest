using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Notifications
{
    public class Response<TData> : INotification
    {
        public Response()
        {
            Messages = new List<Message>();
        }

        public TData Data { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
