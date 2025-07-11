using ConnectSphere.API.Domain.Common.Enums;
using ConnectSphere.API.Domain.Common.Models;
using ConnectSphere.API.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.ValueObjects
{
    public record PersonName
    {
        private const int MaxLength = 50;
        public string FirstName { get; init; }
        public string? MiddleName { get; init; }
        public string LastName { get; init; }
        public string? Title { get; init; }
        public string? Suffix { get; init; }

        private PersonName(string firstName, string? middleName, string lastName, string? title, string? suffix)
        {
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            Title = title;
            Suffix = suffix;
        }

        public static PersonName Create(string firstName, string? middleName, string lastName, string? title, string? suffix)
        {
            if (string.IsNullOrWhiteSpace(firstName)) throw new PersonNameNotValidException("First name cannot be empty.");
            if (firstName.Length > MaxLength) throw new PersonNameNotValidException($"First name cannot exceed {MaxLength} characters.");
            if (middleName != null && middleName.Length > MaxLength) throw new PersonNameNotValidException($"Middle name cannot exceed {MaxLength} characters.");
            if (string.IsNullOrWhiteSpace(lastName)) throw new PersonNameNotValidException("Last name cannot be empty.");
            if (lastName.Length > MaxLength) throw new PersonNameNotValidException($"Last name cannot exceed {MaxLength} characters.");
            if (title != null && string.IsNullOrWhiteSpace(title)) throw new PersonNameNotValidException("Title cannot be empty if provided.");
            if (suffix != null && string.IsNullOrWhiteSpace(suffix)) throw new PersonNameNotValidException("Suffix cannot be empty if provided.");

            return new PersonName(firstName, middleName, lastName, title, suffix);
        }
    }
}
