using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Headlight.Models;
using Headlight.Models.Enumerations;
using Headlight.Models.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Headlight.Services.Authorization
{
    public class RequiredRightHandler : AuthorizationHandler<RequiredRightRequirement>
    {
        public RequiredRightHandler( HeadLightRoleStore roleStore )
        {
            _roleStore = roleStore;
        }

        protected async override Task HandleRequirementAsync( AuthorizationHandlerContext context,
                                                              RequiredRightRequirement requirement )
        {
            IList<HeadLightRoleRight> rightStates = await _roleStore.RetrieveRightByRightIdUserIdAsync((long)requirement.RequiredRight, context.User.GetUserId());

            if (rightStates.Any(r => r.State == RightState.Denied))
            {
                return;
            }

            if (rightStates.Any(r => r.State == RightState.Granted))
            {
                context.Succeed(requirement);
            }

            return;
        }

        private readonly HeadLightRoleStore _roleStore;
    }
}