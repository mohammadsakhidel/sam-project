using AutoMapper;
using Microsoft.AspNet.Identity;
using RamancoLibrary.Security.Tokens;
using RamancoLibrary.Utilities;
using SamAPI.Code.Utils;
using SamDataAccess.IdentityModels;
using SamDataAccess.Repos.Interfaces;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Enums;
using SamUtils.Objects.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
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

        #region GET ACTIONS:
        [HttpGet]
        public IHttpActionResult All()
        {
            try
            {
                Thread.Sleep(2500);
                var um = _identityRepo.GetUserManager();
                var rm = _identityRepo.GetRoleManager();
                var users = um.Users.ToList();
                var roles = rm.Roles.ToList();
                var dtos = users.Select(u => Mapper.Map<AspNetUser, IdentityUserDto>(u, opts => opts.AfterMap((model, dto) =>
                {
                    var userRole = roles.SingleOrDefault(r => model.Roles.Any() && r.Id == model.Roles.First().RoleId);
                    if (userRole != null)
                    {
                        dto.RoleName = userRole.Name;
                        dto.RoleDisplayName = userRole.DisplayName;
                    }
                })));
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception(ExceptionManager.GetProperApiMessage(ex)));
            }
        }
        #endregion

        #region POST ACTION:
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
                            FirstName = "Admin",
                            Surname = "User",
                            Gender = true
                        };
                        userManager.Create(newAdminUser, dto.Password);
                        #endregion

                        #region Create Role:
                        var roleExists = roleManager.RoleExists(RoleType.admin.ToString());
                        if (!roleExists)
                        {
                            var role = new AspNetRole()
                            {
                                Name = RoleType.admin.ToString(),
                                Type = RoleType.admin.ToString(),
                                AccessLevel = "everywhere",
                                DisplayName = "Admin Role"
                            };
                            roleManager.Create(role);
                        }
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
                    return Request.CreateResponse(HttpStatusCode.OK, new ApiOperationResult { Succeeded = false, ErrorCode = ErrorCodes.invalid_username });
                // is user approved?
                if (!user.IsApproved)
                    return Request.CreateResponse(HttpStatusCode.OK, new ApiOperationResult { Succeeded = false, ErrorCode = ErrorCodes.account_blocked });
                // is password valid?
                var isPassValid = userManager.CheckPassword(user, dto.Password);
                if (!isPassValid)
                    return Request.CreateResponse(HttpStatusCode.OK, new ApiOperationResult { Succeeded = false, ErrorCode = ErrorCodes.invalid_password });

                var userRoleId = user.Roles.FirstOrDefault()?.RoleId;
                var userRole = roleManager.FindById(userRoleId);
                #endregion

                #region Generate JWT:
                var header = JwtToken.GetDefaultJwtHeader();

                var payload = JwtToken.GetDefaultJwtPayload(Values.jwt_issure, Collections.ApiClients.First().Key, DateTimeUtils.Now.ToString(StringFormats.jwt_date_time));
                payload.Add(JwtToken.ARG_FIRST_NAME, user.FirstName);
                payload.Add(JwtToken.ARG_SURNAME, user.Surname);
                payload.Add(JwtToken.ARG_GENDER, (user.Gender ? "male" : "female"));
                payload.Add(JwtToken.ARG_YEAR_OF_BIRTH, (user.BirthYear.HasValue ? user.BirthYear.Value.ToString() : ""));
                payload.Add(JwtToken.ARG_USERNAME, user.UserName);
                payload.Add(JwtToken.ARG_ROLE, userRole?.Name);
                payload.Add(JwtToken.ARG_ACCESS, userRole?.AccessLevel);
                payload.Add(JwtToken.ARG_TOKEN_CREATION_TIME, DateTimeUtils.Now.ToString(StringFormats.jwt_date_time));

                var secKey = Collections.ApiClients[payload[JwtToken.ARG_AUDIENCE]];
                var token = new JwtToken(header, payload, secKey);
                #endregion

                return Request.CreateResponse(HttpStatusCode.OK, new ApiOperationResult { Succeeded = true, Data = token.EncodedToken });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ExceptionManager.GetProperApiMessage(ex));
            }
        }

        [HttpPost]
        public IHttpActionResult Create(IdentityUserDto dto)
        {
            try
            {
                var userManager = _identityRepo.GetUserManager();
                var roleManager = _identityRepo.GetRoleManager();

                #region Validation:
                // user exists?
                var existingUser = userManager.FindByName(dto.UserName);
                if (existingUser != null)
                    return Ok(new ApiOperationResult { Succeeded = false, ErrorCode = ErrorCodes.duplicate_username });

                // valid username & password:
                if (!(new Regex(Patterns.username)).IsMatch(dto.UserName))
                    return Ok(new ApiOperationResult { Succeeded = false, ErrorCode = ErrorCodes.invalid_username });
                if (!(new Regex(Patterns.password)).IsMatch(dto.PlainPassword))
                    return Ok(new ApiOperationResult { Succeeded = false, ErrorCode = ErrorCodes.invalid_password });
                #endregion

                #region Create Account:
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    #region Add User:
                    var newUser = new AspNetUser(dto.UserName);
                    newUser.IsApproved = dto.IsApproved;
                    newUser.Email = dto.Email;
                    newUser.PhoneNumber = dto.PhoneNumber;
                    newUser.FirstName = dto.FirstName;
                    newUser.Surname = dto.Surname;
                    newUser.Gender = dto.Gender;

                    var resUser = userManager.Create(newUser, dto.PlainPassword);
                    if (!resUser.Succeeded)
                        return Ok(new ApiOperationResult { Succeeded = false, ErrorMessage = "Identity User Creation Failed!" });
                    #endregion

                    #region Add To Role:
                    var resRole = userManager.AddToRole(newUser.Id, dto.RoleName);
                    if (!resRole.Succeeded)
                        return Ok(new ApiOperationResult { Succeeded = false, ErrorMessage = "Identity Role Assignment Failed!" });
                    #endregion

                    scope.Complete();
                }
                #endregion

                return Ok(new ApiOperationResult { Succeeded = true });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception(ExceptionManager.GetProperApiMessage(ex)));
            }
        }
        #endregion
    }
}
