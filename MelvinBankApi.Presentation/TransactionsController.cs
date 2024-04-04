using Contracts;
using Entities.Models;
using MelvinBankApi.Presentation.ActionFilters;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTranferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MelvinBankApi.Presentation;

[Route("api/v1/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
public class TransactionsController : ControllerBase
{
	private readonly IServiceManager _service;
	private readonly ILoggerManager _logger;

	public TransactionsController(IServiceManager service, ILoggerManager logger)
	{
		_service = service;
		_logger = logger;
	}

	[HttpPost]
	[Route("create_new_transaction")]
	[ServiceFilter(typeof(ValidationFilterAttribute))]
	public async Task<IActionResult> CreateNewTransaction([FromBody] TransactionRequestDto transactionRequest)
	{
		var transaction = _service.TransactionService.GetNewTransaction(transactionRequest);

		return Ok(transaction);
	}


	[HttpPost]
	[Route("make_deposit")]
	public async Task<IActionResult> MakeDeposit(string AccountNumber, string TransactionPin, decimal Amount)
	{
		if (!Regex.IsMatch(AccountNumber, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$"))
			return BadRequest("Account Must be 10-digit");

		var deposit = await _service.TransactionService.MakeDepositAsync(AccountNumber, TransactionPin, Amount, trackChanges: true);

		return Ok(deposit);
	}

	[HttpPost]
	[Route("make_withdrawal")]
	public async Task<IActionResult> MakeWithdrawal(string AccountNumber, string TransactionPin, decimal Amount)
	{
		
		if (!Regex.IsMatch(AccountNumber, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$"))
			return BadRequest("Account Must be 10-digit");
		

		var withdrawal = await _service.TransactionService.MakeWithdrawal(AccountNumber, TransactionPin, Amount, trackChanges: true);

		return Ok(withdrawal);
	}

	[HttpPost]
	[Route("make_funds_transfer")]
	public async Task<IActionResult> MakeFundsTransfer(string FromAccount, string ToAccount, string TransactionPin, decimal Amount)
	{
		if (!Regex.IsMatch(FromAccount, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$") || !Regex.IsMatch(ToAccount, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$"))
			return BadRequest("Account Must be 10-digit");

		var transfer = await _service.TransactionService.MakeFundsTransfer(FromAccount, ToAccount, Amount, TransactionPin, trackChanges: true);

		return Ok(transfer);
	}

}
