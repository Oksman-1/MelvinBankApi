using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Shared.DataTranferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Repository;

public class AuthenticateAccountNumber : RepositoryBase<Account>, IAuthenticateAccountNumber
{
	private readonly IMapper _mapper;
	public AuthenticateAccountNumber(MelvinBankContext melvinBankContext, IMapper mapper) : base(melvinBankContext)
	{
		_mapper = mapper;
	}

	public async Task<(AuthenticateDto GetAccountDto, Account Account)> AuthenticateAccountNumberAsync(string AccountNumber, string Pin, bool trackChanges)
	{
		var account = await FindByCondition(x => x.AccountNumberGenerated == AccountNumber, trackChanges)
					   .SingleOrDefaultAsync();

		if (account == null)
			throw new AccountNumberNotExistException(AccountNumber);
		

		if (!VerifyPinhash(Pin, account.PinHash, account.PinSalt))
			return (null, null);

		var accountDtoToReturn = _mapper.Map<AuthenticateDto>(account);


		return (GetAccountDto: accountDtoToReturn, Account: account);
	}

	private static bool VerifyPinhash(string Pin, byte[]? pinHash, byte[]? pinSalt)
	{
		if (string.IsNullOrWhiteSpace(Pin))
			throw new ArgumentNullException("Pin");

		using (var hmac = new HMACSHA512(pinSalt))
		{
			var computedPinHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(Pin));
			for (int i = 0; i < computedPinHash.Length; i++)
			{
				if (computedPinHash[i] != pinHash[i])
					return false;
			}
		}

		return true;
	}

	public async Task<Account> GetAccountNumberAsync(string AccountNumber, bool trackChanges)
	{
		var account = await FindByCondition(x => x.AccountNumberGenerated == AccountNumber, trackChanges)
						   .SingleOrDefaultAsync();

		if (account == null)
			return null;

		return account;
	}


}
