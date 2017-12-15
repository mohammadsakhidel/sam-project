using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Utils;
using SamWeb.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using SamUtils.Objects.API;
using SamUtils.Objects.Exceptions;
using SamUxLib.Resources.Values;
using RamancoLibrary.Security.Tokens;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace SamWeb.Controllers
{
    public class AccountController : Controller
    {
        #region Get Actions:
        [HttpGet]
        public ActionResult SignIn()
        {
            return View();
        }
        #endregion

        #region Post Actions:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> SignIn(SignInVM model)
        {
            #region Validate Model:
            if (!ModelState.IsValid)
                return View(model);
            #endregion

            using (var hc = HttpUtil.CreateClient())
            {
                var dto = new SignInDto
                {
                    UserName = model.UserName,
                    Password = model.Password
                };
                var response = await hc.PostAsJsonAsync(ApiActions.account_validateuser, dto);
                HttpUtil.EnsureSuccessStatusCode(response);

                var apiResult = await response.Content.ReadAsAsync<ApiOperationResult>();

                #region SignIn Cookie:
                if (apiResult.Succeeded)
                {
                    var token = apiResult.Data;
                    var jwtToken = new JwtToken(token);
                    var claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, jwtToken.Payload[JwtToken.ARG_USERNAME]));
                    claims.Add(new Claim(ClaimTypes.Name, jwtToken.Payload[JwtToken.ARG_USERNAME]));
                    claims.Add(new Claim(ClaimTypes.Role, jwtToken.Payload[JwtToken.ARG_ROLE]));
                    claims.Add(new Claim(ClaimTypes.GivenName, jwtToken.Payload[JwtToken.ARG_FIRST_NAME]));
                    claims.Add(new Claim(ClaimTypes.Surname, jwtToken.Payload[JwtToken.ARG_SURNAME]));
                    claims.Add(new Claim(ClaimTypes.UserData, (!string.IsNullOrEmpty(jwtToken.Payload[JwtToken.ARG_ACCESS]) ? jwtToken.Payload[JwtToken.ARG_ACCESS] : "")));
                    var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                    HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTime.UtcNow.AddDays(7)
                    }, identity);
                    return RedirectToAction("Index", "CustomerPanel");
                }
                #endregion

                #region Process Unsuccessfull Attempt:
                if (apiResult.ErrorCode == ErrorCodes.invalid_username)
                    ModelState.AddModelError("", Messages.InvalidUserName);
                else if (apiResult.ErrorCode == ErrorCodes.invalid_password)
                    ModelState.AddModelError("", Messages.InvalidPassword);
                else if (apiResult.ErrorCode == ErrorCodes.account_blocked)
                    ModelState.AddModelError("", Messages.AccountDisabled);
                else
                    ModelState.AddModelError("", Messages.LoginFailedTryAgain);

                return View(model);
                #endregion
            }
        }
        #endregion
    }
}