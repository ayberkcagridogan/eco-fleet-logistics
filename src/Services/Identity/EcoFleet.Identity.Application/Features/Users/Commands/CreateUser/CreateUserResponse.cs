using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcoFleet.Identity.Application.Features.Users.Commands.CreateUser
{
    public record CreateUserResponse(Guid Id, string Email, string Role);
}