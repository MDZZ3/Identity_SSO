using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer_SSO.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using IdentityServer_SSO.Filer;
using Microsoft.AspNetCore.Authorization;
using IdentityServer4.Services;
using IdentityServer4.Test;
using IdentityServer4.Stores;
using System.Security.Claims;

namespace IdentityServer_SSO.Controllers
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class AccountController : Controller
    {

        private readonly TestUserStore _users;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;

        public AccountController(
           IClientStore clientStore,
           IIdentityServerInteractionService interaction,
        TestUserStore users = null)
        {
            // if the TestUserStore is not in DI, then we'll just use the global users collection
            // this is where you would plug in your own custom identity management library (e.g. ASP.NET Identity)
            _users = users ?? new TestUserStore(Config.GetTestUsers());
            _interaction = interaction;
            _clientStore = clientStore;
                //  _user = user;
            //  _signInManager = signIn;

        }

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return Redirect(returnUrl);
            }
            LoginInputModel vm = new LoginInputModel()
            {
                ReturnUrl = returnUrl
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            //当登录提交给后台的model为null，则返回错误信息给前台
            if (model == null)
            {
                //这里我只是简单处理了
                return View();
            }
            //这里同理，当信息不完整的时候，返回错误信息给前台
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                //这里只是简单处理了
                return View();
            }

            //model.Username == "123" && model.Password == "123456"
            //if里面的是验证账号密码，可以用自定义的验证，
            //我这里使用的是TestUserStore的的验证方法，
            if (_users.FindByUsername(model.Username)!=null&&_users.ValidateCredentials(model.Username,model.Password))
            {
                //配置Cookie
                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(30))
                };
              
                //使用IdentityServer的SignInAsync来进行注册Cookie
                await HttpContext.SignInAsync(model.Username, model.Username);

                //使用IIdentityServerInteractionService的IsValidReturnUrl来验证ReturnUrl是否有问题
                if (_interaction.IsValidReturnUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                return View();
            }
            return View();

        }     
    }
}