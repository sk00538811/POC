using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using DocumentSecurity.WebService.Models;
using DocumentSecurity.WebService.Providers;
using DocumentSecurity.WebService.Results;
using System.Linq;
using MyUserManager = DocumentSecurity.WebService.DataAccess.AccountDataAccess;
using MyUser = DocumentSecurity.WebService.Models.UserModal;
using MyUserRoles = DocumentSecurity.WebService.Models.UserRoleModal;
using MyRole = DocumentSecurity.WebService.Models.RoleModal;
using MyEncryption = DocumentSecurity.WebService.Providers.EncryptionProvider;
namespace DocumentSecurity.WebService.Controllers
{
    // [Authorize(Roles = "Admin")] 
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
        ///...
        private ApplicationRoleManager _roleManager;
        ///...
        public AccountController()
        {
        }
        public AccountController(ApplicationUserManager userManager,
        ISecureDataFormat<AuthenticationTicket> accessTokenFormat, ApplicationRoleManager roleManager)
        {
            ///Make an instance of the user manager in the controller to avoid null reference exception
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
            ///Make an instance of the role manager in the constructor to avoid null reference exception
            RoleManager = roleManager;
        }
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        ///...
        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        
        // GET api/Account/CurrentUser
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Authorize(Roles = "Admin")]
        [Route("CurrentUser")]
        public UserDetailInfoViewModel GetCurrentUserDetail()
        {
            CurrentLoginData currentLogin = CurrentLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserDetailInfoViewModel
            {
                RoleNames = currentLogin != null ? currentLogin.RoleNames : null,
                Email = currentLogin != null ? currentLogin.Email : null,
                Id = currentLogin != null ? currentLogin.Id : null,
                UserName = currentLogin != null ? currentLogin.UserName : null
            };
        }
        // GET api/Account/GetAllRoles
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("GetAllRoles")]
        public RolesInfoViewModel GetAllRoles()
        {
            RolesInfoViewModel ol = new RolesInfoViewModel();
            try
            {
                ol.Roles = new MyUserManager().GetAllRoles(); //await UserManager.CreateAsync(user, model.Password);

            }
            catch (Exception ex) { ModelState.AddModelError("Unable to fetch roles.", ex.Message); }
            if (ol.Roles.Count <= 0)
            {
                ModelState.AddModelError("", "No role exists.");
            }

            return ol;


        }
        // POST api/Account/Register
        // [AllowAnonymous]
        [Authorize(Roles = "SuperAdmin")]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
            //set the IsDeleted property to false
            user.IsDeleted = false;
            UserModal ouser = new UserModal();
            ouser.Id = user.Id;
            ouser.Email = user.Email;
            ouser.UserName = user.UserName;
            ouser.PasswordHash = MyEncryption.Encrypt(model.Password);
            bool flag = false;
            try
            {
                flag = new MyUserManager().CreateUser(ouser); //await UserManager.CreateAsync(user, model.Password);
            }
            catch (Exception ex) { ModelState.AddModelError("", ex.Message); }
            if (!flag)
            {
                ModelState.AddModelError("", "Failed to create user");
                return BadRequest(ModelState);
            }

            return Ok();
        }
        // POST api/Account/users/{id:guid}/roles 
        [AllowAnonymous]
        [Route("users/{id:guid}/roles")]
        [HttpPut]
        public async Task<IHttpActionResult> AssignRolesToUser(string id, string[] rolesToAssign)
        {
            if (rolesToAssign == null)
            {
                return this.BadRequest("No roles specified");
            }

            ///find the user we want to assign roles to
            MyUser appUser = new MyUserManager().GetUserDetailById(id); //await this.UserManager.FindByIdAsync(id);

            if (appUser == null || (appUser.IsDeleted ?? false))
            {
                return NotFound();
            }

            ///check if the user currently has any roles
            List<MyUserRoles> currentRoles = appUser.Roles;// await this.UserManager.GetRolesAsync(appUser.Id);
            List<MyRole> allRoles = new MyUserManager().GetAllRoles();// await this.UserManager.GetRolesAsync(appUser.Id);
            if (allRoles == null || allRoles.Count == 0)
                return NotFound();


            List<MyRole> tempRoles = allRoles.Where(n => !currentRoles.Any(x => x.RoleName.Contains(n.RoleName))).Select(x => new MyRole { RoleId = x.RoleId, RoleName = x.RoleName }).ToList();

            List<MyRole> FinalrolesToAssign = tempRoles.Where(n => rolesToAssign.Contains(n.RoleName)).Select(x => new MyRole { RoleId = x.RoleId, RoleName = x.RoleName }).ToList();

            if (FinalrolesToAssign.Count() == 0)
            {
                ModelState.AddModelError("", string.Format("Roles '{0}' does not exist in the system", string.Join(",", FinalrolesToAssign)));
                return this.BadRequest(ModelState);
            }

            ///remove user from current roles, if any
            // bool flag = new MyUserManager().RemoveUserRoles (appUser.Id, currentRole  );
            string strRoles = string.Empty;
            foreach (MyRole r in FinalrolesToAssign)
            {
                try
                {  ///assign user to the new roles
                    if (!new MyUserManager().AddUserRoles(appUser.Id, r.RoleId))
                        strRoles += r.RoleName + ", ";
                }
                catch
                {
                    strRoles += r.RoleName + ", ";
                }

            }
            if (!string.IsNullOrEmpty(strRoles))
            {
                ModelState.AddModelError("", "Failed to add " + strRoles + " user roles");
                return BadRequest(ModelState);
            }

            return Ok(new { userId = id, rolesAssigned = FinalrolesToAssign.Select(x => x.RoleName).ToArray() });
        }
        // POST api/Account/users/{id:guid}/roles 
        [AllowAnonymous]
        [Route("users/{id:guid}/removeroles")]
        [HttpPut]
        public async Task<IHttpActionResult> RemoveRolesFromUser(string id, string[] rolesToRemove)
        {
            if (rolesToRemove == null)
            {
                return this.BadRequest("No roles specified");
            }

            ///find the user we want to assign roles to
            MyUser appUser = new MyUserManager().GetUserDetailById(id); //await this.UserManager.FindByIdAsync(id);

            if (appUser == null || (appUser.IsDeleted ?? false))
            {
                return NotFound();
            }

            ///check if the user currently has any roles
            List<MyUserRoles> currentRoles = appUser.Roles;// await this.UserManager.GetRolesAsync(appUser.Id);
            List<MyRole> allRoles = new MyUserManager().GetAllRoles();// await this.UserManager.GetRolesAsync(appUser.Id);
            if (allRoles == null || allRoles.Count == 0)
                return NotFound();


            List<MyRole> tempRoles = allRoles.Where(n => currentRoles.Any(x => x.RoleName.Contains(n.RoleName))).Select(x => new MyRole { RoleId = x.RoleId, RoleName = x.RoleName }).ToList();

            List<MyRole> FinalrolesToRemove = tempRoles.Where(n => rolesToRemove.Contains(n.RoleName)).Select(x => new MyRole { RoleId = x.RoleId, RoleName = x.RoleName }).ToList();

            if (FinalrolesToRemove.Count() == 0)
            {
                ModelState.AddModelError("", string.Format("Roles '{0}' does not exist in the system", string.Join(",", FinalrolesToRemove)));
                return this.BadRequest(ModelState);
            }

            ///remove user from current roles, if any
            // bool flag = new MyUserManager().RemoveUserRoles (appUser.Id, currentRole  );
            string strRoles = string.Empty;
            foreach (MyRole r in FinalrolesToRemove)
            {
                try
                {  ///remove role from user  
                    if (!new MyUserManager().RemoveUserRoles(appUser.Id, r.RoleId))
                        strRoles += r.RoleName + ", ";
                }
                catch
                {
                    strRoles += r.RoleName + ", ";
                }

            }
            if (!string.IsNullOrEmpty(strRoles))
            {
                ModelState.AddModelError("", "Failed to remove " + strRoles + " user roles");
                return BadRequest(ModelState);
            }

            return Ok(new { userId = id, rolesRemoved = FinalrolesToRemove.Select(x => x.RoleName).ToArray() });
        }

        // DELETE api/Account/DeleteUser/{id:guid}  
        // [AllowAnonymous]
        [OverrideAuthorization]
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete]
        [Route("DeleteUser/{id:guid}")]
        public IHttpActionResult DeleteUser(string id)
        {
            //check if such a user exists in the database
            var userToDelete = new MyUserManager().GetUserDetailById(id); //this.UserManager.FindById(id);
            if (userToDelete == null)
            {
                return this.NotFound();
            }
            else if (userToDelete.IsDeleted ?? false)
            {
                return this.BadRequest("User already deleted");
            }
            else
            {
                try
                {
                    if (!new MyUserManager().DeleteUser(userToDelete))
                    {
                        return this.BadRequest("Unable to  delete user. Please contact administrator");
                    }
                }
                catch (Exception ex) { return this.BadRequest(ex.Message); }

            }
            return this.Ok();
        }


        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            CurrentLoginData currentLogin = CurrentLoginData.FromIdentity(User.Identity as ClaimsIdentity);
            if (currentLogin == null)
                return NotFound();

            if (model.NewPassword != model.ConfirmPassword || model.OldPassword == model.NewPassword)
            {
                ModelState.AddModelError("", "New and confirm password should be same and it should not matched with old passwrod");
                return BadRequest(ModelState);
            }
            else
            {
                MyUser ouser = new MyUser();
                ouser.Id = currentLogin.Id;
                ouser.Email = currentLogin.Email;
                ouser.PasswordHash = MyEncryption.Encrypt(model.NewPassword);

                bool flag = false;
                try
                {
                    flag = new MyUserManager().ChangeUserPassword(ouser); //await UserManager.CreateAsync(user, model.Password);
                }
                catch (Exception ex) { ModelState.AddModelError("", ex.Message); }
                if (!flag)
                {
                    ModelState.AddModelError("", "Failed to change user password");
                    return BadRequest(ModelState);
                }
            }
            return Ok();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CurrentLoginData currentLogin = CurrentLoginData.FromIdentity(User.Identity as ClaimsIdentity);
            if (currentLogin == null)
                return NotFound();

            if (model.NewPassword != model.ConfirmPassword  )
            {
                ModelState.AddModelError("", "New and confirm password should be same.");
                return BadRequest(ModelState);
            }
            else
            {
                MyUser ouser = new MyUser();
                ouser.Id = currentLogin.Id;
                ouser.Email = currentLogin.Email;
                ouser.PasswordHash = MyEncryption.Encrypt(model.NewPassword);

                bool flag = false;
                try
                {
                    flag = new MyUserManager().ChangeUserPassword(ouser); //await UserManager.CreateAsync(user, model.Password);
                }
                catch (Exception ex) { ModelState.AddModelError("", ex.Message); }
                if (!flag)
                {
                    ModelState.AddModelError("", "Failed to set user password");
                    return BadRequest(ModelState);
                }
            }
            return Ok();
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }
        #region extra service not in use
        /*
         // GET api/Account/UserInfo
         [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
         [Authorize]
         [Route("UserInfo")]
         public UserInfoViewModel GetUserInfo()
         {
             ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

             return new UserInfoViewModel
             {
                 Email = User.Identity.GetUserName(),
                 HasRegistered = externalLogin == null,
                 LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
             };
         }
         // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
         [Route("ManageInfo")]
         public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
         {
             IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

             if (user == null)
             {
                 return null;
             }

             List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

             foreach (IdentityUserLogin linkedAccount in user.Logins)
             {
                 logins.Add(new UserLoginInfoViewModel
                 {
                     LoginProvider = linkedAccount.LoginProvider,
                     ProviderKey = linkedAccount.ProviderKey
                 });
             }

             if (user.PasswordHash != null)
             {
                 logins.Add(new UserLoginInfoViewModel
                 {
                     LoginProvider = LocalLoginProvider,
                     ProviderKey = user.UserName,
                 });
             }

             return new ManageInfoViewModel
             {
                 LocalLoginProvider = LocalLoginProvider,
                 Email = user.UserName,
                 Logins = logins,
                 ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
             };
         }

         // POST api/Account/AddExternalLogin
         [Route("AddExternalLogin")]
         public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
         {
             if (!ModelState.IsValid)
             {
                 return BadRequest(ModelState);
             }

             Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

             AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

             if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                 && ticket.Properties.ExpiresUtc.HasValue
                 && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
             {
                 return BadRequest("External login failure.");
             }

             ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

             if (externalData == null)
             {
                 return BadRequest("The external login is already associated with an account.");
             }

             IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                 new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

             if (!result.Succeeded)
             {
                 return GetErrorResult(result);
             }

             return Ok();
         }

         // POST api/Account/RemoveLogin
         [Route("RemoveLogin")]
         public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
         {
             if (!ModelState.IsValid)
             {
                 return BadRequest(ModelState);
             }

             IdentityResult result;

             if (model.LoginProvider == LocalLoginProvider)
             {
                 result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
             }
             else
             {
                 result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                     new UserLoginInfo(model.LoginProvider, model.ProviderKey));
             }

             if (!result.Succeeded)
             {
                 return GetErrorResult(result);
             }

             return Ok();
         }

         // GET api/Account/ExternalLogin
         [OverrideAuthentication]
         [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
         [AllowAnonymous]
         [Route("ExternalLogin", Name = "ExternalLogin")]
         public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
         {
             if (error != null)
             {
                 return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
             }

             if (!User.Identity.IsAuthenticated)
             {
                 return new ChallengeResult(provider, this);
             }

             ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

             if (externalLogin == null)
             {
                 return InternalServerError();
             }

             if (externalLogin.LoginProvider != provider)
             {
                 Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                 return new ChallengeResult(provider, this);
             }

             ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                 externalLogin.ProviderKey));

             bool hasRegistered = user != null;

             if (hasRegistered)
             {
                 Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                 ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    OAuthDefaults.AuthenticationType);
                 ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                     CookieAuthenticationDefaults.AuthenticationType);

                 AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                 Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
             }
             else
             {
                 IEnumerable<Claim> claims = externalLogin.GetClaims();
                 ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                 Authentication.SignIn(identity);
             }

             return Ok();
         }

         // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
         [AllowAnonymous]
         [Route("ExternalLogins")]
         public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
         {
             IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
             List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

             string state;

             if (generateState)
             {
                 const int strengthInBits = 256;
                 state = RandomOAuthStateGenerator.Generate(strengthInBits);
             }
             else
             {
                 state = null;
             }

             foreach (AuthenticationDescription description in descriptions)
             {
                 ExternalLoginViewModel login = new ExternalLoginViewModel
                 {
                     Name = description.Caption,
                     Url = Url.Route("ExternalLogin", new
                     {
                         provider = description.AuthenticationType,
                         response_type = "token",
                         client_id = Startup.PublicClientId,
                         redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                         state = state
                     }),
                     State = state
                 };
                 logins.Add(login);
             }

             return logins;
         }

         // POST api/Account/RegisterExternal
         [OverrideAuthentication]
         [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
         [Route("RegisterExternal")]
         public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
         {
             if (!ModelState.IsValid)
             {
                 return BadRequest(ModelState);
             }

             var info = await Authentication.GetExternalLoginInfoAsync();
             if (info == null)
             {
                 return InternalServerError();
             }

             var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

             IdentityResult result = await UserManager.CreateAsync(user);
             if (!result.Succeeded)
             {
                 return GetErrorResult(result);
             }

             result = await UserManager.AddLoginAsync(user.Id, info.Login);
             if (!result.Succeeded)
             {
                 return GetErrorResult(result);
             }
             return Ok();
         }
 */
        #endregion
        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }
        private class CurrentLoginData
        {
            public CurrentLoginData() { RoleNames = new List<string>(); }
            public string Id { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public List<string> RoleNames { get; set; }

            public static CurrentLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                List<string> roles = identity.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();

                string username = identity.Claims.Where(x => x.Type == ClaimTypes.Name).Select(x => x.Value).SingleOrDefault();
                string email = identity.Claims.Where(x => x.Type == ClaimTypes.Email).Select(x => x.Value).SingleOrDefault();
                string id = identity.Claims.Where(x => x.Type == ClaimTypes.Sid).Select(x => x.Value).SingleOrDefault();
                return new CurrentLoginData
                {
                    RoleNames = roles,
                    Email = email,
                    Id = id,
                    UserName = username
                };
            }
        }
        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
