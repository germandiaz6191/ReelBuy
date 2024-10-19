﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Helpers;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ReelBuy.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IUsersUnitOfWork _usersUnitOfWork;
    private readonly IConfiguration _configuration;
    private readonly IMailHelper _mailHelper;
    private readonly DataContext _context;

    public AccountsController(IUsersUnitOfWork usersUnitOfWork, IConfiguration configuration, IMailHelper mailHelper, DataContext context)
    {
        _usersUnitOfWork = usersUnitOfWork;
        _configuration = configuration;
        _mailHelper = mailHelper;
        _context = context;
    }

    [HttpPost("CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] UserDTO model)
    {
        var country = await _context.Countries.FindAsync(model.CountryId);
        var profile = await _context.Profiles.FindAsync(model.ProfileId);
        if (country == null || profile == null)
        {
            return BadRequest("ERR004");
        }
        User user = model;
        user.Country = country;
        user.Profile = profile;
        var result = await _usersUnitOfWork.AddUserAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _usersUnitOfWork.AddUserToRoleAsync(user, profile.Name);
            var response = await SendConfirmationEmailAsync(user, model.Language);
            if (response.WasSuccess)
            {
                return NoContent();
            }
            return BadRequest(response.Message);
        }

        return BadRequest(result.Errors.FirstOrDefault());
    }

    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmailAsync(string userId, string token)
    {
        token = token.Replace(" ", "+");
        var user = await _usersUnitOfWork.GetUserAsync(new Guid(userId));
        if (user == null)
        {
            return NotFound();
        }

        var result = await _usersUnitOfWork.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.FirstOrDefault());
        }

        return NoContent();
    }

    [HttpPost("ResedToken")]
    public async Task<IActionResult> ResedTokenAsync([FromBody] EmailDTO model)
    {
        var user = await _usersUnitOfWork.GetUserAsync(model.Email);
        if (user == null)
        {
            return NotFound();
        }

        var response = await SendConfirmationEmailAsync(user, model.Language);
        if (response.WasSuccess)
        {
            return NoContent();
        }

        return BadRequest(response.Message);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDTO model)
    {
        var result = await _usersUnitOfWork.LoginAsync(model);
        if (result.Succeeded)
        {
            var user = await _usersUnitOfWork.GetUserAsync(model.Email);
            return Ok(BuildToken(user));
        }

        if (result.IsLockedOut)
        {
            return BadRequest("ERR007");
        }

        if (result.IsNotAllowed)
        {
            return BadRequest("ERR008");
        }

        return BadRequest("ERR006");
    }

    private TokenDTO BuildToken(User user)
    {
        var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Email!),
                new(ClaimTypes.Role, user.Profile.Name),
                new("FirstName", user.FirstName),
                new("LastName", user.LastName),
                new("Photo", user.Photo ?? string.Empty),
                new("CountryId", user.Country.Id.ToString())
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddDays(30);
        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        return new TokenDTO
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expiration
        };
    }

    private async Task<ActionResponse<string>> SendConfirmationEmailAsync(User user, string language)
    {
        var myToken = await _usersUnitOfWork.GenerateEmailConfirmationTokenAsync(user);
        var tokenLink = Url.Action("ConfirmEmail", "accounts", new
        {
            userid = user.Id,
            token = myToken
        }, HttpContext.Request.Scheme, _configuration["Url Frontend"]);

        if (language == "es")
        {
            return _mailHelper.SendMail(user.FullName, user.Email!, _configuration["Mail:SubjectConfirmationEs"]!, string.Format(_configuration["Mail:BodyConfirmationEs"]!, tokenLink), language);
        }
        return _mailHelper.SendMail(user.FullName, user.Email!, _configuration["Mail:SubjectConfirmationEn"]!, string.Format(_configuration["Mail:BodyConfirmationEn"]!, tokenLink), language);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    public async Task<IActionResult> PutAsync(User user)
    {
        try
        {
            var currentUser = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
            if (currentUser == null)
            {
                return NotFound();
            }

            currentUser.FirstName = user.FirstName;
            currentUser.LastName = user.LastName;
            currentUser.PhoneNumber = user.PhoneNumber;
            currentUser.Photo = !string.IsNullOrEmpty(user.Photo) && user.Photo != currentUser.Photo ? user.Photo : currentUser.Photo;
            currentUser.CountryId = user.CountryId;
            currentUser.Country = user.Country;

            var result = await _usersUnitOfWork.UpdateUserAsync(currentUser);
            if (result.Succeeded)
            {
                return Ok(BuildToken(currentUser));
            }

            return BadRequest(result.Errors.FirstOrDefault());
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        return Ok(await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!));
    }

    [HttpPost("changePassword")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDTO model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _usersUnitOfWork.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.FirstOrDefault()!.Description);
        }

        return NoContent();
    }

    [HttpPost("RecoverPassword")]
    public async Task<IActionResult> RecoverPasswordAsync([FromBody] EmailDTO model)
    {
        var user = await _usersUnitOfWork.GetUserAsync(model.Email);
        if (user == null)
        {
            return NotFound();
        }

        var response = await SendRecoverEmailAsync(user, model.Language);
        if (response.WasSuccess)
        {
            return NoContent();
        }

        return BadRequest(response.Message);
    }

    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordDTO model)
    {
        var user = await _usersUnitOfWork.GetUserAsync(model.Email);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _usersUnitOfWork.ResetPasswordAsync(user, model.Token, model.NewPassword);
        if (result.Succeeded)
        {
            return NoContent();
        }

        return BadRequest(result.Errors.FirstOrDefault()!.Description);
    }

    private async Task<ActionResponse<string>> SendRecoverEmailAsync(User user, string language)
    {
        var myToken = await _usersUnitOfWork.GeneratePasswordResetTokenAsync(user);
        var tokenLink = Url.Action("ResetPassword", "accounts", new
        {
            userid = user.Id,
            token = myToken
        }, HttpContext.Request.Scheme, _configuration["Url Frontend"]);

        if (language == "es")
        {
            return _mailHelper.SendMail(user.FullName, user.Email!, _configuration["Mail:SubjectRecoveryEs"]!, string.Format(_configuration["Mail:BodyRecoveryEs"]!, tokenLink), language);
        }
        return _mailHelper.SendMail(user.FullName, user.Email!, _configuration["Mail:SubjectRecoveryEn"]!, string.Format(_configuration["Mail:BodyRecoveryEn"]!, tokenLink), language);
    }
}