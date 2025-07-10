-- Declare variables for random data generation
DECLARE @FirstNameCount INT = 100;
DECLARE @LastNameCount INT = 100;
DECLARE @MiddleNameCount INT = 50;
DECLARE @TitleCount INT = 5;
DECLARE @SuffixCount INT = 4;

-- Create temporary tables to hold sample data
CREATE TABLE #FirstNames (Id INT IDENTITY, Name NVARCHAR(50));
CREATE TABLE #MiddleNames (Id INT IDENTITY, Name NVARCHAR(50));
CREATE TABLE #LastNames (Id INT IDENTITY, Name NVARCHAR(50));
CREATE TABLE #Titles (Id INT IDENTITY, Name NVARCHAR(10));
CREATE TABLE #Suffixes (Id INT IDENTITY, Name NVARCHAR(10));

-- Populate first names (mix of male and female)
INSERT INTO #FirstNames (Name) VALUES 
('John'), ('Mary'), ('Robert'), ('Jennifer'), ('Michael'), ('Linda'), ('William'), ('Elizabeth'), ('David'), ('Barbara'),
('Richard'), ('Susan'), ('Joseph'), ('Jessica'), ('Thomas'), ('Sarah'), ('Charles'), ('Karen'), ('Christopher'), ('Nancy'),
('Daniel'), ('Lisa'), ('Matthew'), ('Margaret'), ('Anthony'), ('Betty'), ('Donald'), ('Sandra'), ('Mark'), ('Ashley'),
('Paul'), ('Kimberly'), ('Steven'), ('Emily'), ('Andrew'), ('Donna'), ('Kenneth'), ('Michelle'), ('Joshua'), ('Carol'),
('Kevin'), ('Amanda'), ('Brian'), ('Melissa'), ('George'), ('Deborah'), ('Edward'), ('Stephanie'), ('Ronald'), ('Rebecca'),
('Timothy'), ('Laura'), ('Jason'), ('Sharon'), ('Jeffrey'), ('Cynthia'), ('Ryan'), ('Kathleen'), ('Jacob'), ('Helen'),
('Gary'), ('Amy'), ('Nicholas'), ('Angela'), ('Eric'), ('Shirley'), ('Stephen'), ('Anna'), ('Jonathan'), ('Brenda'),
('Larry'), ('Pamela'), ('Justin'), ('Nicole'), ('Scott'), ('Ruth'), ('Brandon'), ('Katherine'), ('Benjamin'), ('Samantha'),
('Samuel'), ('Christine'), ('Gregory'), ('Emma'), ('Frank'), ('Catherine'), ('Alexander'), ('Debra'), ('Raymond'), ('Virginia'),
('Patrick'), ('Rachel'), ('Jack'), ('Carolyn'), ('Dennis'), ('Janet'), ('Jerry'), ('Maria'), ('Tyler'), ('Heather'),
('Aaron'), ('Diane'), ('Jose'), ('Julie'), ('Adam'), ('Joyce'), ('Nathan'), ('Victoria'), ('Henry'), ('Olivia'),
('Douglas'), ('Kelly'), ('Zachary'), ('Christina'), ('Peter'), ('Lauren'), ('Kyle'), ('Joan'), ('Walter'), ('Evelyn'),
('Ethan'), ('Judith'), ('Jeremy'), ('Megan'), ('Harold'), ('Andrea'), ('Keith'), ('Cheryl'), ('Christian'), ('Hannah'),
('Roger'), ('Jacqueline'), ('Noah'), ('Martha'), ('Gerald'), ('Gloria'), ('Carl'), ('Teresa'), ('Terry'), ('Ann'),
('Sean'), ('Sara'), ('Austin'), ('Madison'), ('Arthur'), ('Frances'), ('Lawrence'), ('Kathryn'), ('Dylan'), ('Janice'),
('Jesse'), ('Jean'), ('Jordan'), ('Abigail'), ('Bryan'), ('Alice'), ('Billy'), ('Julia'), ('Joe'), ('Judy'),
('Bruce'), ('Sophia'), ('Gabriel'), ('Grace'), ('Logan'), ('Denise'), ('Albert'), ('Amber'), ('Willie'), ('Doris'),
('Alan'), ('Marilyn'), ('Juan'), ('Danielle'), ('Wayne'), ('Beverly'), ('Elijah'), ('Isabella'), ('Randy'), ('Theresa'),
('Roy'), ('Diana'), ('Vincent'), ('Natalie'), ('Ralph'), ('Brittany'), ('Eugene'), ('Charlotte'), ('Russell'), ('Marie'),
('Louis'), ('Kayla'), ('Bobby'), ('Alexis'), ('Philip'), ('Lori'), ('Johnny'), ('Tiffany');

-- Populate middle names
INSERT INTO #MiddleNames (Name) VALUES 
('A.'), ('B.'), ('C.'), ('D.'), ('E.'), ('F.'), ('G.'), ('H.'), ('I.'), ('J.'),
('K.'), ('L.'), ('M.'), ('N.'), ('O.'), ('P.'), ('Q.'), ('R.'), ('S.'), ('T.'),
('U.'), ('V.'), ('W.'), ('X.'), ('Y.'), ('Z.'), ('Anne'), ('Marie'), ('Lee'), ('Lynn'),
('James'), ('Elizabeth'), ('Rose'), ('Grace'), ('Edward'), ('Jane'), ('Louise'), ('Jean'), ('May'), ('Ray'),
('Dean'), ('Wayne'), ('Gene'), ('Paul'), ('Joe'), ('Kay'), ('Eve'), ('Claire'), ('Faith'), ('Hope');

