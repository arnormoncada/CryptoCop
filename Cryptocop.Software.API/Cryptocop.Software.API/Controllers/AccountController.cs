using Microsoft.AspNetCore.Mvc;
using Cryptocop.Software.API.Models.InputModels;
using Cryptocop.Software.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;


namespace Cryptocop.Software.API.Controllers
{
    [Authorize]
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;

        public AccountController(IAccountService accountService, ITokenService tokenService)
        {
            _accountService = accountService;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public IActionResult Register([FromBody] RegisterInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = _accountService.CreateUser(inputModel);
            var token = _tokenService.GenerateJwtToken(user);
            //returns ok since we cant point to an uri
            return Ok(user);
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("signin")]
        public IActionResult SignIn([FromBody] LoginInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = _accountService.AuthenticateUser(inputModel);
            if (user == null) { return Unauthorized("Username or Password is invalid"); }
            var token = _tokenService.GenerateJwtToken(user);
            return Ok(token);
        }

        //extra helper endpoint to get the current user
        [HttpGet]
        [Route("userinfo")]
        public IActionResult GetUserInfo()
        {
            var claims = User.Claims.Select(c => new
            {
                Type = c.Type,
                Value = c.Value
            });
            return Ok(claims);
        }

        [HttpGet]
        [Route("signout")]
        public IActionResult LogOut()
        {
            int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "tokenID")?.Value, out var tokenId);
            _accountService.Logout(tokenId);
            return NoContent();
        }

    }
}