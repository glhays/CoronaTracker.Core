﻿using System;
using Xeptions;

namespace CoronaTracker.Core.Models.Countries.Exceptions
{
    public class FailedCountryStorageException : Xeption
    {
        public FailedCountryStorageException(Exception innerException)
            : base(message: "Failed country storage error occurred, contact support.", innerException)
        { }
    }
}
