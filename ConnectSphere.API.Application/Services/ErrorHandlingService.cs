﻿using ConnectSphere.API.Common.ILogging;
using ConnectSphere.API.Domain.Common.Enums;
using ConnectSphere.API.Domain.Common.Models;
using ConnectSphere.API.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.Services
{
    public class ErrorHandlingService : IErrorHandlingService
    {
        private readonly IAppLogger<ErrorHandlingService> _logger;

        public ErrorHandlingService(IAppLogger<ErrorHandlingService> logger)
        {
            _logger = logger;
        }

        public OperationResult<T> HandleCancelationToken<T>(OperationCanceledException ex, string? correlationId)
        {
            _logger.LogError(ex, $" {ex.Message}, {correlationId}");
            return OperationResult<T>.Failure(new Error(ErrorCode.OperationCancelled,
                $"The operation was canceled: {ex.Message}", $"{ex.Source} - {ex.ToString()}.", correlationId));
        }

        public OperationResult<T> HandleDomainValidationException<T>(DomainModelInvalidException ex, string? correlationId)
        {
            _logger.LogError(ex, "Domain validation error accoured: {errormessage}", ex.Message);
            return OperationResult<T>.Failure(new Error(ErrorCode.DomainValidationError,
                "DOMAIN_VALIDATION_ERROR",$"Domain validation error occurred: {ex.Message}, {ex.Source}", correlationId));
        }

        public OperationResult<T> HandleException<T>(Exception ex, string? correlationId)
        {
            _logger.LogError(ex, $" {ex.Message}, {correlationId}");
            return OperationResult<T>.Failure(new Error(ErrorCode.InternalServerError,
                $"An unexpected error occurred: {ex.Message}", $"{ex.Source} - {ex.ToString()}.", correlationId));
        }

        public OperationResult<T> ResourceAlreadyExists<T>(string key)
        {
            throw new NotImplementedException();
        }

        public OperationResult<T> ResourceCreationFailed<T>()
        {
            throw new NotImplementedException();
        }
    }
}
