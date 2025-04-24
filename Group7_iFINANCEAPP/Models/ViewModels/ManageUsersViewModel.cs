using System.Collections.Generic;

namespace Group7_iFINANCEAPP.Models.ViewModels
{
    public class ManageUsersViewModel
    {
        public List<NonAdminUser> NonAdminUsers { get; set; }
        public List<Administrator> Administrators { get; set; }
        public string ActiveTab { get; set; }
    }
}
