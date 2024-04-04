using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts;

public interface IServiceManager
{
	IAccountService AccountService { get; }
	ITransactionService TransactionService { get; }
	IAuthenticationService AuthenticationService { get; }
}
