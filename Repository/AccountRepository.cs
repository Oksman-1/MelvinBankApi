using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Repository;

public class AccountRepository : RepositoryBase<Account>, IAccountRepository
{
	
	public AccountRepository(MelvinBankContext melvinBankContext) : base(melvinBankContext)
	{
		
	}

	public async Task<Account> Create(Account account, string Pin, string ConfirmPin, bool trackChanges)
	{
		IEnumerable<Account> allAccounts = await FindAll(trackChanges).ToListAsync();
		//var allAccounts = GetAllAccounts(trackChanges).Result;

		if (allAccounts.Any(x => x.Email == account.Email))
			throw new EmailAccountExistsException(account.Email);

		if (!Pin.Equals((ConfirmPin)))
			throw new PinsNotMatchException();

		byte[] pinHash, pinSalt;

		CreatePinHash(Pin, out pinHash, out pinSalt);	

		account.PinHash = pinHash;
		account.PinSalt = pinSalt;

		Create(account);

		return account;

	}

	private static void CreatePinHash(string pin, out byte[] pinHash, out byte[] pinSalt)
	{
		using(var hmac = new HMACSHA512())
		{
			pinSalt = hmac.Key;
			pinHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pin));	
		}
	}

	public void Delete(int Id, bool trackChanges)
	{
		var account = FindByCondition(x => x.Id == Id, trackChanges).SingleOrDefault();
		if (account != null)
		{
			Delete(account);
		}
	}

	public async Task<IEnumerable<Account>> GetAllAccountsFromDb(bool trackChanges)
	{
		return await FindAll(trackChanges)
		.OrderBy(n => n.Id)
		//.Skip((employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
		//.Take(employeeParameters.PageSize)
		.ToListAsync();
	}

	public async Task<PagedList<Account>> GetAllAccounts(AccountsParameters accountsParameters, bool trackChanges)
	{
		var accounts = await FindAll(trackChanges)
	    .FilterAccounts(accountsParameters.MinCurrentAccountBalance, accountsParameters.MaxCurrentAccountBalance)
		.Search(accountsParameters.SearchTerm)
		.Sort(accountsParameters.OrderBy)
		//.OrderBy(n => n.Id)
		.ToListAsync();

		return PagedList<Account>.ToPagedList(accounts, accountsParameters.PageNumber, accountsParameters.PageSize);
	}



	//public async Task<PagedList<Account>> GetAllAccounts(AccountsParameters accountsParameters, bool trackChanges)
	//{
	//	var accounts = await FindAll(trackChanges)
	//	.OrderBy(n => n.Id)
	//	.ToListAsync();

	//	return PagedList<Account>.ToPagedList(accounts, accountsParameters.PageNumber, accountsParameters.PageSize);
	//}


	public async Task<Account> GetByAccountNumber(string AccountNumber, bool trackChanges)
	{
		var account = await FindByCondition(x => x.AccountNumberGenerated == AccountNumber, trackChanges)
			               .SingleOrDefaultAsync();	 

		if (account == null)
			throw new AccountNumberNotExistException(AccountNumber);

		return account;		
	}


	public async Task<Account> GetById(int Id, bool trackChanges)
	{
		var account = await FindByCondition(x => x.Id == Id, trackChanges).SingleOrDefaultAsync();

		if (account == null)
			throw new IdNotExistException(Id);

		return account;
	}


	public void Update(Account account, bool trackChanges, string Pin = null)
	{
		var accountToBeUpdated = FindByCondition(x => x.Id == account.Id, trackChanges)
					   .SingleOrDefault();

		if (accountToBeUpdated == null)
			throw new IdNotExistException(account.Id);

		IEnumerable<Account> allAccounts =  FindAll(trackChanges).ToList();

		if (!string.IsNullOrWhiteSpace(account.Email))
		{
			if (allAccounts.Any(x => x.Email == account.Email))
				throw new EmailAccountExistsException(account.Email);

			accountToBeUpdated.Email = account.Email;	
		}

		if (!string.IsNullOrWhiteSpace(account.PhoneNumber))
		{
			if (allAccounts.Any(x => x.PhoneNumber == account.PhoneNumber))
				throw new PhoneNumberAccountExistsException(account.PhoneNumber);

			accountToBeUpdated.PhoneNumber = account.PhoneNumber;
		}

		if (!string.IsNullOrWhiteSpace(Pin))
		{
			byte[] pinHash, pinSalt;

			CreatePinHash(Pin, out pinHash, out pinSalt);

			accountToBeUpdated.PinHash = pinHash;	
			accountToBeUpdated.PinSalt = pinSalt;	
				
		}

		accountToBeUpdated.DateLastUpdated = DateTime.Now;
		Update(accountToBeUpdated);	
	}

	
}
