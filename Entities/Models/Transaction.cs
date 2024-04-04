using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models;

public enum TransStatus
{
	Failed,
	Success,
	Error
}

public enum TranType
{
	Deposit,
	Withdrawal,
	Transfer
}

[Table("Transactions")]
public class Transaction
{
	
	[Key]	
	public int Id { get; set; }	
	public string? TransactionUniqueReference { get; set; }//Will generate upon every instantiation.
	public decimal TransactionAmount { get; set; }	
	public TransStatus TransactionStatus { get; set; }//Failed,Successful or Error
	public bool IsSuccessful => TransactionStatus.Equals(TransStatus.Success);//Depends on TransactionStatus
	public string? TransactionSourceAccount { get; set; }
	public string? TransactionDestinationAccount { get; set; }
	public string? TransactionParticulars { get; set; }
	public TranType TransactionType { get; set; }
	public DateTime TransactionDate { get; set; }

	//Generate TransactionUniqueReference in Constructor
	public Transaction()
	{
		TransactionUniqueReference = $"{Guid.NewGuid().ToString().Replace("-", "").Substring(1,17)}";
	}

}

