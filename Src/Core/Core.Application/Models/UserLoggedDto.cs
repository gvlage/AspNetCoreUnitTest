using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Models
{
    public class UserLoggedDto : INotification
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public string UserName { get; set; }

        public ICollection<string> Roles { get; set; }
    }
}
