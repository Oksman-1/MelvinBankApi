using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTranferObjects;

public record TransactionRequestDto
{

	public decimal TransactionAmount { get; set; }
	public string? TransactionSourceAccount { get; set; }
	public string? TransactionDestinationAccount { get; set; }
	public TranType TransactionType { get; set; }
	public DateTime TransactionDate { get; set; }
}

public enum TranType
{
	Deposit,
	Withdrawal,
	Transfer
}