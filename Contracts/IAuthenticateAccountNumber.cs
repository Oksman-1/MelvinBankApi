using Entities.Models;
using Shared.DataTranferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts;

public interface IAuthenticateAccountNumber
{
	Task<(AuthenticateDto GetAccountDto, Account Account)> AuthenticateAccountNumberAsync(string AccountNumber, string Pin, bool trackChanges);
	Task<Account> GetAccountNumberAsync(string AccountNumber, bool trackChanges);
}
