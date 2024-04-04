using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.DataTranferObjects;

public record RegisterNewAccountDto : RegisterNewAccountForCreationDto
{


	//[Required(ErrorMessage = "FirstName is a required field.")]
	//public string? FirstName { get; set; }

	//[Required(ErrorMessage = "LastName is a required field.")]
	//public string? LastName { get; set; }

	////public string? AccountName { get; set; }
	//public string? PhoneNumber { get; set; }
	//public string? Email { get; set; }

	//Account Info
	//public decimal CurrentAccountBalance { get; set; }
	//public AccountType AccountType { get; set; }//This is the account type (Savings or Current).
	//public string? AccountNumberGenerated { get; set; }

	//To store the Hash and Salt of the Account Transaction Pin
	[JsonIgnore]
	public byte[]? PinHash { get; set; }

	[JsonIgnore]
	public byte[]? PinSalt { get; set; }
	//public DateTime DateCreated { get; set; }
	//public DateTime DateLastUpdated { get; set; }

	//[Required(ErrorMessage = "Pin is a required field.")]
	//[RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Pin Must Not be more than 4 digits.")]//A four-digit pin
	//public string? Pin { get; set; }

	//[Compare("Pin", ErrorMessage = "Pin do not match")]//To compare both Pins
	//public string? ConfirmPin { get; set; }


}


