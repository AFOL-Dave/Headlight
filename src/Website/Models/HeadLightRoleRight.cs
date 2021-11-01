using Headlight.Models.Enumerations;

namespace Headlight.Models
{
    public class HeadLightRoleRight
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public RightState State {get; set;}
    }
}