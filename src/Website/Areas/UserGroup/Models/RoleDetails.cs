using System.Collections.Generic;

namespace Headlight.Areas.UserGroup.Models
{
    public class RoleDetails
    {
        public long Id { get; set; }

        public bool? IdDefault {get; set;}

        public string Name { get; set; }

        public List<RoleRight> Rights {get; set;}

        public List<RoleUser> Users {get; set;}
    }
}