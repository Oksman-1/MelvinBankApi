using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions;

public sealed class MaxCurrentAccountRangeBadRequestException : BadRequestException
{
	public MaxCurrentAccountRangeBadRequestException() : base("MaxCurrentAccountBalance can't be less than or equal to MinCurrentAccountBalance.")
	{
	}
}
