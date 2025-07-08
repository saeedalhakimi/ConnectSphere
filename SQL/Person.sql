-- Stores person type details (e.g., Employee, Contractor)
CREATE TABLE PersonType (
    PersonTypeId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Name NVARCHAR(50) NOT NULL, -- e.g., Employee, Contractor, Vendor
    CONSTRAINT CHK_PersonType_Name CHECK (Name <> ''),
    CONSTRAINT UQ_PersonType_Name UNIQUE (Name)
);

-- Populate PersonType table with common person types for HR and departments
INSERT INTO PersonType (Name)
VALUES
    ('Employee'),
    ('Contractor'),
    ('Intern'),
    ('Vendor'),
    ('Consultant'),
    ('Volunteer'),
    ('Temporary'),
    ('Freelancer'),
    ('Executive'),
    ('Applicant');

-- Stores core person details
CREATE TABLE Person (
    PersonId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    FirstName NVARCHAR(50) NOT NULL,
    MiddleName NVARCHAR(50) NULL,
    LastName NVARCHAR(50) NOT NULL,
    Title NVARCHAR(10) NULL, -- e.g., Mr., Ms., Dr.
    Suffix NVARCHAR(10) NULL, -- e.g., Jr., Sr.
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT CHK_Person_FirstName CHECK (FirstName <> ''),
    CONSTRAINT CHK_Person_LastName CHECK (LastName <> ''),
    CONSTRAINT CHK_Person_Title CHECK (Title IS NULL OR Title <> ''),
    CONSTRAINT CHK_Person_Suffix CHECK (Suffix IS NULL OR Suffix <> '')
);


CREATE NONCLUSTERED INDEX IX_Person_LastName_FirstName ON Person(LastName, FirstName);


CREATE PROCEDURE SP_CreatePerson
	@PersonId UNIQUEIDENTIFIER,
    @FirstName NVARCHAR(50),
    @MiddleName NVARCHAR(50) = NULL,
    @LastName NVARCHAR(50),
    @Title NVARCHAR(10) = NULL,
    @Suffix NVARCHAR(10) = NULL,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME = NULL,
    @IsDeleted BIT,
    @CorrelationId NVARCHAR(36) = NULL
AS
BEGIN
    BEGIN TRY
        -- Insert into Person table
        INSERT INTO dbo.Person (
            PersonId,
            FirstName,
            MiddleName,
            LastName,
            Title,
            Suffix,
            CreatedAt,
            UpdatedAt,
            IsDeleted
        )
        VALUES (
            @PersonId,
            @FirstName,
            @MiddleName,
            @LastName,
            @Title,
            @Suffix,
            @CreatedAt,
            @UpdatedAt,
            @IsDeleted
        );
    END TRY
    BEGIN CATCH
        -- Capture error details
        DECLARE @ErrorNumber INT = ERROR_NUMBER();
        DECLARE @ErrorMessage NVARCHAR(MAX) = ERROR_MESSAGE();
        DECLARE @ErrorProcedure NVARCHAR(128) = ERROR_PROCEDURE();
        DECLARE @ErrorLine INT = ERROR_LINE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        DECLARE @AdditionalInfo NVARCHAR(MAX) = 
            'Parameters: PersonId=' + CAST(@PersonId AS NVARCHAR(36)) + 
            ', FirstName=' + ISNULL(@FirstName, 'NULL') + 
            ', LastName=' + ISNULL(@LastName, 'NULL') + 
            ', CorrelationId=' + ISNULL(@CorrelationId, 'NULL');

        -- Log error to ErrorLog table
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

        -- Re-throw the error to the caller
        THROW;
    END CATCH
END;
GO