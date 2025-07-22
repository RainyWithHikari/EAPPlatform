using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms;
using System.Collections.Generic;
using EAPPlatform.ViewModel.HomeVMs;
using System.Linq;
using WalkingTec.Mvvm.Core.Support.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace EAPPlatform.Controllers
{
    [AllRights]
    public class LoginController : BaseController
    {

        [Public]
        [ActionDescription("Login")]
        public IActionResult Login()
        {
            LoginVM vm = Wtm.CreateVM<LoginVM>();
    //        HttpContext.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
    //{
    //    { "ReturnUrl", Url.Action("Login", "Login") }
    //});
            vm.Redirect = HttpContext.Request.Query["ReturnUrl"];
            if (Wtm.ConfigInfo.IsQuickDebug == true)
            {
                vm.ITCode = "admin";
                vm.Password = "000000";
            }
            return View(vm);
        }

        [Public]
        [HttpPost]
        public async Task<ActionResult> Login(LoginVM vm)
        {
            //if (Wtm.ConfigInfo.IsQuickDebug == true)
            //{
            //    var verifyCode = HttpContext.Session.Get<string>("verify_code");
            //    if (string.IsNullOrEmpty(verifyCode) || verifyCode.ToLower() != vm.VerifyCode.ToLower())
            //    {
            //        vm.MSD.AddModelError("", Localizer["Login.ValidationFail"]);
            //        return View(vm);
            //    }
            //}
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("account", vm.ITCode);
            data.Add("password", vm.Password);
            data.Add("withmenu", "false");
            //Request.Host.ToString();
            // General:

            var user = await Wtm.CallAPI<LoginUserInfo>("", Request.Scheme + "://" + Request.Host.ToString() + "/api/_account/login", HttpMethodEnum.POST, data, 2);

            if (user?.Data == null)
            {
                // SX:
                user = await Wtm.CallAPI<LoginUserInfo>("", "Http" + "://" + "localhost:" + "80" + "/api/_account/login", HttpMethodEnum.POST, data);
                //JQ:
                //user = await Wtm.CallAPI<LoginUserInfo>("", "Http" + "://" + "10.5.1.223:" + "80" + "/api/_account/login", HttpMethodEnum.POST, data);
                //GDL:
                //user = await Wtm.CallAPI<LoginUserInfo>("", Request.Scheme + "://" + Request.Host.ToString() + "/eapweb/api/_account/login", HttpMethodEnum.POST, data);
                //var user = await Wtm.CallAPI<LoginUserInfo>("", "Http" + "://" + "localhost:" + "81" + "/api/_account/login", HttpMethodEnum.POST, data);
                if (Wtm.ConfigInfo.IsQuickDebug == true)
                {
                    user = await Wtm.CallAPI<LoginUserInfo>("", Request.Scheme + "://" + Request.Host.ToString() + "/api/_account/login", HttpMethodEnum.POST, data);
                }
            }



            if (user?.Data == null)
            {

                vm.MSD.AddModelError("", Localizer["Sys.LoginFailed"]);
                return View(vm);
            }
            else
            {
                Wtm.LoginUserInfo = user.Data;
                string url = string.Empty;
                if (!string.IsNullOrEmpty(vm.Redirect))
                {
                    url = vm.Redirect;
                }
                else
                {
                    //url = "/eapweb/";
                    url = "/";
                }

                AuthenticationProperties properties = null;
                if (vm.RememberLogin)
                {
                    properties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(30))
                    };
                }

                var principal = user.Data.CreatePrincipal();
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
                return Redirect(HttpUtility.UrlDecode(url));
            }
        }

        public async Task<IActionResult> LoginTest([FromForm] string account, [FromForm] string password, [FromForm] bool rememberLogin = false, [FromForm] bool withMenu = true)
        {

            var rv = await DC.Set<FrameworkUser>().Where(x => x.ITCode.ToLower() == account.ToLower() && (x.Password == Utils.GetMD5String(password) || x.Password == password) && x.IsValid).Select(x => new { itcode = x.ITCode, id = x.GetID() }).SingleOrDefaultAsync();

            if (rv == null)
            {
                return BadRequest(Localizer["Sys.LoginFailed"].Value);
            }
            LoginUserInfo user = new LoginUserInfo
            {
                ITCode = rv.itcode,
                UserId = rv.id.ToString()
            };

            await user.LoadBasicInfoAsync(Wtm);

            //其他属性可以通过user.Attributes["aaa"] = "bbb"方式赋值

            Wtm.LoginUserInfo = user;

            AuthenticationProperties properties = null;
            if (rememberLogin)
            {
                properties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(30))
                };
            }

            var principal = Wtm.LoginUserInfo.CreatePrincipal();
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
            if (withMenu == false)
            {
                return Ok(user);
            }
            else
            {
                List<SimpleMenuApi> ms = new List<SimpleMenuApi>();
                LoginUserInfo forapi = new LoginUserInfo();
                forapi.UserId = user.UserId;
                forapi.ITCode = user.ITCode;
                forapi.Name = user.Name;
                forapi.Roles = user.Roles;
                forapi.Groups = user.Groups;
                forapi.PhotoId = user.PhotoId;
                var roleIDs = Wtm.LoginUserInfo.Roles.Select(x => x.RoleCode).ToList();
                var data = DC.Set<FrameworkMenu>().Where(x => string.IsNullOrEmpty(x.MethodName)).ToList();
                var topdata = data.Where(x => x.ParentId == null && x.ShowOnMenu).ToList().FlatTree(x => x.DisplayOrder).Where(x => (x.IsInside == false || x.FolderOnly == true || string.IsNullOrEmpty(x.MethodName)) && x.ShowOnMenu).ToList();
                var allowed = DC.Set<FunctionPrivilege>()
                                .AsNoTracking()
                                .Where(x => x.RoleCode != null && roleIDs.Contains(x.RoleCode))
                                .Select(x => new { x.MenuItem.ID, x.MenuItem.Url })
                                .ToList();

                var allowedids = allowed.Select(x => x.ID).ToList();
                foreach (var item in topdata)
                {
                    if (allowedids.Contains(item.ID))
                    {
                        ms.Add(new SimpleMenuApi
                        {
                            Id = item.ID.ToString().ToLower(),
                            ParentId = item.ParentId?.ToString()?.ToLower(),
                            Text = item.PageName,
                            Url = item.Url,
                            Icon = item.Icon
                        });
                    }
                }

                // LocalizeMenu(ms);

                List<string> urls = new List<string>();
                urls.AddRange(allowed.Select(x => x.Url).Distinct());
                urls.AddRange(GlobaInfo.AllModule.Where(x => x.IsApi == true).SelectMany(x => x.Actions).Where(x => (x.IgnorePrivillege == true || x.Module.IgnorePrivillege == true) && x.Url != null).Select(x => x.Url));
                forapi.Attributes = new Dictionary<string, object>();
                forapi.Attributes.Add("Menus", ms);
                forapi.Attributes.Add("Actions", urls);

                return Ok(forapi);
            }
        }



        [Public]
        public IActionResult Reg()
        {
            var vm = Wtm.CreateVM<RegVM>();
            return PartialView(vm);
        }

        [Public]
        [HttpPost]
        public IActionResult Reg(RegVM vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(vm);
            }
            else
            {
                var rv = vm.DoReg();
                if (rv == true)
                {
                    return FFResult().CloseDialog().Message(Localizer["Reg.Success"]);
                }
                else
                {
                    return PartialView(vm);
                }
            }
        }

        [AllRights]
        [ActionDescription("Logout")]
        public async Task Logout()
        {
            await Wtm.RemoveUserCache(Wtm.LoginUserInfo.ITCode);
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Response.Redirect("/");
        }

        [AllRights]
        [ActionDescription("ChangePassword")]
        public ActionResult ChangePassword()
        {
            var vm = Wtm.CreateVM<ChangePasswordVM>();
            vm.ITCode = Wtm.LoginUserInfo.ITCode;
            return PartialView(vm);
        }

        [AllRights]
        [HttpPost]
        [ActionDescription("ChangePassword")]
        public ActionResult ChangePassword(ChangePasswordVM vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(vm);
            }
            else
            {
                vm.DoChange();
                return FFResult().CloseDialog().Alert(Localizer["Login.ChangePasswordSuccess"]);
            }
        }

    }
}
