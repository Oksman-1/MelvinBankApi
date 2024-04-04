using Entities.Models;
using Shared.DataTranferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts;

public interface ITransactionService
{
	Response GetNewTransaction(TransactionRequestDto transactionRequest);
	Task<Response> MakeDepositAsync(string AccountNumber, string TransactionPin, decimal Amount, bool trackChanges);
	Task<Response> MakeWithdrawal(string AccountNumber, string TransactionPin, decimal Amount, bool trackChanges);
	Task<Response> MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin, bool trackChanges);
}
