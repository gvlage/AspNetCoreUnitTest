using Core.Application.Notifications;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Application.Models
{
    public class UserForLoginDto: IRequest<Response<UserLoggedDto>>
    {
        [Required]
        public string Username { get; set; }

        [StringLength(16, MinimumLength = 4, ErrorMessage = "You must specify password between 4 and 16 characters!")]
        public string Password { get; set; }
    }
}
