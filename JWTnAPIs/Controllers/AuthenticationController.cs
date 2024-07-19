using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project2.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Project2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthorizeService _authorizeService;
        public AuthenticationController(IAuthorizeService authorizeService)
        {
            _authorizeService = authorizeService;
        }

        [HttpGet]
        [Route("CheckAPIStatus")]
        public IActionResult TestAPI()
        {
            var response = _authorizeService.TestAPI();
            return Ok(response);
        }

        [HttpGet]
        [Route("getAuthenticationToken")]

        public IActionResult GetAuthenticationToken(string? username, [DataType(DataType.Password)] string? password)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username is not provided.");
            }

            if (string.IsNullOrEmpty(password))
            {
                return BadRequest("Password is not provided.");
            }

            bool bAuthenticate = _authorizeService.AuthenticateUser(username, password);
            if (bAuthenticate)
            {
                var token = _authorizeService.CreateToken(username);
                return Ok(token);
            }
            else
            {
                return BadRequest("Provided username and/or password is incorrect.");
            }

        }
    }
}
