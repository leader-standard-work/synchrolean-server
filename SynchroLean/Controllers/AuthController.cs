using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SynchroLean.Controllers.Resources;
using SynchroLean.Core;

namespace SynchroLean.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        public AuthController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginResource loginResource)
        {
            var account = await unitOfWork.userAccountRepository
                .GetUserAccountByEmailAsync(loginResource.Email);

            if (account == null || account.IsDeleted)
            {
                return NotFound("Account could not be found.");
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("developmentKey!@3"));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: "http://localhost:55542",
                audience: "http://localhost:4200",
                claims: new List<Claim>(),
                expires: DateTime.Now.AddSeconds(5),
                signingCredentials: signingCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return Ok(new { Token = tokenString });
        }
    }
}
