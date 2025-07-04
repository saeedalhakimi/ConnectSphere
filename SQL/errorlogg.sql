CREATE TABLE dbo.ErrorLog (
        ErrorLogId BIGINT IDENTITY(1,1) PRIMARY KEY,
        ErrorNumber INT NOT NULL,
        ErrorMessage NVARCHAR(MAX) NOT NULL,
        ErrorProcedure NVARCHAR(128) NULL,
        ErrorLine INT NULL,
        ErrorSeverity INT NULL,
        ErrorState INT NULL,
        CorrelationId NVARCHAR(36) NULL, -- Stores GUID as string for application correlation
        ErrorTimestamp DATETIME NOT NULL DEFAULT GETDATE(),
        AdditionalInfo NVARCHAR(MAX) NULL -- Optional details (e.g., parameter values)
    );