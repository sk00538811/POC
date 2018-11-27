using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocumentSecurity.WebService.Models
{
    public class UserModal
    {
        public UserModal() { Roles = new List<UserRoleModal>(); }
        public static class DBObject
        {

        }
        public static class DataTables
        {
            public static string dtUser = "dtUser";

        }
        public class Fields
        {
            public static string Id = "Id";
            public static string Email = "Email";
            public static string EmailConfirmed = "EmailConfirmed";
            public static string PasswordHash = "PasswordHash";
            public static string SecurityStamp = "SecurityStamp";
            public static string PhoneNumber = "PhoneNumber";
            public static string PhoneNumberConfirmed = "PhoneNumberConfirmed";
            public static string TwoFactorEnabled = "TwoFactorEnabled";
            public static string LockoutEndDateUtc = "LockoutEndDateUtc";
            public static string LockoutEnabled = "LockoutEnabled";
            public static string AccessFailedCount = "AccessFailedCount";
            public static string UserName = "UserName";
            public static string IsDeleted = "IsDeleted";
        }
        #region property
        public string Id { get; set; }
        public string Email { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        public bool? TwoFactorEnabled { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public bool? LockoutEnabled { get; set; }
        public int? AccessFailedCount { get; set; }
        public string UserName { get; set; }
        public bool? IsDeleted { get; set; }

        public List<UserRoleModal> Roles { get; set; }

        #endregion
    }
    public class UserRoleModal
    {
        public static class DBObject
        {
        }
        public static class DataTables
        {
            public static string dtUserRoles = "dtUserRoles";
        }
        public class Fields
        {
            public static string UserId = "UserId";
            public static string RoleId = "RoleId";
            public static string RoleName = "RoleName";
        }
        #region property
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        #endregion
    }
    public class RoleModal
    {
        public static class DBObject
        {
        }
        public static class DataTables
        {
            public static string dtRoles = "dtRoles";
        }
        public class Fields
        {
             public static string RoleId = "RoleId";
            public static string RoleName = "RoleName";
        }
        #region property
         public string RoleId { get; set; }
        public string RoleName { get; set; }
        #endregion
    }
}