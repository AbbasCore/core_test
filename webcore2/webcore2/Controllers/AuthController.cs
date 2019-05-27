using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using webcore2.Data;
using webcore2.DTOS;
using webcore2.Models;

namespace webcore2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController:ControllerBase
    {
        private readonly IAuthRepository _rpo;
        private readonly IConfiguration _conf;

        public AuthController(IAuthRepository rpo, IConfiguration configuration)
        {
            _rpo = rpo;
            _conf = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForLogin userForRegistory)
        {
            userForRegistory.username = userForRegistory.username.ToLower();
            if (await _rpo.UserExists(userForRegistory.username)) return BadRequest("هذا الاسم موجود ");
            var newUser = new User { userName = userForRegistory.username }; 
            var makePassword_register = await _rpo.Register(newUser, userForRegistory.password);
            return StatusCode(201);
        }


        [HttpPost("login")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Login(userForLogins userForLogin)
        {
            //throw new Exception("api says noooo !!!");
            try
            {
                var userFromRepo = await _rpo.Login(userForLogin.username.ToLower(), userForLogin.password);
                if (userFromRepo == null) return NotFound();
                //create token...
                var claims = new[]//token body
               {
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.id.ToString()),
                new Claim(ClaimTypes.Name,userFromRepo.userName)
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_conf.GetSection("AppSettings:Token").Value));
                //ecryption key of token
                var cride = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var tokenDescription = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = cride//key of encription
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescription);

                return Ok(new { Token = tokenHandler.WriteToken(token) });
            }
            catch()
            {
                return StatusCode(500, "api is vary tired");
            }
        }

    }
}
