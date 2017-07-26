using Microsoft.AspNet.Identity;
using RamancoLibrary.Security.Tokens;
using RamancoLibrary.Utilities;
using SamAPI.Code.Utils;
using SamDataAccess.IdentityModels;
using SamDataAccess.Repos.Interfaces;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web.Http;

namespace SamAPI.Controllers
{
    public class AccountController : ApiController
    {
        #region Fields:
        IIdentityRepo _identityRepo;
        #endregion

        #region Ctors:
        public AccountController(IIdentityRepo identityRepo)
        {
            _identityRepo = identityRepo;
        }
        #endregion

        #region POST:
        [HttpPost]
        public HttpResponseMessage ValidateUser(SignInDto dto)
        {
            try
            {
                var userManager = _identityRepo.GetUserManager();
                var roleManager = _identityRepo.GetRoleManager();

                #region CREATE FIRTS ADMIN USER:
                var adminRole = roleManager.FindByName(RoleType.admin.ToString());
                var adminUsers = adminRole != null ? adminRole.Users : null;
                if ((adminRole == null || !adminUsers.Any()) &&
                    dto.UserName.ToLower() == Values.def_admin_name)
                {
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        #region Create Admin User:
                        var newAdminUser = new AspNetUser()
                        {
                            UserName = dto.UserName.ToLower(),
                            IsApproved = true,
                            FirstName = "Administrator",
                            Surname = "Account",
                            Gender = true
                        };
                        userManager.Create(newAdminUser, dto.Password);
                        #endregion

                        #region Create Role:
                        var roleExists = roleManager.RoleExists(RoleType.admin.ToString());
                        if (!roleExists)
                            roleManager.Create(new AspNetRole(RoleType.admin.ToString()));
                        #endregion

                        #region Add User To Role:
                        userManager.AddToRole(newAdminUser.Id, RoleType.admin.ToString());
                        #endregion

                        scope.Complete();
                    }
                }
                #endregion

                #region Validate UserName & Password:
                var user = userManager.FindByName(dto.UserName);
                // is username valid?
                if (user == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                // is user approved?
                if (!user.IsApproved)
                    return Request.CreateResponse(HttpStatusCode.Forbidden);
                // is password valid?
                var isPassValid = userManager.CheckPassword(user, dto.Password);
                if (!isPassValid)
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);

                var userRoles = userManager.GetRoles(user.Id);
                #endregion

                #region Generate JWT:
                var header = JwtToken.GetDefaultJwtHeader();

                var payload = JwtToken.GetDefaultJwtPayload(Values.jwt_issure, Collections.ApiClients.First().Key, DateTimeUtils.Now.ToString(StringFormats.jwt_date_time));
                payload.Add(JwtToken.ARG_FIRST_NAME, user.FirstName);
                payload.Add(JwtToken.ARG_SURNAME, user.Surname);
                payload.Add(JwtToken.ARG_GENDER, (user.Gender ? "male" : "female"));
                payload.Add(JwtToken.ARG_YEAR_OF_BIRTH, (user.BirthYear.HasValue ? user.BirthYear.Value.ToString() : ""));
                payload.Add(JwtToken.ARG_USERNAME, user.UserName);
                payload.Add(JwtToken.ARG_ROLE, TextUtils.Concat(userRoles, Values.list_separator, false));
                payload.Add(JwtToken.ARG_ACCESS, user.AccessLevel);
                payload.Add(JwtToken.ARG_TOKEN_CREATION_TIME, DateTimeUtils.Now.ToString(StringFormats.jwt_date_time));

                var secKey = Collections.ApiClients[payload[JwtToken.ARG_AUDIENCE]];
                var token = new JwtToken(header, payload, secKey);
                #endregion

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(token.EncodedToken);
                return response;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ExceptionManager.GetProperApiMessage(ex));
            }
        }
        #endregion
    }
}
