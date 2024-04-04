using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTranferObjects;

public record UpdateAccountDto
{
	[Key]
	public int Id { get; set; }

	[MaxLength(11, ErrorMessage = "Maximum Length for the PhoneNumber is 11 Characters")]
	public string? PhoneNumber { get; set; }

	[Required(ErrorMessage = "Email is a required field.")]
	public string? Email { get; set; }

	[Required(ErrorMessage = "Pin is a required field.")]
	[RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Pin Must Not be more than 4 digits.")]//A four-digit pin
	public string? Pin { get; set; }

	[Compare("Pin", ErrorMessage = "Pin do not match")]//To compare both Pins
	public string? ConfirmPin { get; set; }
	public DateTime DateLastUpdated { get; set; }

}
