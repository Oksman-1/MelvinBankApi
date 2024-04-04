using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Shared.DataTranferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts;

public interface IRepositoryManager
{
	IAccountRepository Account { get; }
	ITransactionRepository Transaction { get; }
	Task SaveAsync();
	EntityState GetEntityState(Account account);
}
