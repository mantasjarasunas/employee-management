using System;

namespace Infrastructure.Helpers;

public class NotFoundException : Exception
{
    public NotFoundException() : base() { }

    public NotFoundException(string message) : base(message) { }
}