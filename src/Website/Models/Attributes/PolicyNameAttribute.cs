using System;

namespace Headlight.Models.Attributes
{
    [System.AttributeUsage(AttributeTargets.Field)]
    public class PolicyNameAttribute : Attribute
    {
        public string PolicyName { get; private set;}

        public PolicyNameAttribute(string policyName)
        {
            PolicyName = policyName;
        }
    }
}