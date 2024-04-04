using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions;

public class PhoneNumberAccountExistsException : EntityExistsException
{
	public PhoneNumberAccountExistsException(string? PhoneNumber) : base($"An Account with this Phone Number {PhoneNumber} already Exists!")
	{
	}
}
