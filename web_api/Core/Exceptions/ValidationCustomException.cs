using System;
using System.Collections.Generic;
using FluentValidation.Results;

namespace web_api.Core.Exceptions
{
    public class ValidationCustomException : Exception
    {
        public List<string> Errors { get; }
        public ValidationCustomException() : base("One or more validation failures have occurred.")
        {
            Errors = new List<string>();
        }

        public ValidationCustomException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            foreach (var failure in failures)
            {
                Errors.Add(failure.ErrorMessage);
            }
        }

    }
}
