using System;

namespace Infrastructure.Helpers;

public class ForbiddenException : Exception
{
    public ForbiddenException() : base() { }

    public ForbiddenException(string message) : base(message) { }
}