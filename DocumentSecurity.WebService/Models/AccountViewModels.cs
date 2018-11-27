using System;
using System.Collections.Generic;
using MyRole = DocumentSecurity.WebService.Models.RoleModal;

namespace DocumentSecurity.WebService.Models
{
    // Models returned by AccountController actions.

    public class ExternalLoginViewModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string State { get; set; }
    }

    public class ManageInfoViewModel
    {
        public string LocalLoginProvider { get; set; }

        public string Email { get; set; }

        public IEnumerable<UserLoginInfoViewModel> Logins { get; set; }

        public IEnumerable<ExternalLoginViewModel> ExternalLoginProviders { get; set; }
    }

    public class UserInfoViewModel
    {
        public string Email { get; set; }

        public bool HasRegistered { get; set; }

        public string LoginProvider { get; set; }
    }
    public class UserDetailInfoViewModel
    {
        public UserDetailInfoViewModel() { RoleNames = new List<string>(); }
        public string Email { get; set; }

        public List<string> RoleNames { get; set; }

        public string Id { get; set; }
        public string UserName { get; set; }
    }
    public class RolesInfoViewModel
    {    public RolesInfoViewModel() { Roles = new List<MyRole>(); }
      public List<MyRole> Roles { set; get; }
    }
    public class UserLoginInfoViewModel
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }
    }
}
