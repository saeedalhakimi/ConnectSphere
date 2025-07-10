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

CREATE OR ALTER PROCEDURE SP_GetPersonById
    @PersonId UNIQUEIDENTIFIER,
    @CorrelationId NVARCHAR(36) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        -- Validate PersonId
        IF @PersonId IS NULL OR @PersonId = '00000000-0000-0000-0000-000000000000'
        BEGIN
            THROW 50001, 'PersonId cannot be null or empty.', 1;
        END

        -- Fetch person
        SELECT 
            PersonId,
            Title,
            FirstName,
            MiddleName,
            LastName,
            Suffix,
            CreatedAt,
            UpdatedAt,
            IsDeleted
        FROM [dbo].[Person]
        WHERE PersonId = @PersonId AND IsDeleted = 0;

        IF @@ROWCOUNT = 0
        BEGIN
            THROW 50002, 'Person not found.', 1;
        END
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
            'Parameters: PersonId=' + ISNULL(CAST(@PersonId AS NVARCHAR(36)), 'NULL') + 
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

CREATE OR ALTER PROCEDURE [dbo].[SP_GetAllPersons]
    @PageNumber INT,
    @PageSize INT,
    @CorrelationId NVARCHAR(36) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF @PageNumber < 1 OR @PageSize < 1
        BEGIN
            THROW 50004, 'PageNumber and PageSize must be greater than 0.', 1;
        END

        DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

        SELECT 
            PersonId,
            Title,
            FirstName,
            MiddleName,
            LastName,
            Suffix,
            CreatedAt,
            UpdatedAt,
            IsDeleted
        FROM [dbo].[Person]
        WHERE IsDeleted = 0
        ORDER BY PersonId
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY;

        IF @@ROWCOUNT = 0
        BEGIN
            THROW 50003, 'No persons found for the specified page.', 1;
        END
    END TRY
    BEGIN CATCH
        DECLARE @ErrorNumber INT = ERROR_NUMBER();
        DECLARE @ErrorMessage NVARCHAR(MAX) = ERROR_MESSAGE();
        DECLARE @ErrorProcedure NVARCHAR(128) = ERROR_PROCEDURE();
        DECLARE @ErrorLine INT = ERROR_LINE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        DECLARE @AdditionalInfo NVARCHAR(MAX) = 
            'Parameters: PageNumber=' + CAST(@PageNumber AS NVARCHAR(10)) + 
            ', PageSize=' + CAST(@PageSize AS NVARCHAR(10)) + 
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
GO


CREATE OR ALTER PROCEDURE [dbo].[SP_DeletePerson]
    @PersonId UNIQUEIDENTIFIER,
    @CorrelationId NVARCHAR(36) = NULL,
    @RowsAffected INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE [dbo].[Person]
        SET IsDeleted = 1,
            UpdatedAt = GETUTCDATE()
        WHERE PersonId = @PersonId AND IsDeleted = 0;

        SET @RowsAffected = @@ROWCOUNT;

        
    END TRY
    BEGIN CATCH
        DECLARE @ErrorNumber INT = ERROR_NUMBER();
        DECLARE @ErrorMessage NVARCHAR(MAX) = ERROR_MESSAGE();
        DECLARE @ErrorProcedure NVARCHAR(128) = ERROR_PROCEDURE();
        DECLARE @ErrorLine INT = ERROR_LINE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        DECLARE @AdditionalInfo NVARCHAR(MAX) = 
            'Parameters: PersonId=' + CAST(@PersonId AS NVARCHAR(36)) + 
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
GO


CREATE OR ALTER PROCEDURE [dbo].[SP_UpdatePersonName]
    @PersonId UNIQUEIDENTIFIER,
    @Title NVARCHAR(10) = NULL,
    @FirstName NVARCHAR(50),
    @MiddleName NVARCHAR(50) = NULL,
    @LastName NVARCHAR(50),
    @Suffix NVARCHAR(10) = NULL,
    @CorrelationId NVARCHAR(36) = NULL,
    @RowsAffected INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF @FirstName IS NULL OR @LastName IS NULL
        BEGIN
            THROW 50006, 'FirstName and LastName cannot be null.', 1;
        END

        UPDATE [dbo].[Person]
        SET Title = @Title,
            FirstName = @FirstName,
            MiddleName = @MiddleName,
            LastName = @LastName,
            Suffix = @Suffix,
            UpdatedAt = GETUTCDATE()
        WHERE PersonId = @PersonId AND IsDeleted = 0;

        SET @RowsAffected = @@ROWCOUNT;

        IF @RowsAffected = 0
        BEGIN
            THROW 50005, 'Person not found or already deleted.', 1;
        END
    END TRY
    BEGIN CATCH
        DECLARE @ErrorNumber INT = ERROR_NUMBER();
        DECLARE @ErrorMessage NVARCHAR(MAX) = ERROR_MESSAGE();
        DECLARE @ErrorProcedure NVARCHAR(128) = ERROR_PROCEDURE();
        DECLARE @ErrorLine INT = ERROR_LINE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        DECLARE @AdditionalInfo NVARCHAR(MAX) = 
            'Parameters: PersonId=' + CAST(@PersonId AS NVARCHAR(36)) + 
            ', Title=' + ISNULL(@Title, 'NULL') + 
            ', FirstName=' + ISNULL(@FirstName, 'NULL') + 
            ', MiddleName=' + ISNULL(@MiddleName, 'NULL') + 
            ', LastName=' + ISNULL(@LastName, 'NULL') + 
            ', Suffix=' + ISNULL(@Suffix, 'NULL') + 
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
