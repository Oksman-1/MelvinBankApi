using Contracts;
using Entities.ConfigurationModels;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository;

public class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepository
{
	private readonly IAuthenticateAccountNumber _authenticateAccountNumber;
	private readonly ILoggerManager _logger;
	private readonly Appsettings _settings;
	private static string? _ourBankSettlementAccount;
	



	public TransactionRepository(MelvinBankContext melvinBankContext, ILoggerManager logger, IOptions<Appsettings> settings, IAuthenticateAccountNumber authenticateAccountNumber) : base(melvinBankContext)
	{	
		_authenticateAccountNumber = authenticateAccountNumber;
		_logger = logger;
		_settings = settings.Value;
		_ourBankSettlementAccount = _settings.OurBankSettlementaccount;
	}

	

	public Response CreateNewTransaction(Transaction transaction)
	{
		Response response = new Response();
		Create(transaction);

		response.ResponseCode = "00";
		response.ResponseMessage = "Transaction Created Successfully";
		response.Data = null;

		return response;			
	}

	public async Task<Response> FindTransactionByDate(DateTime date, bool trackChanges)
	{
		Response response = new Response();
		var transaction = await FindByCondition(x => x.TransactionDate == date, trackChanges).ToListAsync();

		response.ResponseCode = "00";
		response.ResponseMessage = "Transaction Created Successfully";
		response.Data = transaction;

		return response;
	}

	public async Task<Response> MakeDeposit(string AccountNumber, string TransactionPin, decimal Amount, bool trackChanges)
	{
		Response response = new Response();
		Account sourceAccount;
		Account destinationAccount;

		Transaction transaction = new Transaction();

		var authUser = await _authenticateAccountNumber.AuthenticateAccountNumberAsync(AccountNumber, TransactionPin, trackChanges);
		if (authUser.Account == null)
			throw new InvalidCredentialsException();

		try
		{
			sourceAccount = await _authenticateAccountNumber.GetAccountNumberAsync(AccountNumber, trackChanges);

			//sourceAccount = authenticatedSourceAccountNumber.Account;

			destinationAccount = await _authenticateAccountNumber.GetAccountNumberAsync(_ourBankSettlementAccount, trackChanges);

			//destinationAccount = authenticatedDestinationAccountNumber.Account;

			//var sourceAccountDto = await _serviceManager.AccountService.GetByAccountNumberAsync(AccountNumber, trackChanges);
			//sourceAccount =  await _repository.Account.GetByAccountNumber(AccountNumber,trackChanges);

			//sourceAccount = _mapper.Map<Account>(sourceAccountDto);

			//sourceAccount = sourceAccountDto.Account;

			//destinationAccount = await _accountNumber.GetByAccountNumber(_ourBankSettlementAccount, trackChanges);


			//var destinationAccountDto = await _serviceManager.AccountService.GetByAccountNumberAsync(_ourBankSettlementAccount, trackChanges);
			//destinationAccount = await _repository.Account.GetByAccountNumber(_ourBankSettlementAccount, trackChanges);

			//destinationAccount = _mapper.Map<Account>(destinationAccountDto);
			//destinationAccount = destinationAccountDto.Account;

			sourceAccount.CurrentAccountBalance += Amount;
			destinationAccount.CurrentAccountBalance -= Amount;

			var sourceEntityState = (DbEntityState(sourceAccount)).ToString();
			var destinationEntityState = (DbEntityState(destinationAccount)).ToString();

			_logger.LogInfo($"SOURCE ENTITY STATE: {sourceEntityState}");
			_logger.LogInfo($"SOURCE CURRENT ACCOUNT BALANCE: {sourceAccount.CurrentAccountBalance}");
			_logger.LogInfo($"DESTINATION ENTITY STATE: {destinationEntityState}");
			_logger.LogInfo($"DESTINATION CURRENT ACCOUNT BALANCE: {destinationAccount.CurrentAccountBalance}");


			if ((DbEntityState(sourceAccount) == EntityState.Modified) && (DbEntityState(destinationAccount) == EntityState.Modified))
			{
				transaction.TransactionStatus = TransStatus.Success;
				response.ResponseCode = "00";
				response.ResponseMessage = "Transaction Successful!";
				response.Data = null;
			}
			else
			{
				transaction.TransactionStatus = TransStatus.Failed;
				response.ResponseCode = "02";
				response.ResponseMessage = "Transaction Failed!";
				response.Data = null;
			}

		}
		catch (Exception ex)
		{
			_logger.LogError($"An Error Occured......=> {ex.Message}");
		}

		transaction.TransactionType = TranType.Deposit;
		transaction.TransactionSourceAccount = _ourBankSettlementAccount;
		transaction.TransactionDestinationAccount = AccountNumber;
		transaction.TransactionAmount = Amount;
		transaction.TransactionDate = DateTime.Now;
		transaction.TransactionParticulars = $"NEW TRANSACTION FROM SOURCE => {JsonConvert.SerializeObject(transaction.TransactionSourceAccount)} TO DESTINATION ACCOUNT => {JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} ON DATE => {JsonConvert.SerializeObject(transaction.TransactionDate)} FOR AMOUNT => {JsonConvert.SerializeObject(transaction.TransactionAmount)} TRANSACTION TYPE =>{transaction.TransactionType} TRANSACTION STATUS => {transaction.TransactionStatus} ";

		Create(transaction);		

		return response;
	}

