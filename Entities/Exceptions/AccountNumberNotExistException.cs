using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions;

public class AccountNumberNotExistException : EntityNotExistException
{
	public AccountNumberNotExistException(string? AccountNumber) : base($"An Account with Account Number: {AccountNumber} does not exist!")
	{
	}
}
