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
using System.Data.Entity.Validation;
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
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpGet]
        public IHttpActionResult Roles()
        {
            try
            {
                var rm = _identityRepo.GetRoleManager();
                var roles = rm.Roles.ToList();
                var dtos = roles.Select(r => Mapper.Map<AspNetRole, IdentityRoleDto>(r));
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion

        #region POST ACTION:
        [HttpPost]
        public IHttpActionResult ValidateUser(SignInDto dto)
        {
            try
            {
                var userManager = _identityRepo.GetUserManager();
                var roleManager = _identityRepo.GetRoleManager();

                #region CREATE FIRTS ADMIN USER:
                var adminRole = roleManager.FindByName(RoleType.admin.ToString());
                var adminUsers = adminRole != null ? adminRole.Users : null;
                if ((adminRole == null || !adminUsers.Any()) && dto.UserName.ToLower() == Values.def_admin_name)
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
                            Creator = "System",
                            CreationTime = DateTimeUtils.Now
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
                                AccessLevel = "Everywhere",
                                DisplayName = "Administrator",
                                CreationTime = DateTimeUtils.Now,
                                Creator = "System"
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
                    return Ok(new ApiOperationResult { Succeeded = false, ErrorCode = ErrorCodes.invalid_username });
                // is user approved?
                if (!user.IsApproved)
                    return Ok(new ApiOperationResult { Succeeded = false, ErrorCode = ErrorCodes.account_blocked });
                // is password valid?
                var isPassValid = userManager.CheckPassword(user, dto.Password);
                if (!isPassValid)
                    return Ok(new ApiOperationResult { Succeeded = false, ErrorCode = ErrorCodes.invalid_password });

                var userRoleId = user.Roles.FirstOrDefault()?.RoleId;
                var userRole = roleManager.FindById(userRoleId);
                #endregion

                #region Generate JWT:
                var header = JwtToken.GetDefaultJwtHeader();

                var payload = JwtToken.GetDefaultJwtPayload(Values.jwt_issure, Collections.ApiClients.First().Key, DateTimeUtils.Now.ToString(StringFormats.jwt_date_time));
                payload.Add(JwtToken.ARG_FIRST_NAME, user.FirstName);
                payload.Add(JwtToken.ARG_SURNAME, user.Surname);
                payload.Add(JwtToken.ARG_GENDER, (user.Gender.HasValue ? (user.Gender.Value ? "male" : "female") : ""));
                payload.Add(JwtToken.ARG_YEAR_OF_BIRTH, (user.BirthYear.HasValue ? user.BirthYear.Value.ToString() : ""));
                payload.Add(JwtToken.ARG_USERNAME, user.UserName);
                payload.Add(JwtToken.ARG_ROLE, userRole?.Name);
                payload.Add(JwtToken.ARG_ACCESS, userRole?.AccessLevel);
                payload.Add(JwtToken.ARG_TOKEN_CREATION_TIME, DateTimeUtils.Now.ToString(StringFormats.jwt_date_time));

                var secKey = Collections.ApiClients[payload[JwtToken.ARG_AUDIENCE]];
                var token = new JwtToken(header, payload, secKey);
                #endregion

                return Ok(new ApiOperationResult { Succeeded = true, Data = token.EncodedToken });
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpPost]
        public IHttpActionResult Create(IdentityUserDto dto)
        {
            try
            {
                var userManager = _identityRepo.GetUserManager();

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
                    newUser.Creator = dto.Creator;
                    newUser.CreationTime = DateTimeUtils.Now;

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
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpPost]
        public IHttpActionResult CreateRole(IdentityRoleDto dto)
        {
            try
            {
                var roleManager = _identityRepo.GetRoleManager();

                #region Validation:
                // role exists?
                if (roleManager.RoleExists(dto.Name))
                    return Ok(new ApiOperationResult { Succeeded = false, ErrorCode = ErrorCodes.role_exists });
                #endregion

                #region Create Role:
                var role = new AspNetRole()
                {
                    Name = dto.Name,
                    Type = RoleType.oprator.ToString(),
                    AccessLevel = dto.AccessLevel,
                    DisplayName = dto.DisplayName,
                    CreationTime = DateTimeUtils.Now,
                    Creator = dto.Creator
                };
                var resCreate = roleManager.Create(role);
                if (!resCreate.Succeeded)
                    return Ok(new ApiOperationResult { Succeeded = false, ErrorMessage = "Role Creation Failed!" });
                #endregion

                return Ok(new ApiOperationResult { Succeeded = true });
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion

        #region PUT ACTIONS:
        [HttpPut]
        public IHttpActionResult Update(IdentityUserDto dto)
        {
            try
            {
                var userManager = _identityRepo.GetUserManager();
                var toBeEditedUser = userManager.FindById(dto.Id);
                if (toBeEditedUser == null)
                    return NotFound();

                #region Validation:
                if (toBeEditedUser.UserName == Values.def_admin_name && dto.UserName != Values.def_admin_name)
                    return Ok(new ApiOperationResult { Succeeded = false, ErrorCode = ErrorCodes.sysadmin_username_changing });

                // user exists?
                var existingUser = userManager.FindByName(dto.UserName);
                if (existingUser != null && existingUser.UserName != toBeEditedUser.UserName)
                    return Ok(new ApiOperationResult { Succeeded = false, ErrorCode = ErrorCodes.duplicate_username });

                // valid username & password:
                if (!(new Regex(Patterns.username)).IsMatch(dto.UserName))
                    return Ok(new ApiOperationResult { Succeeded = false, ErrorCode = ErrorCodes.invalid_username });
                if (!string.IsNullOrEmpty(dto.PlainPassword) && !(new Regex(Patterns.password)).IsMatch(dto.PlainPassword))
                    return Ok(new ApiOperationResult { Succeeded = false, ErrorCode = ErrorCodes.invalid_password });
                #endregion

                #region Update Data:
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    #region Update User Info:
                    toBeEditedUser.UserName = dto.UserName;
                    toBeEditedUser.IsApproved = dto.IsApproved;
                    toBeEditedUser.Email = dto.Email;
                    toBeEditedUser.PhoneNumber = dto.PhoneNumber;
                    toBeEditedUser.FirstName = dto.FirstName;
                    toBeEditedUser.Surname = dto.Surname;
                    #endregion

                    #region Update Role:
                    if (!userManager.IsInRole(toBeEditedUser.Id, dto.RoleName))
                    {
                        userManager.RemoveFromRoles(toBeEditedUser.Id, userManager.GetRoles(toBeEditedUser.Id).ToArray());
                        var resRole = userManager.AddToRole(toBeEditedUser.Id, dto.RoleName);
                        if (!resRole.Succeeded)
                            return Ok(new ApiOperationResult { Succeeded = false, ErrorMessage = "Identity Role Assignment Failed!" });
                    }
                    #endregion

                    #region Reset Password:
                    if (!string.IsNullOrEmpty(dto.PlainPassword))
                    {
                        var resReset1 = userManager.RemovePassword(toBeEditedUser.Id);
                        var resReset2 = userManager.AddPassword(toBeEditedUser.Id, dto.PlainPassword);
                        if (!resReset1.Succeeded || !resReset2.Succeeded)
                            return Ok(new ApiOperationResult { Succeeded = false, ErrorMessage = "Reset Password Failed!" });
                    }
                    #endregion

                    userManager.Update(toBeEditedUser);
                    scope.Complete();
                }
                #endregion

                return Ok(new ApiOperationResult { Succeeded = true });
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpPut]
        public IHttpActionResult UpdateRole(IdentityRoleDto dto)
        {
            try
            {
                var roleManager = _identityRepo.GetRoleManager();
                var toBeEditedRole = roleManager.FindById(dto.Id);
                if (toBeEditedRole == null)
                    return NotFound();

                #region Validation:
                if (toBeEditedRole.Type == RoleType.admin.ToString() && dto.Name != toBeEditedRole.Name)
                    return Ok(new ApiOperationResult { Succeeded = false, ErrorCode = ErrorCodes.sysadmin_rolename_changing });

                // user exists?
                var existingRole = roleManager.FindByName(dto.Name);
                if (existingRole != null && existingRole.Name != toBeEditedRole.Name)
                    return Ok(new ApiOperationResult { Succeeded = false, ErrorCode = ErrorCodes.role_exists });
                #endregion

                #region Update Role Info:
                toBeEditedRole.Name = dto.Name;
                toBeEditedRole.DisplayName = dto.DisplayName;
                toBeEditedRole.Type = dto.Type;
                toBeEditedRole.AccessLevel = dto.AccessLevel;
                #endregion

                roleManager.Update(toBeEditedRole);
                return Ok(new ApiOperationResult { Succeeded = true });
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion

        #region DELETE ACTIONS:
        [HttpDelete]
        public IHttpActionResult Delete(string id)
        {
            try
            {
                var userManager = _identityRepo.GetUserManager();
                var userToDelete = userManager.FindById(id);
                if (userToDelete == null)
                    return NotFound();

                #region Validation:
                if (userToDelete.UserName == Values.def_admin_name)
                    throw new Exception("You can't delete built in system administrator.");
                #endregion

                #region Delete Data:
                userManager.Delete(userToDelete);
                #endregion

                return Ok(new ApiOperationResult { Succeeded = true });
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpDelete]
        public IHttpActionResult DeleteRole(string id)
        {
            try
            {
                var roleManager = _identityRepo.GetRoleManager();
                var roleToDelete = roleManager.FindById(id);
                if (roleToDelete == null)
                    return NotFound();

                #region Validation:
                if (roleToDelete.Type == RoleType.admin.ToString())
                    throw new Exception("You can't delete built in administrator role.");
                #endregion

                #region Delete Data:
                roleManager.Delete(roleToDelete);
                #endregion

                return Ok(new ApiOperationResult { Succeeded = true });
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion
    }
}