-- Populate last names
INSERT INTO #LastNames (Name) VALUES 
('Smith'), ('Johnson'), ('Williams'), ('Brown'), ('Jones'), ('Miller'), ('Davis'), ('Garcia'), ('Rodriguez'), ('Wilson'),
('Martinez'), ('Anderson'), ('Taylor'), ('Thomas'), ('Hernandez'), ('Moore'), ('Martin'), ('Jackson'), ('Thompson'), ('White'),
('Lopez'), ('Lee'), ('Gonzalez'), ('Harris'), ('Clark'), ('Lewis'), ('Robinson'), ('Walker'), ('Perez'), ('Hall'),
('Young'), ('Allen'), ('Sanchez'), ('Wright'), ('King'), ('Scott'), ('Green'), ('Baker'), ('Adams'), ('Nelson'),
('Hill'), ('Ramirez'), ('Campbell'), ('Mitchell'), ('Roberts'), ('Carter'), ('Phillips'), ('Evans'), ('Turner'), ('Torres'),
('Parker'), ('Collins'), ('Edwards'), ('Stewart'), ('Flores'), ('Morris'), ('Nguyen'), ('Murphy'), ('Rivera'), ('Cook'),
('Rogers'), ('Morgan'), ('Peterson'), ('Cooper'), ('Reed'), ('Bailey'), ('Bell'), ('Gomez'), ('Kelly'), ('Howard'),
('Ward'), ('Cox'), ('Diaz'), ('Richardson'), ('Wood'), ('Watson'), ('Brooks'), ('Bennett'), ('Gray'), ('James'),
('Reyes'), ('Cruz'), ('Hughes'), ('Price'), ('Myers'), ('Long'), ('Foster'), ('Sanders'), ('Ross'), ('Morales'),
('Powell'), ('Sullivan'), ('Russell'), ('Ortiz'), ('Jenkins'), ('Gutierrez'), ('Perry'), ('Butler'), ('Barnes'), ('Fisher');

-- Populate titles
INSERT INTO #Titles (Name) VALUES ('Mr.'), ('Mrs.'), ('Ms.'), ('Dr.'), ('Prof.');

-- Populate suffixes
INSERT INTO #Suffixes (Name) VALUES ('Jr.'), ('Sr.'), ('II'), ('III');

-- Insert 1000 random records
DECLARE @i INT = 0;
WHILE @i < 1000
BEGIN
    DECLARE @Title NVARCHAR(10) = NULL;
    DECLARE @Suffix NVARCHAR(10) = NULL;
    DECLARE @MiddleName NVARCHAR(50) = NULL;
    
    -- Randomly decide if we should include title (about 70% chance)
    IF CAST(RAND() * 100 AS INT) % 100 < 70
    BEGIN
        SELECT @Title = Name FROM #Titles WHERE Id = CAST(RAND() * @TitleCount + 1 AS INT);
    END
    
    -- Randomly decide if we should include middle name (about 50% chance)
    IF CAST(RAND() * 100 AS INT) % 100 < 50
    BEGIN
        SELECT @MiddleName = Name FROM #MiddleNames WHERE Id = CAST(RAND() * @MiddleNameCount + 1 AS INT);
    END
    
    -- Randomly decide if we should include suffix (about 15% chance)
    IF CAST(RAND() * 100 AS INT) % 100 < 15
    BEGIN
        SELECT @Suffix = Name FROM #Suffixes WHERE Id = CAST(RAND() * @SuffixCount + 1 AS INT);
    END
    
    -- Randomly decide if record should be marked as deleted (about 5% chance)
    DECLARE @IsDeleted BIT = CASE WHEN CAST(RAND() * 100 AS INT) % 100 < 5 THEN 1 ELSE 0 END;
    
    -- Randomly decide if we should set UpdatedAt (about 30% chance)
    DECLARE @UpdatedAt DATETIME = NULL;
    IF CAST(RAND() * 100 AS INT) % 100 < 30
    BEGIN
        SET @UpdatedAt = DATEADD(DAY, -CAST(RAND() * 365 AS INT), GETDATE());
    END
    
    INSERT INTO Person (FirstName, MiddleName, LastName, Title, Suffix, UpdatedAt, IsDeleted)
    SELECT 
        (SELECT Name FROM #FirstNames WHERE Id = CAST(RAND() * @FirstNameCount + 1 AS INT)),
        @MiddleName,
        (SELECT Name FROM #LastNames WHERE Id = CAST(RAND() * @LastNameCount + 1 AS INT)),
        @Title,
        @Suffix,
        @UpdatedAt,
        @IsDeleted;
    
    SET @i = @i + 1;
END

-- Clean up temporary tables
DROP TABLE #FirstNames;
DROP TABLE #MiddleNames;
DROP TABLE #LastNames;
DROP TABLE #Titles;
DROP TABLE #Suffixes;

-- Verify the insert
SELECT COUNT(*) AS TotalRecords FROM Person;