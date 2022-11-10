using CardStorageService.Models.Requests;
using CardStorageService.Models;
using CardStorageService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using FluentValidation;
using System.Collections.Generic;
using FluentValidation.Results;

namespace CardStorageService.Controllers
{
    [Authorize]
    [Route("api/auth")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        #region Services

        private readonly IAuthenticateService _authenticateService;
        private readonly IValidator<AuthenticationRequest> _authenticationRequestValidator;

        #endregion

        #region Constructors

        public AuthenticateController(IAuthenticateService authenticateService,
             IValidator<AuthenticationRequest> authenticationRequestValidator)
        {
            _authenticateService = authenticateService;
            _authenticationRequestValidator = authenticationRequestValidator;
        }

        #endregion

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        public IActionResult Login([FromBody] AuthenticationRequest authenticationRequest)
        {
            ValidationResult validationResult = _authenticationRequestValidator.Validate(authenticationRequest);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.ToDictionary());

            AuthenticationResponse authenticationResponse = _authenticateService.Login(authenticationRequest);
            if (authenticationResponse.Status == Models.AuthenticationStatus.Success)
            {
                Response.Headers.Add("X-Session-Token", authenticationResponse.SessionInfo.SessionToken);
            }
            return Ok(authenticationResponse);
        }

        [HttpGet]
        [Route("session")]
        [ProducesResponseType(typeof(SessionInfo), StatusCodes.Status200OK)]
        public IActionResult GetSessionInfo()
        {
            var authorization = Request.Headers[HeaderNames.Authorization];
            
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var scheme = headerValue.Scheme; 
                var sessionToken = headerValue.Parameter; 
                if (string.IsNullOrEmpty(sessionToken))
                    return Unauthorized();

                SessionInfo sessionInfo = _authenticateService.GetSessionInfo(sessionToken);
                if (sessionInfo == null)
                    return Unauthorized();

                return Ok(sessionInfo);
            }
            return Unauthorized();
        }


    }

}
