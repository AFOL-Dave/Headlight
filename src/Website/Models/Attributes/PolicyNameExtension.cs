using System;
using System.Linq;

namespace Headlight.Models.Attributes
{
    public static class PolicyNameExtension
    {
        public static string GetPolicyName( this Enumerations.Right right )
        {
            Type enumType = right.GetType();
            string name = Enum.GetName(enumType, right);
            PolicyNameAttribute attribute = enumType.GetField(name).GetCustomAttributes(false).OfType<PolicyNameAttribute>().SingleOrDefault();

            if (attribute != null)
            {
                return attribute.PolicyName;
            }

            return string.Empty;
        }
    }
}