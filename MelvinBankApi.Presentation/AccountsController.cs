using CompanyEmployees.Presentation.ActionFilters;
using Contracts;
using Entities.LinkModels;
using Marvin.Cache.Headers;
using MelvinBankApi.Presentation.ActionFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTranferObjects;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MelvinBankApi.Presentation;

//[ApiVersion("1.0")]
[Route("api/v1/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
//[ResponseCache(CacheProfileName = "120SecondsDuration")]
public class AccountsController : ControllerBase
{
	private readonly IServiceManager _service;
	private readonly ILoggerManager _logger;
	private readonly IAuthenticateAccountNumber _authenticateAccountNumber;

	public AccountsController(IServiceManager service, ILoggerManager logger, IAuthenticateAccountNumber authenticateAccountNumber)
	{
		_authenticateAccountNumber = authenticateAccountNumber;
		_service = service;
		_logger = logger;
	}


	/// <summary>
	/// Gets the list of all Accounts
	/// </summary>
	/// <returns>The Accounts list</returns>
	[HttpGet]
	[Route("get_all_accounts_from_db")]
	[HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
	[HttpCacheValidation(MustRevalidate = false)]
	[Authorize(Roles = "Manager")]
	//[ResponseCache(Duration = 60)]
	public async Task<IActionResult> GetAllAccountsFromDb()
	{
		var accounts = await _service.AccountService.GetAllAccountsFromDb(trackChanges: false);		

		return Ok(accounts);

	}

	/// <summary>
	/// Gets the list of all Accounts
	/// </summary>
	/// <returns>The Accounts list</returns>
	[HttpGet]
	[HttpHead]
	[Authorize(Roles = "Manager")]
	[Route("get_all_accounts", Name = "GetAllAccounts")]
	[HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
	[HttpCacheValidation(MustRevalidate = false)]
	[ServiceFilter(typeof(ValidateMediaTypeAttribute))]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> GetAllAccounts([FromQuery] AccountsParameters accountsParameters/*, string AccountNumber*/)
	{
		var linkParams = new LinkParameters(accountsParameters, HttpContext);

		var result = await _service.AccountService.GetAllAccountsAsync(linkParams, /*AccountNumber,*/ trackChanges: false);

		Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.metaData));

		return result.linkResponse.HasLinks ? Ok(result.linkResponse.LinkedEntities) : Ok(result.linkResponse.ShapedEntities);

	}


	[HttpPost]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
	[Route("register_new_account", Name = "RegisterNewAccount")]
	[ServiceFilter(typeof(ValidationFilterAttribute))]
	public async Task<IActionResult> RegisterNewAccount([FromBody] RegisterNewAccountForCreationDto newAccount)
	{
		var createdAccount = await _service.AccountService.RegisterNewAccountAsync(newAccount, newAccount.Pin, newAccount.ConfirmPin, trackChanges: true);

		return Ok(createdAccount);

	}

	/// <summary>
	/// Authenticates an Account
	/// </summary>
	/// <returns>null</returns>
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]	
	[Route("authenticate", Name = "Authenticate")]
	[ServiceFilter(typeof(ValidationFilterAttribute))]
	public async Task<IActionResult> Authenticate([FromBody] AuthenticateForCreationDto authenticateModel)
	{
		var createdAuthenticationModel = await _authenticateAccountNumber.AuthenticateAccountNumberAsync(authenticateModel.AccountNumber, authenticateModel.Pin, trackChanges: true);

		var createdAuthenticationModelDto = createdAuthenticationModel.GetAccountDto;

		return Ok(createdAuthenticationModelDto);
	}

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[Route("get_by_account_number/{AccountNumber}")]
	public async Task<IActionResult> GetByAccountNumber(string AccountNumber)
	{
		if(!Regex.IsMatch(AccountNumber, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$"))
			return BadRequest("Account Must be 10-digit");

		var accountToReturn = await _service.AccountService.GetByAccountNumberAsync(AccountNumber, trackChanges: false);

		var account = accountToReturn.GetAccountDto;

		return Ok(account);	

	}

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[Route("get_account_by_id/{id:int}")]
	public async Task<IActionResult> GetAccountById(int id)
	{
		var account = await _service.AccountService.GetAccountByIdAsync(id, trackChanges: false);	

		return Ok(account);	
	}

	[HttpPut]
	[Route("update_account", Name = "UpdateAccount")]
	[ServiceFilter(typeof(ValidationFilterAttribute))]
	public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountDto account)
	{
		await _service.AccountService.UpdateAccountAsync(account, trackChanges: true);

		return NoContent();
	}

	[HttpDelete]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[Route("delete_account/{Id:int}")]
	public async Task<IActionResult> DeleteAccount(int Id)
	{
		await _service.AccountService.DeleteAccountAsync(Id, trackChanges: false);	
		return Ok();	
	}

	[HttpOptions]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[Route("get_accounts_options")]
	public IActionResult GetAccountsOptions()
	{
		Response.Headers.Add("Allow", "GET, OPTIONS, POST, PUT, DELETE");

		return Ok();
	}


}
