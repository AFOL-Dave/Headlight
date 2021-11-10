using System.Collections.Generic;
using Headlight.Models.Enumerations;
using Microsoft.AspNetCore.Authorization;

namespace Headlight.Services.Authorization
{
    public class RequireAnyRightRequirement: IAuthorizationRequirement
    {
        public IList<Right> RequiredRights {get; private set; }

        public RequireAnyRightRequirement(IList<Right> rights)
        {
            RequiredRights = rights;
        }
    }
}