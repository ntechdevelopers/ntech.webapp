using System.Collections.Generic;

namespace Ntech.WebApp.Models
{
    public class ManageUserRolesVM
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public IList<UserRolesVM> UserRoles { get; set; }
    }

    public class UserRolesVM
    {
        public string RoleName { get; set; }
        public bool Selected { get; set; }
    }
}