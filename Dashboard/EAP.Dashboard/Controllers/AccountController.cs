using EAP.Dashboard.Models;
using EAP.Dashboard.Utils;
using EAP.Models.Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EAP.Dashboard.Controllers
{
    public class AccountController : BaseController
    {
        public ActionResult Login()
        {
            ViewBag.site = site;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public ActionResult Login(LoginViewModel model)
        {
            //如果视图模型中的属性没有验证通过，则返回到登录页面，要求用户重新填写
            if (!ModelState.IsValid)
            {
                // 设置错误信息
                TempData["ErrorMessage"] = "用户名或密码填写格式不对！请检查！";

                return RedirectToAction("Login", "Account");
            }
            Session["isSSOLogin"] = false;
            //model.isSSOLogin = false;
            //根据用户登录名查询指定用户实体
            var db = DbFactory.GetSqlSugarClient();

            var user = db.Queryable<FrameworkUsers>().Where(it => it.ITCode == model.UserName.Trim()).First();

            //如果用户不存在，则携带错误消息并返回登录页面
            if (user == null)
            {
                // 设置错误信息
                TempData["ErrorMessage"] = "用户名或密码错误,请重新登录!";

                return RedirectToAction("Login", "Account");
            }

            //如果密码不匹配，则携带错误消息并返回登录页面
            if (user.Password != Encryptor.Md5Hash(model.Password.Trim()))
            {
                // 设置错误信息
                TempData["ErrorMessage"] = "用户名或密码错误,请重新登录!";

                return RedirectToAction("Login", "Account");
            }

            //并用户实体保存到Session中
            Session["user_account"] = user;
            var role = db.Queryable<FrameworkUserRoles>().Where(it => it.UserCode == user.ITCode).First();
            var roleCode = role == null ? "Guest" : role.RoleCode;

            var menueID = db.Queryable<FunctionPrivileges>().Where(it => it.RoleCode == roleCode).Select(it => it.MenuItemId).Distinct().ToList();

            var modules = db.Queryable<FrameworkMenus>().Where(it => menueID.Contains(it.ID)).ToList();
            Session["controllers"] = modules.Select(it => it.ModuleName).Distinct().ToList();


            Session["RoleCode"] = roleCode;



            //跳转到首页
            return RedirectToAction("index", "home");
        }
        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public ActionResult SSOLogin(LoginViewModel model)
        {

            //如果视图模型中的属性没有验证通过，则返回到登录页面，要求用户重新填写
            if (!ModelState.IsValid)
            {
                // 设置错误信息
                TempData["ErrorMessage"] = "用户名或密码填写格式不对！请检查！";

                return RedirectToAction("Login", "Account");
            }

            //根据用户登录名查询指定用户实体
            model.UserName = model.UserName.ToLower();
            //model.isSSOLogin = true;
            Newtonsoft.Json.Linq.JObject jobj = null;
            var ipAddress = Request.UserHostAddress;
            var test = model.UserName;
            var db = DbFactory.GetSqlSugarClient();
            var ssouser = SsoHelper.GetUserWithUP(ipAddress, model.UserName, model.Password);



            //如果用户不存在，则携带错误消息并返回登录页面
            //if (user == null)
            //{
            //    ModelState.AddModelError("error_message", "用户不存在");
            //    return View(model);
            //}

            //SSO验证未通过的账户
            if (ssouser == null)
            {
                // 设置错误信息
                TempData["ErrorMessage"] = "用户名或密码错误,请重新登录!";

                return RedirectToAction("Login", "Account");


            }
            var user = db.Queryable<FrameworkUsers>().Where(it => it.ITCode == ssouser.userId.ToString()).First();
            //SSO验证通过但是DB未找到该用户
            if (user == null)
            {
                //先去FrameworkRoles找对应的department有没有roleid
                var roleIsExist = db.Queryable<FrameworkRoles>().Where(it => it.RoleName == ssouser.department).First();
                //如果没有，生成一个Role
                if (roleIsExist == null)
                {
                    var roleItem = new FrameworkRoles()
                    {
                        ID = Guid.NewGuid().ToByteArray(),
                        RoleCode = Guid.NewGuid().ToString(),
                        RoleName = ssouser.department,
                        CreateTime = DateTime.Now,
                        CreateBy = "system",
                    };
                    db.Insertable(roleItem).ExecuteCommand();
                }
                //如果有，获取它的roleID
                var ssoRole = db.Queryable<FrameworkRoles>().Where(it => it.RoleName == ssouser.department).First();
                var ssoRoleCode = ssoRole.RoleCode;

                //向DB添加该用户的信息
                user = new FrameworkUsers()
                {
                    ID = Guid.NewGuid().ToByteArray(),
                    //Email = ssouser.mail,0625邮箱用于三色灯项目发邮件才维护的字段
                    ITCode = ssouser.userId,
                    Name = ssouser.name,
                    Password = Encryptor.Md5Hash(ssouser.userId),
                    CreateTime = DateTime.Now,
                    CreateBy = "system",
                    IsValid = true

                };
                db.Insertable(user).ExecuteCommand();

                //添加user-role关系
                var userRole = new FrameworkUserRoles()
                {
                    ID = Guid.NewGuid().ToByteArray(),
                    UserCode = user.ITCode,
                    RoleCode = ssoRoleCode,
                    CreateTime = DateTime.Now,
                    CreateBy = "system"
                };

                db.Insertable(userRole).ExecuteCommand();
            }


            //并用户实体保存到Session中
            Session["user_account"] = user;
            Session["isSSOLogin"] = true;
            var role = db.Queryable<FrameworkUserRoles>().Where(it => it.UserCode == user.ITCode).First();
            var roleCode = role == null ? "Guest" : role.RoleCode;

            Session["RoleCode"] = roleCode;

            var menueID = db.Queryable<FunctionPrivileges>().Where(it => it.RoleCode == roleCode).Select(it => it.MenuItemId).Distinct().ToList();

            var modules = db.Queryable<FrameworkMenus>().Where(it => menueID.Contains(it.ID)).Distinct().ToList();
            Session["controllers"] = modules.Where(it => it.ModuleName != null).Select(it => it.ModuleName).Distinct().ToList();
            //Session["actions"] = modules.Select(it => it.MethodName).ToList();

            //跳转到首页
            return RedirectToAction("index", "home");
        }

        [HttpPost]
        public JsonResult Logout()
        {
            try
            {
                var ssoLogin = (bool)Session["isSSOLogin"];
                if (ssoLogin)
                {
                    string ipAddress = Request.UserHostAddress;
                    SsoHelper.SignOut(ipAddress);
                }

                Session.Abandon();
                return Json(true);
            }
            catch (Exception ex)
            {

                return Json(ex);
            }

        }
    }
}