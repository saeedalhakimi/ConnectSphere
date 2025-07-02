-- Stores email address type details (e.g., Personal, Work, Alternate)
CREATE TABLE EmailAddressType (
    EmailAddressTypeId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Name NVARCHAR(50) NOT NULL, -- e.g., Personal, Work, Alternate
    Description NVARCHAR(200) NULL, -- Optional context, e.g., "Primary personal email"
    CONSTRAINT CHK_EmailAddressType_Name CHECK (Name <> ''),
    CONSTRAINT UQ_EmailAddressType_Name UNIQUE (Name)
);

-- Populate EmailAddressType with common types
INSERT INTO EmailAddressType (Name, Description)
VALUES
    ('Personal', 'Primary personal email address'),
    ('Work', 'Professional or office email address'),
    ('Alternate', 'Secondary or backup email address'),
    ('Billing', 'Email for financial correspondence'),
    ('Emergency', 'Email for emergency contact purposes'),
    ('Other', 'Miscellaneous or unspecified email address');

-- Stores email address details for a person (one-to-many with Person)
CREATE TABLE EmailAddress (
    EmailAddressId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    PersonId UNIQUEIDENTIFIER NOT NULL,
    EmailAddressTypeId UNIQUEIDENTIFIER NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT CHK_EmailAddress_Email CHECK (Email <> '' AND Email LIKE '%@%.%'),
    CONSTRAINT UQ_EmailAddress_PersonId_EmailAddressTypeId_Email UNIQUE (PersonId, EmailAddressTypeId, Email),
    FOREIGN KEY (PersonId) REFERENCES Person(PersonId),
    FOREIGN KEY (EmailAddressTypeId) REFERENCES EmailAddressType(EmailAddressTypeId)
);

CREATE NONCLUSTERED INDEX IX_EmailAddress_PersonId ON EmailAddress(PersonId);
CREATE NONCLUSTERED INDEX IX_EmailAddress_EmailAddressTypeId ON EmailAddress(EmailAddressTypeId);
CREATE NONCLUSTERED INDEX IX_EmailAddress_PersonId_EmailAddressTypeId ON EmailAddress(PersonId, EmailAddressTypeId);