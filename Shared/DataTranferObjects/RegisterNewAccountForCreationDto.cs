using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTranferObjects;

public record RegisterNewAccountForCreationDto
{
	[Required(ErrorMessage = "FirstName is a required field.")]
	public string? FirstName { get; init; }

	[Required(ErrorMessage = "LastName is a required field.")]
	public string? LastName { get; init; }
	//public string? AccountName { get; init; }

	[MaxLength(11, ErrorMessage = "Maximum Length for the PhoneNumber is 11 Characters")]
	public string? PhoneNumber { get; init; }

	[Required(ErrorMessage = "Email is a required field.")]
	[EmailAddress(ErrorMessage = "Invalid email address.")]
	public string? Email { get; init; }

	//Account Info
	//public decimal CurrentAccountBalance { get; set; }
	public AccountType AccountType { get; set; }//This is the account type (Savings or Current).
												//public string? AccountNumberGenerated { get; set; }

	//To store the Hash and Salt of the Account Transaction Pin
	//public byte[]? PinHash { get; set; }
	//public byte[]? PinSalt { get; set; }
	public DateTime DateCreated { get; set; }
	public DateTime DateLastUpdated { get; set; }

	[Required(ErrorMessage = "Pin is a required field.")]
	[RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Pin Must Not be more than 4 digits.")]//A four-digit pin
	public string? Pin { get; set; }

	//[Compare("Pin", ErrorMessage = "Pin do not match")]//To compare both Pins
	public string? ConfirmPin { get; set; }
}

public enum AccountType
{
	Savings,
	Current,
	Corporate,
	Government
}