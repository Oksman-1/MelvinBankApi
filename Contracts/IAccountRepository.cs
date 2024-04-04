using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts;

public interface IAccountRepository
{
	//Task<Account> Authenticate(string AccountNumber, string Pin, bool trackChanges);
	Task<IEnumerable<Account>> GetAllAccountsFromDb(bool trackChanges);
	Task<PagedList<Account>> GetAllAccounts(AccountsParameters accountsParameters, bool trackChanges);
	Task<Account> Create(Account account, string Pin, string ConfirmPin, bool trackChanges);
	void Update(Account account, bool trackChanges, string Pin = null);
	void Delete(int Id, bool trackChanges);
	Task<Account> GetById(int Id, bool trackChanges);
	Task<Account> GetByAccountNumber(string AccountNumber, bool trackChanges);

	//EntityState GetEntityState
}
