using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcoFleetLogistics.Application.Authentication.Commands.Logout
{
    public record LogoutRequest(
        string RefreshToken
    );
}