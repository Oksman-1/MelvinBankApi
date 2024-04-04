using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.LinkModels;
using Entities.Models;
using Service.Contracts;
using Shared.DataTranferObjects;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.Metrics;
using System.Dynamic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Service;

public sealed class AccountService : IAccountService
{
	
	private readonly IRepositoryManager _repository;
	private readonly ILoggerManager _logger;
	private readonly IMapper _mapper;
	//private readonly IDataShaper<GetAccountDto> _dataShaper;
	private readonly IAccountLinks _accountLinks;

	public AccountService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, /*IDataShaper<GetAccountDto> dataShaper,*/ IAccountLinks accountLinks)
	{
		_repository = repository;
		_logger = logger;
		_mapper = mapper;
		//_dataShaper = dataShaper;
		_accountLinks = accountLinks;	
	}

	//public async Task<AuthenticateDto> AuthenticateAsync(AuthenticateForCreationDto authenticationModel, bool trackChanges)
	//{
	//	var x = _mapper.Map<Account>(authenticationModel);

	//	var createdAuthenticationModel = await _repository.Account.Authenticate(authenticationModel.AccountNumber, authenticationModel.Pin, trackChanges);

	//	var AuthenticationModelToReturn = _mapper.Map<AuthenticateDto>(createdAuthenticationModel);

	//	return AuthenticationModelToReturn;
	//}

	//public async Task<(AuthenticateDto GetAccountDto, Account Account)> AuthenticateAccountNumberAsync(string AccountNumber, string Pin, bool trackChanges)
	//{
	//	var createdAuthenticationModel = await _authenticateaccountNumber.AuthenticateAccountNumberAsync (AccountNumber, Pin, trackChanges);

	//	var accountTypeAuthenticatedModel = createdAuthenticationModel.Account;
	//	var dtoTypeAuthenticatedModel = createdAuthenticationModel.GetAccountDto;

	//	return (GetAccountDto: dtoTypeAuthenticatedModel, Account: accountTypeAuthenticatedModel);

	//}
	//public async Task<GetAccountDto> AuthenticateAsync(string AccountNumber, string Pin, bool trackChanges)
	//{
	//	//AuthenticateForCreationDto authenticateForCreationDto = new AuthenticateForCreationDto();
	//	//var x = _mapper.Map<Account>(authenticationModel);

	//	//var createdAuthenticationModel = await _repository.Account.Authenticate(AccountNumber, Pin, trackChanges);

	//	var AuthenticationModelToReturn = _mapper.Map<GetAccountDto>(createdAuthenticationModel);

	//	return AuthenticationModelToReturn;
	//}

	public async Task DeleteAccountAsync(int Id, bool trackChanges)
	{
		_repository.Account.Delete(Id, trackChanges);

		await _repository.SaveAsync();

	}

	public async Task<GetAccountDto> GetAccountByIdAsync(int Id, bool trackChanges)
	{
		var account = await _repository.Account.GetById(Id, trackChanges);

		var accountToReturn = _mapper.Map<GetAccountDto>(account);

		return accountToReturn;	
	}

	//public async Task<IEnumerable<GetAccountDto>> GetAllAccountsAsync(bool trackChanges)
	//{
	//	var accounts = await _repository.Account.GetAllAccounts(trackChanges);

	//	var cleanedAccounts = _mapper.Map<IEnumerable<GetAccountDto>>(accounts);

	//	return cleanedAccounts;		
	//}

	//public async Task<(IEnumerable<ShapedEntity> accountDtos, MetaData metaData)> GetAllAccountsAsync(AccountsParameters accountsParameters, bool trackChanges)
	//{
	//	if (!accountsParameters.ValidCurrentAccountRange)
	//		throw new MaxCurrentAccountRangeBadRequestException();

	//	var accountsWithMetaData = await _repository.Account.GetAllAccounts(accountsParameters,trackChanges);

	//	//var accountsWithMetaData = await _repository.Account
	//	//	.GetEmployeesAsync(employeeParameters, trackChanges);
	//	var accountsWithMetaDataDto = _mapper.Map<IEnumerable<GetAccountDto>>(accountsWithMetaData);
	//	var shapedData = _dataShaper.ShapeData(accountsWithMetaDataDto, accountsParameters.Fields);

	//	//var cleanedAccounts = _mapper.Map<IEnumerable<GetAccountDto>>(accounts);
	//	return (Accounts: shapedData, metaData: accountsWithMetaData.MetaData);
	//}

	public async Task<(LinkResponse linkResponse, MetaData metaData)> GetAllAccountsAsync(LinkParameters linkParameters, /*string AccountNumber,*/ bool trackChanges)
	{
		if (!linkParameters.accountParameters.ValidCurrentAccountRange)
			throw new MaxCurrentAccountRangeBadRequestException();

		var accountsWithMetaData = await _repository.Account.GetAllAccounts(linkParameters.accountParameters, trackChanges);

		var accountsDto = _mapper.Map<IEnumerable<GetAccountDto>>(accountsWithMetaData);

		var links = _accountLinks.TryGenerateLinks(accountsDto, linkParameters.accountParameters.Fields, /*AccountNumber, */linkParameters.Context);

		return (linkResponse: links, metaData: accountsWithMetaData.MetaData);
	}

	public async Task<IEnumerable<GetAccountDto>> GetAllAccountsFromDb(bool trackChanges)
	{
		var accounts = await _repository.Account.GetAllAccountsFromDb(trackChanges);
		var accountToReturn = _mapper.Map<IEnumerable<GetAccountDto>>(accounts);

		return accountToReturn;
	}

	//public async Task<AccountDTO> GetByAccountNumberAsync(string AccountNumber, bool trackChanges)
	//{
	//	var account = await _repository.Account.GetByAccountNumber(AccountNumber, trackChanges);

	//	var accountToReturn = _mapper.Map<AccountDTO>(account);

	//	return accountToReturn;	
	//	//return account;
	//}

	public async Task<(GetAccountDto GetAccountDto, Account Account)> GetByAccountNumberAsync(string AccountNumber, bool trackChanges)
	{
		var account = await _repository.Account.GetByAccountNumber(AccountNumber, trackChanges);

		var accountDtoToReturn = _mapper.Map<GetAccountDto>(account);

		return (GetAccountDto: accountDtoToReturn, Account: account);


	}

	public async Task<RegisterNewAccountDto> RegisterNewAccountAsync(RegisterNewAccountForCreationDto newAccount, string Pin, string ConfirmPin, bool trackChanges)
	{
		var account = _mapper.Map<Account>(newAccount);

		account.AccountName = $"{newAccount.FirstName} {newAccount.LastName}";

		await _repository.Account.Create(account, Pin, ConfirmPin, trackChanges);

		await _repository.SaveAsync();

		var accountToReturn = _mapper.Map<RegisterNewAccountDto>(account);

		return accountToReturn;
	}

	public async Task UpdateAccountAsync(UpdateAccountDto account, bool trackChanges, string Pin)
	{
		var accountToUpdate = _mapper.Map<Account>(account);

		_repository.Account.Update(accountToUpdate, trackChanges, account.Pin);

		await _repository.SaveAsync();	
	}

	
}
