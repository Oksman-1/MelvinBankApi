using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions;

public class IdNotExistException : EntityNotExistException
{
	public IdNotExistException(int Id) : base($"Account with Id {Id} does not exist")
	{
	}
}
