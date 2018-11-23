using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Threading.Tasks;
using MyUser = DocumentSecurity.WebService.Models.UserModal;
using MyRole = DocumentSecurity.WebService.Models.RoleModal;
using MyAccountDataAccess = DocumentSecurity.WebService.DataAccess.AccountDataAccess;
using MyEncryption = DocumentSecurity.WebService.Providers.EncryptionProvider;
using System.Web.Http.Cors;

namespace DocumentSecurity.WebService.Providers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated(); //   
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            MyUser user = new MyAccountDataAccess().GetUserDetailByUserName(context.UserName);

            if (user != null)
            {

                if (user.UserName == context.UserName && user.PasswordHash == MyEncryption.Encrypt(context.Password))
                { 
                    foreach (var r in user.Roles)
                    {
                        identity.AddClaim(new Claim("Role",r.RoleName));
                    }

                    var props = new AuthenticationProperties(new Dictionary<string, string>
                            {
                                {
                                    "userdisplayname", context.UserName
                                },
                                {
                                     "Email", user.Email 
                                }
                             });

                    var ticket = new AuthenticationTicket(identity, props);
                    context.Validated(ticket);
                }
                else
                {
                    context.SetError("invalid_grant", "Provided username and password is incorrect");
                    context.Rejected();
                }

            }
            else
            {
                context.SetError("invalid_grant", "Provided username and password is incorrect");
                context.Rejected();
            }
            return;

        }
    }
}