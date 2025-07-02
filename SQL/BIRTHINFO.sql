-- Stores birth details for a person (one-to-one with Person)
CREATE TABLE PersonBirthDetails (
    PersonBirthDetailsID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    PersonId UNIQUEIDENTIFIER NOT NULL UNIQUE,
    BirthDate DATE NOT NULL,
    BirthCity NVARCHAR(100) NULL,
    CountryId UNIQUEIDENTIFIER NOT NULL, -- References Country for birth country
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT CHK_PersonBirthDetails_BirthDate CHECK (BirthDate <= GETDATE() AND BirthDate >= '1900-01-01'),
    FOREIGN KEY (PersonId) REFERENCES Person(PersonId),
    FOREIGN KEY (CountryId) REFERENCES Country(CountryId)
);

CREATE NONCLUSTERED INDEX IX_PersonBirthDetails_PersonId ON PersonBirthDetails(PersonId);
CREATE NONCLUSTERED INDEX IX_PersonBirthDetails_CountryId ON PersonBirthDetails(CountryId);
CREATE NONCLUSTERED INDEX IX_PersonBirthDetails_PersonId_CountryId ON PersonBirthDetails(PersonId, CountryId);