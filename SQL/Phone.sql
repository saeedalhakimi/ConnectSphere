-- Stores phone number type details (e.g., Mobile, Work)
CREATE TABLE PhoneNumberType (
    PhoneNumberTypeId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Name NVARCHAR(50) NOT NULL, -- e.g., Mobile, Work, Home
    Description NVARCHAR(200) NULL, -- Optional context, e.g., "Primary mobile number"
    CONSTRAINT CHK_PhoneNumberType_Name CHECK (Name <> ''),
    CONSTRAINT UQ_PhoneNumberType_Name UNIQUE (Name)
);

-- Populate PhoneNumberType with common types
INSERT INTO PhoneNumberType (Name, Description)
VALUES
    ('Mobile', 'Primary mobile number'),
    ('Work', 'Office or professional contact number'),
    ('Home', 'Residential or personal landline'),
    ('Fax', 'Fax number for business use'),
    ('Emergency', 'Number for emergency contact purposes'),
    ('Other', 'Miscellaneous or unspecified phone number');

-- Stores phone number details for a person (one-to-many with Person)
CREATE TABLE PhoneNumber (
    PhoneNumberId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    PersonId UNIQUEIDENTIFIER NOT NULL,
    PhoneNumberTypeId UNIQUEIDENTIFIER NOT NULL,
    Number NVARCHAR(25) NOT NULL,
    CountryId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT CHK_PhoneNumber_Number CHECK (Number <> ''),
    CONSTRAINT UQ_PhoneNumber_PersonId_PhoneNumberTypeId_Number UNIQUE (PersonId, PhoneNumberTypeId, Number),
    FOREIGN KEY (PersonId) REFERENCES Person(PersonId),
    FOREIGN KEY (PhoneNumberTypeId) REFERENCES PhoneNumberType(PhoneNumberTypeId),
    FOREIGN KEY (CountryId) REFERENCES Country(CountryId)
);

CREATE NONCLUSTERED INDEX IX_PhoneNumber_PersonId ON PhoneNumber(PersonId);
CREATE NONCLUSTERED INDEX IX_PhoneNumber_PhoneNumberTypeId ON PhoneNumber(PhoneNumberTypeId);
CREATE NONCLUSTERED INDEX IX_PhoneNumber_CountryId ON PhoneNumber(CountryId);
CREATE NONCLUSTERED INDEX IX_PhoneNumber_PersonId_PhoneNumberTypeId ON PhoneNumber(PersonId, PhoneNumberTypeId);