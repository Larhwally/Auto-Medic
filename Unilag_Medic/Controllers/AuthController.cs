﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Unilag_Medic.Data;
using Unilag_Medic.ViewModel;

namespace Unilag_Medic.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [Route("register")]
        [HttpPost]
        public async Task<ActionResult> RegisterUser([FromBody] RegisterViewModel model)
        {
            var user = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.PhoneNum,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, model.password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
            }
            return Ok(new { Username = user.UserName });
        }

        [Route("login")]
        [HttpPost]
        public async Task<ActionResult> Login([FromBody] LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var claim = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName)
                };
                var signingKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]));

                int expiryInMinutes = Convert.ToInt32(_configuration["Jwt:ExpiryInMinutes"]);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Site"],
                    audience: _configuration["Jwt:Site"],
                    expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(
                    new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
            }
            return Unauthorized();
        }

        [Route("GetUser")]
        [HttpGet]
        public string GetUsers()
        {
            EntityConnection con = new EntityConnection("AspNetUsers");
            string result = "{'status':true, 'data':" + EntityConnection.ToJson(con.Select()) + "}";
            return result;
        }

    }
}