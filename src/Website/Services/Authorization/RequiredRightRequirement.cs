using Headlight.Models.Enumerations;
using Microsoft.AspNetCore.Authorization;

namespace Headlight.Services.Authorization
{
    public class RequiredRightRequirement : IAuthorizationRequirement
    {
        public Right RequiredRight { get; }

        public RequiredRightRequirement(Right right)
        {
            RequiredRight = right;
        }
    }
}