using AutoMapper;
using Contracts;
using Entities.ConfigurationModels;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Service.Contracts;
using Shared.DataTranferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service;

public sealed class ServiceManager : IServiceManager
{
	
	private readonly Lazy<IAccountService> _accountService;
	private readonly Lazy<ITransactionService> _transactionService;
	private readonly Lazy<IAuthenticationService> _authenticationService;

	public ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger, IMapper mapper/*, IDataShaper<GetAccountDto> dataShaper*//*, IOptions<Appsettings> settings*//*, IAccountService service,*/ /*ITransactionRepository transactionRepository*/ /*IRepositoryBase<Transaction> repositoryBase*/, IAccountLinks accountLinks, UserManager<User> userManager, IOptions<JwtConfiguration> configuration)
	{
		
		_accountService = new Lazy<IAccountService>(() => new AccountService(repositoryManager, logger, mapper, accountLinks/*, dataShaper*/));
		_transactionService = new Lazy<ITransactionService>(() => new TransactionService(repositoryManager, logger, mapper/*, settings*//*, service, transactionRepository*//* repositoryBase*/));
		_authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(logger, mapper, userManager, configuration));
	}

	public IAccountService AccountService => _accountService.Value;
	public ITransactionService TransactionService => _transactionService.Value;
	public IAuthenticationService AuthenticationService => _authenticationService.Value;
}