	public async Task<Response> MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin, bool trackChanges)
	{

		Response response = new Response();
		Account sourceAccount;
		Account destinationAccount;
		Transaction transaction = new Transaction();


		var authUser = await _authenticateAccountNumber.AuthenticateAccountNumberAsync(FromAccount, TransactionPin, trackChanges);
		if (authUser.Account == null)
			throw new InvalidCredentialsException();

		try
		{
			//var sourceAccountDto = await _accountService.GetByAccountNumberAsync(FromAccount, trackChanges);
			//sourceAccount = _mapper.Map<Account>(sourceAccountDto);
			sourceAccount = await _authenticateAccountNumber.GetAccountNumberAsync(FromAccount, trackChanges);

			//var destinationAccountDto = await _accountService.GetByAccountNumberAsync(ToAccount, trackChanges);
			//destinationAccount = _mapper.Map<Account>(destinationAccountDto);
			destinationAccount = await _authenticateAccountNumber.GetAccountNumberAsync(ToAccount, trackChanges);


			if (sourceAccount.CurrentAccountBalance <= 10000)
				throw new ApplicationException("Insufficient Funds, Please fund your Account......");
			if (Amount < 500)
				throw new ApplicationException("Amount must be above current transfer amount......");


			if ((sourceAccount.CurrentAccountBalance - Amount) <= 10000)
			{
				transaction.TransactionStatus = TransStatus.Failed;
				response.ResponseCode = "03";
				response.ResponseMessage = "Insufficient Funds!";
				response.Data = null;

				sourceAccount.CurrentAccountBalance -= 0;
				destinationAccount.CurrentAccountBalance += 0;

				_logger.LogWarn("Insufficient Funds, Please fund your Account......");
			}
			else
			{
				sourceAccount.CurrentAccountBalance -= Amount;
				destinationAccount.CurrentAccountBalance += Amount;

				var sourceEntityState = (DbEntityState(sourceAccount)).ToString();
				var destinationEntityState = (DbEntityState(destinationAccount)).ToString();

				_logger.LogInfo($"SOURCE ENTITY STATE: {sourceEntityState}");
				_logger.LogInfo($"SOURCE CURRENT ACCOUNT BALANCE: {sourceAccount.CurrentAccountBalance}");
				_logger.LogInfo($"DESTINATION ENTITY STATE: {destinationEntityState}");
				_logger.LogInfo($"DESTINATION CURRENT ACCOUNT BALANCE: {destinationAccount.CurrentAccountBalance}");

				if ((DbEntityState(sourceAccount) == EntityState.Modified) && (DbEntityState(destinationAccount) == EntityState.Modified))
				{
					transaction.TransactionStatus = TransStatus.Success;
					response.ResponseCode = "00";
					response.ResponseMessage = "Transaction Successful!";
					response.Data = null;
				}
				else
				{
					transaction.TransactionStatus = TransStatus.Failed;
					response.ResponseCode = "02";
					response.ResponseMessage = "Transaction Failed!";
					response.Data = null;
				}
			}

		}
		catch (Exception ex)
		{
			_logger.LogError($"An Error Occured......=> {ex.Message}");
		}

		transaction.TransactionType = TranType.Transfer;
		transaction.TransactionSourceAccount = FromAccount;
		transaction.TransactionDestinationAccount = ToAccount;
		transaction.TransactionAmount = Amount;
		transaction.TransactionDate = DateTime.Now;
		transaction.TransactionParticulars = $"NEW TRANSACTION FROM SOURCE => {JsonConvert.SerializeObject(transaction.TransactionSourceAccount)} TO DESTINATION ACCOUNT => {JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} ON DATE => {JsonConvert.SerializeObject(transaction.TransactionDate)} FOR AMOUNT => {JsonConvert.SerializeObject(transaction.TransactionAmount)} TRANSACTION TYPE =>{transaction.TransactionType} TRANSACTION STATUS => {transaction.TransactionStatus} ";

		Create(transaction);
		
		return response;
	}

	public async Task<Response> MakeWithdrawal(string AccountNumber, string TransactionPin, decimal Amount, bool trackChanges)
	{
		Response response = new Response();
		Account sourceAccount;
		Account destinationAccount;
		Transaction transaction = new Transaction();

		var authUser = await _authenticateAccountNumber.AuthenticateAccountNumberAsync(AccountNumber, TransactionPin, trackChanges);
		if (authUser.Account == null)
			throw new InvalidCredentialsException();

		try
		{
			//var sourceAccountDto = await _accountService.GetByAccountNumberAsync(_ourBankSettlementAccount, trackChanges);
			//sourceAccount = _mapper.Map<Account>(sourceAccountDto);
			sourceAccount = await _authenticateAccountNumber.GetAccountNumberAsync(AccountNumber, trackChanges);

			//var destinationAccountDto = await _accountService.GetByAccountNumberAsync(AccountNumber, trackChanges);
			//destinationAccount = _mapper.Map<Account>(destinationAccountDto);
			destinationAccount = await _authenticateAccountNumber.GetAccountNumberAsync(_ourBankSettlementAccount, trackChanges);

			sourceAccount.CurrentAccountBalance -= Amount;
			destinationAccount.CurrentAccountBalance += Amount;

			var sourceEntityState = (DbEntityState(sourceAccount)).ToString();
			var destinationEntityState = (DbEntityState(destinationAccount)).ToString();

			_logger.LogInfo($"SOURCE ENTITY STATE: {sourceEntityState}");
			_logger.LogInfo($"SOURCE CURRENT ACCOUNT BALANCE: {sourceAccount.CurrentAccountBalance}");
			_logger.LogInfo($"DESTINATION ENTITY STATE: {destinationEntityState}");
			_logger.LogInfo($"DESTINATION CURRENT ACCOUNT BALANCE: {destinationAccount.CurrentAccountBalance}");

			if ((DbEntityState(sourceAccount) == EntityState.Modified) && (DbEntityState(destinationAccount) == EntityState.Modified))
			{
				transaction.TransactionStatus = TransStatus.Success;
				response.ResponseCode = "00";
				response.ResponseMessage = "Transaction Successful!";
				response.Data = null;
			}
			else
			{
				transaction.TransactionStatus = TransStatus.Failed;
				response.ResponseCode = "02";
				response.ResponseMessage = "Transaction Failed!";
				response.Data = null;
			}

		}
		catch (Exception ex)
		{
			_logger.LogError($"An Error Occured......=> {ex.Message}");
		}

		transaction.TransactionType = TranType.Withdrawal;
		transaction.TransactionSourceAccount = AccountNumber;
		transaction.TransactionDestinationAccount = _ourBankSettlementAccount;
		transaction.TransactionAmount = Amount;
		transaction.TransactionDate = DateTime.Now;
		transaction.TransactionParticulars = $"NEW TRANSACTION FROM SOURCE => {JsonConvert.SerializeObject(transaction.TransactionSourceAccount)} TO DESTINATION ACCOUNT => {JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} ON DATE => {JsonConvert.SerializeObject(transaction.TransactionDate)} FOR AMOUNT => {JsonConvert.SerializeObject(transaction.TransactionAmount)} TRANSACTION TYPE =>{transaction.TransactionType} TRANSACTION STATUS => {transaction.TransactionStatus} ";

		Create(transaction);		

		return response;
	}

	public void SaveEntity(Transaction entity)
	{
		Create(entity);
	}
}
