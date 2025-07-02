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
    PersonTypeId UNIQUEIDENTIFIER NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT CHK_Person_FirstName CHECK (FirstName <> ''),
    CONSTRAINT CHK_Person_LastName CHECK (LastName <> ''),
    CONSTRAINT CHK_Person_Title CHECK (Title IS NULL OR Title <> ''),
    CONSTRAINT CHK_Person_Suffix CHECK (Suffix IS NULL OR Suffix <> ''),
    FOREIGN KEY (PersonTypeId) REFERENCES PersonType(PersonTypeId)
);

CREATE NONCLUSTERED INDEX IX_Person_PersonTypeId ON Person(PersonTypeId);
CREATE NONCLUSTERED INDEX IX_Person_LastName_FirstName ON Person(LastName, FirstName);