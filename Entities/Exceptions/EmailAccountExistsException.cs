using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions;

public class EmailAccountExistsException : EntityExistsException
{
	public EmailAccountExistsException(string? Email) : base($"An Account with this Email: {Email} already exists!")
	{
	}
}
