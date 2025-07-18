﻿using ConnectSphere.API.Domain.Aggregate;
using ConnectSphere.API.Domain.Common.Models;
using ConnectSphere.API.Domain.Entities.Persons;
using ConnectSphere.API.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.DomainRepositories.IRepositories
{
    public interface IPersonRepository
    {
        /// <summary>
        /// Creates a new person and persists it, including any related entities and domain events.
        /// </summary>
        /// <param name="person">The person to create.</param>
        /// <param name="correlationId">The correlation ID for tracing the operation.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>An OperationResult containing the created person or errors.</returns>
        Task<OperationResult<Person>> CreateAsync(Person person, string? correlationId, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an existing person, including related entities and domain events.
        /// </summary>
        /// <param name="person">The person to update.</param>
        /// <param name="correlationId">The correlation ID for tracing the operation.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>An OperationResult indicating success or errors.</returns>
        Task<OperationResult<bool>> UpdateAsync(Person person, string? correlationId, CancellationToken cancellationToken);

        /// <summary>
        /// Soft-deletes a person by setting IsDeleted to true.
        /// </summary>
        /// <param name="personId">The ID of the person to delete.</param>
        /// <param name="correlationId">The correlation ID for tracing the operation.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>An OperationResult indicating success or errors.</returns>
        Task<OperationResult<bool>> DeleteAsync(Guid personId, string? correlationId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a person by ID, including related entities (Addresses, PhoneNumbers, EmailAddresses, GovernmentalInfos, BirthDetails).
        /// </summary>
        /// <param name="personId">The ID of the person to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>An OperationResult containing the person or errors.</returns>
        Task<OperationResult<Person>> GetByIdAsync(Guid personId, string? correlationId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves all non-deleted persons, optionally including related entities.
        /// </summary>
        /// <param name="pageNumber"> The page number for pagination.</param>
        /// <param name="pageSize"> The number of persons per page.</param>
        /// <param name="correlationId"> The correlation ID for tracing the operation.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>An OperationResult containing a list of persons or errors.</returns>
        Task<OperationResult<IReadOnlyList<Person>>> GetAllAsync(int pageNumber, int pageSize, string? correlationId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the count of non-deleted persons.
        /// </summary>
        /// <param name="cancellationToken"> The cancellation token to cancel the operation.</param>
        /// <param name="correlationId"> The correlation ID for tracing the operation.</param>
        /// <returns>An OperationResult containing the count of persons or errors.</returns>
        Task<OperationResult<int>> GetCountAsync(string? correlationId, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the name of a person.
        /// </summary>
        /// <param name="personId"> The ID of the person whose name is to be updated.</param>
        /// <param name="newName"> The new name to set for the person.</param>
        /// <param name="correlationId"> The correlation ID for tracing the operation.</param>
        /// <param name="cancellationToken"> The cancellation token to cancel the operation.</param>
        /// <returns>An OperationResult bool indicating success or errors.</returns>
        Task<OperationResult<bool>> UpdateNameAsync(Guid personId, PersonName newName, string? correlationId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a person by email address.
        /// </summary>
        /// <param name="email">The email address to search for.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>An OperationResult containing the person or errors.</returns>
        Task<OperationResult<Person>> GetByEmailAsync(Email email, CancellationToken cancellationToken);

        
        /// <summary>
        /// Retrieves a country by ID.
        /// </summary>
        /// <param name="countryId">The ID of the country.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>An OperationResult containing the country or errors.</returns>
        Task<OperationResult<Country>> GetCountryByIdAsync(Guid countryId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves an address type by ID.
        /// </summary>
        /// <param name="addressTypeId">The ID of the address type.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>An OperationResult containing the address type or errors.</returns>
        Task<OperationResult<AddressType>> GetAddressTypeByIdAsync(Guid addressTypeId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a phone number type by ID.
        /// </summary>
        /// <param name="phoneNumberTypeId">The ID of the phone number type.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>An OperationResult containing the phone number type or errors.</returns>
        Task<OperationResult<PhoneNumberType>> GetPhoneNumberTypeByIdAsync(Guid phoneNumberTypeId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves an email address type by ID.
        /// </summary>
        /// <param name="emailAddressTypeId">The ID of the email address type.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>An OperationResult containing the email address type or errors.</returns>
        Task<OperationResult<EmailAddressType>> GetEmailAddressTypeByIdAsync(Guid emailAddressTypeId, CancellationToken cancellationToken);
    }
}
