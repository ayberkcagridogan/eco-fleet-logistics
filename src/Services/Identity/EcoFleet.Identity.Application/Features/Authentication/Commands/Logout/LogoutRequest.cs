using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcoFleet.Identity.Application.Features.Authentication.Commands.Logout
{
    public record LogoutRequest(
        string RefreshToken
    );
}