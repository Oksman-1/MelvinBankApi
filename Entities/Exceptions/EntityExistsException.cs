using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions;

public abstract class EntityExistsException : Exception
{
	public EntityExistsException(string? message) : base(message)
	{
	}
}
