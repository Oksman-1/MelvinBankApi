using Contracts;
using Entities.ConfigurationModels;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Service.Contracts;
using Shared.DataTranferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository;

public class RepositoryManager : IRepositoryManager
{
	private readonly MelvinBankContext _melvinBankContext;
	private readonly Lazy<IAccountRepository> _accountRepository;
	private readonly Lazy<ITransactionRepository> _transactionRepository;

	public RepositoryManager(MelvinBankContext melvinBankContext, ILoggerManager logger, IOptions<Appsettings> settings, IAuthenticateAccountNumber accountNumber)
	{
		_melvinBankContext = melvinBankContext;
		_accountRepository = new Lazy<IAccountRepository>(() => new AccountRepository(melvinBankContext));
		_transactionRepository = new Lazy<ITransactionRepository>(() => new TransactionRepository(melvinBankContext, logger, settings, accountNumber));
	}
	public IAccountRepository Account => _accountRepository.Value;

	public ITransactionRepository Transaction => _transactionRepository.Value;

	public async Task SaveAsync() => await _melvinBankContext.SaveChangesAsync();

	public EntityState GetEntityState(Account account) => _melvinBankContext.Entry(account).State;

}