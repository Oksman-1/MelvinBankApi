using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTranferObjects;

public record AuthenticateForCreationDto /*: AuthenticateDto*/
{
	[Required(ErrorMessage = "AccountNumber is a required field.")]
	[RegularExpression(@"^[0][1-9]\d{9}$|^[1-9]\d{9}$", ErrorMessage ="Account Number must be 10 characters")]
	public string? AccountNumber { get; set; }

	[Required(ErrorMessage = "Pin is a required field.")]
	public string? Pin { get; set; }
}
