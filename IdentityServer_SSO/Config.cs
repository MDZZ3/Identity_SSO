using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer_SSO
{
    public class Config
    {

        public static IEnumerable<ApiResource> GetApiResource()
        {
            return new List<ApiResource>()
            {
                new ApiResource("api","this is api",new List<string>{ClaimTypes.Role })
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>()
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Phone()
            };
        }

        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>{
                new TestUser()
                {
                    SubjectId="123",
                    Username="Mr.wen",
                    Password="123465",
                    Claims=new Claim[]
                    {
                        new Claim(ClaimTypes.Role,"管理员")
                    }

                },
                new TestUser()
                {
                    SubjectId="456",
                    Username="123",
                    Password="123456",
                    Claims=new Claim[]
                    {
                        new Claim(ClaimTypes.Role,"阅览者")
                    }
                    
                }

            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>()
            {
                new Client()
                {
                    ClientId="mvc_imp",
                    ClientName="Mvc_Name",
                    AllowedGrantTypes=GrantTypes.Implicit,
                    ////设置是否要授权
                    RequireConsent=false,

                    RedirectUris={ "http://www.b.net:5001/signin-oidc","http://www.a.cn:5002/signin-oidc" },

                    PostLogoutRedirectUris={ "http://www.b.net:5001/signout-callback-oidc","http://www.a.cn:5002/signout-callback-oidc" },
                    ClientSecrets={new Secret("secret".Sha256())},
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        
                    },
                    AllowOfflineAccess=true
                }

            };
        }
    }
}
