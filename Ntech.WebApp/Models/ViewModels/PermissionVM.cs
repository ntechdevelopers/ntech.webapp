using System.Collections.Generic;

namespace Ntech.WebApp.Models
{
    public class PermissionVM
    {
        public string RoleId { get; set; }
        public IList<RoleClaimsVM> RoleClaims { get; set; }
    }

    public class RoleClaimsVM
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }
    }
}
