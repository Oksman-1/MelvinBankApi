using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTranferObjects;

public record GetAccountDto
{
	[Key]
	public int Id { get; set; }

	//Personal Info
	[Required(ErrorMessage = "FirstName is a required field.")]
	public string? FirstName { get; set; }

	[Required(ErrorMessage = "LastName is a required field.")]
	public string? LastName { get; set; }
	public string? AccountName { get; set; }
	public string? PhoneNumber { get; set; }
	public string? Email { get; set; }
	public decimal CurrentAccountBalance { get; set; }
	public AccountType AccountType { get; set; }//This is the account type (Savings or Current).
	public string? AccountNumberGenerated { get; set; }
	public DateTime DateCreated { get; set; }
	public DateTime DateLastUpdated { get; set; }
}
