using AutoMapper;
using CardStorageService.Data;
using CardStorageService.Models.Requests;
using CardStorageService.Models.Validators;
using CardStorageService.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace CardStorageService.Controllers
{
    [Authorize]
    [Route("api/client")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepositoryService _clientRepositoryService;
        private readonly ILogger<ClientController> _logger;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateClientRequest> _createClientRequestValidator;

        public ClientController(IClientRepositoryService clientRepositoryService,
            ILogger<ClientController> logger,
            IMapper mapper,
            IValidator<CreateClientRequest> createClientRequestValidator)
        {
            _clientRepositoryService = clientRepositoryService;
            _logger = logger;
            _mapper = mapper;
            _createClientRequestValidator = createClientRequestValidator;
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CreateClientResponse), StatusCodes.Status200OK)]
        public IActionResult Create([FromBody] CreateClientRequest request)
        {
            try
            {
                ValidationResult validationResult = _createClientRequestValidator.Validate(request);
                if (!validationResult.IsValid)
                    return BadRequest(validationResult.ToDictionary());

                var clientId = _clientRepositoryService.Create(_mapper.Map<Client>(request));
                return Ok(new CreateClientResponse
                {
                    ClientId = clientId
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Create client error.");
                return Ok(new CreateCardResponse
                {
                    ErrorCode = 912,
                    ErrorMessage = "Create clinet error."
                });
            }
        }

    }
}
