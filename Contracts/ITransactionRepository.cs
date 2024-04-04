using Entities.Models;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Transaction = Entities.Models.Transaction;

namespace Contracts;

public interface ITransactionRepository
{
	Response CreateNewTransaction(Transaction transaction);
	Task<Response> FindTransactionByDate(DateTime date, bool trackChanges);
	Task<Response> MakeDeposit(string AccountNumber, string TransactionPin, decimal Amount, bool trackChanges);
	Task<Response> MakeWithdrawal(string AccountNumber, string TransactionPin, decimal Amount, bool trackChanges);
	Task<Response> MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin, bool trackChanges);
	void SaveEntity(Transaction entity);

	//IAccountService AccountService { get; }
}
