-- Stores address type details (e.g., Home, Work)
CREATE TABLE AddressType (
    AddressTypeId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Name NVARCHAR(50) NOT NULL, -- e.g., Home, Work, Billing
    Description NVARCHAR(200) NULL, -- Optional context, e.g., "Primary residence"
    CONSTRAINT CHK_AddressType_Name CHECK (Name <> ''),
    CONSTRAINT UQ_AddressType_Name UNIQUE (Name)
);

-- Populate AddressType with common types
INSERT INTO AddressType (Name, Description)
VALUES
    ('Home', 'Primary residence'),
    ('Work', 'Office or workplace address'),
    ('Billing', 'Address for invoicing or financial correspondence'),
    ('Shipping', 'Address for deliveries'),
    ('Temporary', 'Short-term or seasonal address'),
    ('Emergency', 'Address for emergency contact purposes'),
    ('Other', 'Miscellaneous or unspecified address');

-- Stores address details for a person (one-to-many with Person)
CREATE TABLE Address (
    AddressId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    PersonId UNIQUEIDENTIFIER NOT NULL,
    AddressTypeId UNIQUEIDENTIFIER NOT NULL,
    AddressLine1 NVARCHAR(100) NOT NULL,
    AddressLine2 NVARCHAR(100) NULL,
    City NVARCHAR(100) NOT NULL,
    PostalCode NVARCHAR(20) NULL,
    CountryId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT CHK_Address_AddressLine1 CHECK (AddressLine1 <> ''),
    CONSTRAINT CHK_Address_City CHECK (City <> ''),
    CONSTRAINT CHK_Address_PostalCode CHECK (PostalCode IS NULL OR PostalCode <> ''),
    FOREIGN KEY (PersonId) REFERENCES Person(PersonId),
    FOREIGN KEY (AddressTypeId) REFERENCES AddressType(AddressTypeId),
    FOREIGN KEY (CountryId) REFERENCES Country(CountryId)
);

CREATE NONCLUSTERED INDEX IX_Address_PersonId ON Address(PersonId);
CREATE NONCLUSTERED INDEX IX_Address_AddressTypeId ON Address(AddressTypeId);
CREATE NONCLUSTERED INDEX IX_Address_CountryId ON Address(CountryId);
CREATE NONCLUSTERED INDEX IX_Address_PersonId_AddressTypeId ON Address(PersonId, AddressTypeId);