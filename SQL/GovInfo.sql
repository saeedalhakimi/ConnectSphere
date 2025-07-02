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
    CONSTRAINT CHK_GovernmentalInfo_GovIdNumber CHECK (GovIdNumber IS NULL OR GovIdNumber <> ''),
    CONSTRAINT CHK_GovernmentalInfo_PassportNumber CHECK (PassportNumber IS NULL OR PassportNumber <> ''),
    CONSTRAINT UQ_GovernmentalInfo_CountryId_GovIdNumber UNIQUE (CountryId, GovIdNumber),
    FOREIGN KEY (PersonId) REFERENCES Person(PersonId),
    FOREIGN KEY (CountryId) REFERENCES Country(CountryId)
);

CREATE NONCLUSTERED INDEX IX_GovernmentalInfo_PersonId ON GovernmentalInfo(PersonId);
CREATE NONCLUSTERED INDEX IX_GovernmentalInfo_CountryId ON GovernmentalInfo(CountryId);