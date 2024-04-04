using AutoMapper;
using Contracts;
using Entities.ConfigurationModels;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Service.Contracts;
using Shared.DataTranferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Service;

public sealed class TransactionService : ITransactionService
{
	private readonly IRepositoryManager _repository;
	private readonly ILoggerManager _logger;
	private readonly IMapper _mapper;
	//private readonly Appsettings _settings;
	//private static string? _ourBankSettlementAccount;
	//private readonly IAccountService _accountService;
	//private readonly ITransactionRepository _transactionRepository;

	//public IAccountService AccountService => AccountService;

	public TransactionService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper/*, IOptions<Appsettings>  settings,*/ /*IAccountService accountService*/ /*ITransactionRepository transactionRepository*/)
	{
		_repository = repository;
		_logger = logger;
		_mapper = mapper;
		//_settings = settings.Value;
		//_ourBankSettlementAccount = _settings.OurBankSettlementaccount;	
		//_accountService = accountService;
		//_transactionRepository = transactionRepository;	

	}

	public Response GetNewTransaction(TransactionRequestDto transactionRequest)
	{
		var transaction = _mapper.Map<Transaction>(transactionRequest);

		var transactionToReturn = _repository.Transaction.CreateNewTransaction(transaction);

		return transactionToReturn;
	}

	public async Task<Response> MakeDepositAsync(string AccountNumber, string TransactionPin, decimal Amount, bool trackChanges)
	{
		var depositResponse = await _repository.Transaction.MakeDeposit(AccountNumber, TransactionPin, Amount, trackChanges);
		await _repository.SaveAsync();

		return depositResponse;			
	}
	
	public async Task<Response> MakeWithdrawal(string AccountNumber, string TransactionPin, decimal Amount, bool trackChanges)
	{
		var withdrawalResponse = await _repository.Transaction.MakeWithdrawal(AccountNumber, TransactionPin, Amount, trackChanges);
		await _repository.SaveAsync();

		return withdrawalResponse;
	}

	public async Task<Response> MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin, bool trackChanges)
	{
		var fundsTransferResponse = await _repository.Transaction.MakeFundsTransfer(FromAccount, ToAccount, Amount, TransactionPin, trackChanges);
		await _repository.SaveAsync();

		return fundsTransferResponse;

	}


}
