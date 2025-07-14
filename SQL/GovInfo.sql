-- Stores governmental information for a person (one-to-many with Person)
CREATE TABLE GovernmentalInfo (
    GovernmentalInfoID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    PersonId UNIQUEIDENTIFIER NOT NULL,
    CountryId UNIQUEIDENTIFIER NOT NULL, -- References Country for nationality
    GovIdNumber NVARCHAR(50) NULL,
    PassportNumber NVARCHAR(50) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (PersonId) REFERENCES Person(PersonId),
    FOREIGN KEY (CountryId) REFERENCES Country(CountryId)
);

CREATE NONCLUSTERED INDEX IX_GovernmentalInfo_PersonId ON GovernmentalInfo(PersonId);
CREATE NONCLUSTERED INDEX IX_GovernmentalInfo_CountryId ON GovernmentalInfo(CountryId);
GO

ALTER TABLE GovernmentalInfo
DROP CONSTRAINT CHK_GovernmentalInfo_PassportNumber;

USE [ConnectSphereDb]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[SP_CreateGovernmentalInfo]
    @GovernmentalInfoId UNIQUEIDENTIFIER,
    @PersonId UNIQUEIDENTIFIER,
    @CountryId UNIQUEIDENTIFIER,
    @GovIdNumber NVARCHAR(50) = NULL,
    @PassportNumber NVARCHAR(50) = NULL,
    @CorrelationId NVARCHAR(36) = NULL
AS
BEGIN
    BEGIN TRY
        INSERT INTO [dbo].[GovernmentalInfo] (
            GovernmentalInfoId,
            PersonId,
            CountryId,
            GovIdNumber,
            PassportNumber,
            CreatedAt,
            UpdatedAt,
            IsDeleted
        )
        VALUES (
            @GovernmentalInfoId,
            @PersonId,
            @CountryId,
            @GovIdNumber,
            @PassportNumber,
            GETUTCDATE(),
            NULL,
            0
        );
    END TRY
    BEGIN CATCH
        DECLARE @ErrorNumber INT = ERROR_NUMBER();
        DECLARE @ErrorMessage NVARCHAR(MAX) = ERROR_MESSAGE();
        DECLARE @ErrorProcedure NVARCHAR(128) = ERROR_PROCEDURE();
        DECLARE @ErrorLine INT = ERROR_LINE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        DECLARE @AdditionalInfo NVARCHAR(MAX) = 
            'Parameters: GovernmentalInfoId=' + CAST(@GovernmentalInfoId AS NVARCHAR(36)) + 
            ', PersonId=' + CAST(@PersonId AS NVARCHAR(36)) + 
            ', CountryId=' + CAST(@CountryId AS NVARCHAR(36)) + 
            ', GovIdNumber=' + ISNULL(@GovIdNumber, 'NULL') + 
            ', PassportNumber=' + ISNULL(@PassportNumber, 'NULL') + 
            ', CorrelationId=' + ISNULL(@CorrelationId, 'NULL');

        INSERT INTO dbo.ErrorLog (
            ErrorNumber,
            ErrorMessage,
            ErrorProcedure,
            ErrorLine,
            ErrorSeverity,
            ErrorState,
            CorrelationId,
            AdditionalInfo
        )
        VALUES (
            @ErrorNumber,
            @ErrorMessage,
            @ErrorProcedure,
            @ErrorLine,
            @ErrorSeverity,
            @ErrorState,
            @CorrelationId,
            @AdditionalInfo
        );

        THROW;
    END CATCH
END;