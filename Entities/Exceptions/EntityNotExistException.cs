﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions;

public abstract class EntityNotExistException : Exception
{
	public EntityNotExistException(string? message) : base(message)
	{
	}
}
