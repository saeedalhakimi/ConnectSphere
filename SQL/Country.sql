-- Stores country information
CREATE TABLE Country (
    CountryId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    CountryCode NVARCHAR(3) NOT NULL, -- e.g., USA, CAN
    Name NVARCHAR(100) NOT NULL, -- e.g., United States, Canada
    Continent NVARCHAR(50) NULL, -- e.g., North America
    Capital NVARCHAR(100) NULL, -- e.g., Washington D.C.
    CurrencyCode NVARCHAR(3) NULL, -- e.g., USD, CAD
    CountryDialNumber NVARCHAR(5) NULL, -- e.g., +1, +44
    CONSTRAINT CHK_Country_Code CHECK (CountryCode <> ''),
    CONSTRAINT CHK_Country_Name CHECK (Name <> '')
);

USE [ConnectSphereDb]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[SP_GetAllCountries]
    @PageNumber INT,
    @PageSize INT,
    @CorrelationId NVARCHAR(36) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF @PageNumber < 1 OR @PageSize < 1
        BEGIN
            THROW 50001, 'PageNumber and PageSize must be greater than 0.', 1;
        END

        DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

        SELECT 
            CountryId,
            CountryCode,
            Name,
            Continent,
            Capital,
            CurrencyCode,
            CountryDialNumber
        FROM [dbo].[Country]
        ORDER BY Name
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY;

        IF @@ROWCOUNT = 0
        BEGIN
            THROW 50002, 'No countries found for the specified page.', 1;
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

USE [ConnectSphereDb]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[SP_GetAllCountriesNoPagination]
    @CorrelationId NVARCHAR(36) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        SELECT 
            CountryId,
            CountryCode,
            Name,
            Continent,
            Capital,
            CurrencyCode,
            CountryDialNumber
        FROM [dbo].[Country]
        ORDER BY Name;

        IF @@ROWCOUNT = 0
        BEGIN
            THROW 50002, 'No countries found.', 1;
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
            'Parameters: CorrelationId=' + ISNULL(@CorrelationId, 'NULL');

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

USE [ConnectSphereDb]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[SP_GetCountryById]
    @CountryId UNIQUEIDENTIFIER,
    @CorrelationId NVARCHAR(36) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF @CountryId = '00000000-0000-0000-0000-000000000000'
        BEGIN
            THROW 50003, 'Invalid CountryId provided.', 1;
        END

        SELECT 
            CountryId,
            CountryCode,
            Name,
            Continent,
            Capital,
            CurrencyCode,
            CountryDialNumber
        FROM [dbo].[Country]
        WHERE CountryId = @CountryId;

        IF @@ROWCOUNT = 0
        BEGIN
            THROW 50004, 'Country not found.', 1;
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
            'Parameters: CountryId=' + CAST(@CountryId AS NVARCHAR(36)) + 
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

USE [ConnectSphereDb]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[SP_GetCountryByCode]
    @CountryCode NVARCHAR(3),
    @CorrelationId NVARCHAR(36) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF @CountryCode = '' OR @CountryCode IS NULL
        BEGIN
            THROW 50005, 'Invalid CountryCode provided.', 1;
        END

        SELECT 
            CountryId,
            CountryCode,
            Name,
            Continent,
            Capital,
            CurrencyCode,
            CountryDialNumber
        FROM [dbo].[Country]
        WHERE CountryCode = @CountryCode;

        IF @@ROWCOUNT = 0
        BEGIN
            THROW 50006, 'Country not found for the specified code.', 1;
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
            'Parameters: CountryCode=' + ISNULL(@CountryCode, 'NULL') + 
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

USE [ConnectSphereDb]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[SP_GetCountryByName]
    @Name NVARCHAR(100),
    @CorrelationId NVARCHAR(36) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF @Name = '' OR @Name IS NULL
        BEGIN
            THROW 50007, 'Invalid Country Name provided.', 1;
        END

        SELECT 
            CountryId,
            CountryCode,
            Name,
            Continent,
            Capital,
            CurrencyCode,
            CountryDialNumber
        FROM [dbo].[Country]
        WHERE Name = @Name;

        IF @@ROWCOUNT = 0
        BEGIN
            THROW 50008, 'Country not found for the specified name.', 1;
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
            'Parameters: Name=' + ISNULL(@Name, 'NULL') + 
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

-- Populate Country table with all 250 countries and territories (ISO 3166-1, 2025 data)
INSERT INTO Country (CountryCode, Name, Continent, Capital, CurrencyCode, CountryDialNumber)
VALUES
    ('AFG', 'Afghanistan', 'Asia', 'Kabul', 'AFN', '+93'),
    ('ALA', 'Aland Islands', 'Europe', 'Mariehamn', 'EUR', '+358'),
    ('ALB', 'Albania', 'Europe', 'Tirana', 'ALL', '+355'),
    ('DZA', 'Algeria', 'Africa', 'Algiers', 'DZD', '+213'),
    ('ASM', 'American Samoa', 'Oceania', 'Pago Pago', 'USD', '+1684'),
    ('AND', 'Andorra', 'Europe', 'Andorra la Vella', 'EUR', '+376'),
    ('AGO', 'Angola', 'Africa', 'Luanda', 'AOA', '+244'),
    ('AIA', 'Anguilla', 'North America', 'The Valley', 'XCD', '+1264'),
    ('ATA', 'Antarctica', 'Antarctica', NULL, NULL, NULL),
    ('ATG', 'Antigua and Barbuda', 'North America', 'Saint John''s', 'XCD', '+1268'),
    ('ARG', 'Argentina', 'South America', 'Buenos Aires', 'ARS', '+54'),
    ('ARM', 'Armenia', 'Asia', 'Yerevan', 'AMD', '+374'),
    ('ABW', 'Aruba', 'North America', 'Oranjestad', 'AWG', '+297'),
    ('AUS', 'Australia', 'Oceania', 'Canberra', 'AUD', '+61'),
    ('AUT', 'Austria', 'Europe', 'Vienna', 'EUR', '+43'),
    ('AZE', 'Azerbaijan', 'Asia', 'Baku', 'AZN', '+994'),
    ('BHS', 'Bahamas', 'North America', 'Nassau', 'BSD', '+1242'),
    ('BHR', 'Bahrain', 'Asia', 'Manama', 'BHD', '+973'),
    ('BGD', 'Bangladesh', 'Asia', 'Dhaka', 'BDT', '+880'),
    ('BRB', 'Barbados', 'North America', 'Bridgetown', 'BBD', '+1246'),
    ('BLR', 'Belarus', 'Europe', 'Minsk', 'BYN', '+375'),
    ('BEL', 'Belgium', 'Europe', 'Brussels', 'EUR', '+32'),
    ('BLZ', 'Belize', 'North America', 'Belmopan', 'BZD', '+501'),
    ('BEN', 'Benin', 'Africa', 'Porto-Novo', 'XOF', '+229'),
    ('BMU', 'Bermuda', 'North America', 'Hamilton', 'BMD', '+1441'),
    ('BTN', 'Bhutan', 'Asia', 'Thimphu', 'BTN', '+975'),
    ('BOL', 'Bolivia', 'South America', 'Sucre', 'BOB', '+591'),
    ('BES', 'Bonaire, Sint Eustatius and Saba', 'North America', 'Kralendijk', 'USD', '+599'),
    ('BIH', 'Bosnia and Herzegovina', 'Europe', 'Sarajevo', 'BAM', '+387'),
    ('BWA', 'Botswana', 'Africa', 'Gaborone', 'BWP', '+267'),
    ('BVT', 'Bouvet Island', 'Antarctica', NULL, 'NOK', NULL),
    ('BRA', 'Brazil', 'South America', 'Brasilia', 'BRL', '+55'),
    ('IOT', 'British Indian Ocean Territory', 'Asia', 'Diego Garcia', 'USD', '+246'),
    ('BRN', 'Brunei Darussalam', 'Asia', 'Bandar Seri Begawan', 'BND', '+673'),
    ('BGR', 'Bulgaria', 'Europe', 'Sofia', 'BGN', '+359'),
    ('BFA', 'Burkina Faso', 'Africa', 'Ouagadougou', 'XOF', '+226'),
    ('BDI', 'Burundi', 'Africa', 'Gitega', 'BIF', '+257'),
    ('CPV', 'Cabo Verde', 'Africa', 'Praia', 'CVE', '+238'),
    ('KHM', 'Cambodia', 'Asia', 'Phnom Penh', 'KHR', '+855'),
    ('CMR', 'Cameroon', 'Africa', 'Yaounde', 'XAF', '+237'),
    ('CAN', 'Canada', 'North America', 'Ottawa', 'CAD', '+1'),
    ('CYM', 'Cayman Islands', 'North America', 'George Town', 'KYD', '+1345'),
    ('CAF', 'Central African Republic', 'Africa', 'Bangui', 'XAF', '+236'),
    ('TCD', 'Chad', 'Africa', 'N''Djamena', 'XAF', '+235'),
    ('CHL', 'Chile', 'South America', 'Santiago', 'CLP', '+56'),
    ('CHN', 'China', 'Asia', 'Beijing', 'CNY', '+86'),
    ('CXR', 'Christmas Island', 'Oceania', 'Flying Fish Cove', 'AUD', '+61'),
    ('CCK', 'Cocos (Keeling) Islands', 'Oceania', 'West Island', 'AUD', '+61'),
    ('COL', 'Colombia', 'South America', 'Bogota', 'COP', '+57'),
    ('COM', 'Comoros', 'Africa', 'Moroni', 'KMF', '+269'),
    ('COD', 'Congo, Democratic Republic of the', 'Africa', 'Kinshasa', 'CDF', '+243'),
    ('COG', 'Congo', 'Africa', 'Brazzaville', 'XAF', '+242'),
    ('COK', 'Cook Islands', 'Oceania', 'Avarua', 'NZD', '+682'),
    ('CRI', 'Costa Rica', 'North America', 'San Jose', 'CRC', '+506'),
    ('HRV', 'Croatia', 'Europe', 'Zagreb', 'EUR', '+385'),
    ('CUB', 'Cuba', 'North America', 'Havana', 'CUP', '+53'),
    ('CUW', 'Curacao', 'North America', 'Willemstad', 'ANG', '+599'),
    ('CYP', 'Cyprus', 'Asia', 'Nicosia', 'EUR', '+357'),
    ('CZE', 'Czechia', 'Europe', 'Prague', 'CZK', '+420'),
    ('DNK', 'Denmark', 'Europe', 'Copenhagen', 'DKK', '+45'),
    ('DJI', 'Djibouti', 'Africa', 'Djibouti', 'DJF', '+253'),
    ('DMA', 'Dominica', 'North America', 'Roseau', 'XCD', '+1767'),
    ('DOM', 'Dominican Republic', 'North America', 'Santo Domingo', 'DOP', '+1809'),
    ('ECU', 'Ecuador', 'South America', 'Quito', 'USD', '+593'),
    ('EGY', 'Egypt', 'Africa', 'Cairo', 'EGP', '+20'),
    ('SLV', 'El Salvador', 'North America', 'San Salvador', 'USD', '+503'),
    ('GNQ', 'Equatorial Guinea', 'Africa', 'Malabo', 'XAF', '+240'),
    ('ERI', 'Eritrea', 'Africa', 'Asmara', 'ERN', '+291'),
    ('EST', 'Estonia', 'Europe', 'Tallinn', 'EUR', '+372'),
    ('SWZ', 'Eswatini', 'Africa', 'Mbabane', 'SZL', '+268'),
    ('ETH', 'Ethiopia', 'Africa', 'Addis Ababa', 'ETB', '+251'),
    ('FLK', 'Falkland Islands (Malvinas)', 'South America', 'Stanley', 'FKP', '+500'),
    ('FRO', 'Faroe Islands', 'Europe', 'Torshavn', 'DKK', '+298'),
    ('FJI', 'Fiji', 'Oceania', 'Suva', 'FJD', '+679'),
    ('FIN', 'Finland', 'Europe', 'Helsinki', 'EUR', '+358'),
    ('FRA', 'France', 'Europe', 'Paris', 'EUR', '+33'),
    ('GUF', 'French Guiana', 'South America', 'Cayenne', 'EUR', '+594'),
    ('PYF', 'French Polynesia', 'Oceania', 'Papeete', 'XPF', '+689'),
    ('ATF', 'French Southern Territories', 'Antarctica', 'Port-aux-Francais', 'EUR', NULL),
    ('GAB', 'Gabon', 'Africa', 'Libreville', 'XAF', '+241'),
    ('GMB', 'Gambia', 'Africa', 'Banjul', 'GMD', '+220'),
    ('GEO', 'Georgia', 'Asia', 'Tbilisi', 'GEL', '+995'),
    ('DEU', 'Germany', 'Europe', 'Berlin', 'EUR', '+49'),
    ('GHA', 'Ghana', 'Africa', 'Accra', 'GHS', '+233'),
    ('GIB', 'Gibraltar', 'Europe', 'Gibraltar', 'GIP', '+350'),
    ('GRC', 'Greece', 'Europe', 'Athens', 'EUR', '+30'),
    ('GRL', 'Greenland', 'North America', 'Nuuk', 'DKK', '+299'),
    ('GRD', 'Grenada', 'North America', 'Saint George''s', 'XCD', '+1473'),
    ('GLP', 'Guadeloupe', 'North America', 'Basse-Terre', 'EUR', '+590'),
    ('GUM', 'Guam', 'Oceania', 'Hagatna', 'USD', '+1671'),
    ('GTM', 'Guatemala', 'North America', 'Guatemala City', 'GTQ', '+502'),
    ('GGY', 'Guernsey', 'Europe', 'Saint Peter Port', 'GBP', '+44'),
    ('GIN', 'Guinea', 'Africa', 'Conakry', 'GNF', '+224'),
    ('GNB', 'Guinea-Bissau', 'Africa', 'Bissau', 'XOF', '+245'),
    ('GUY', 'Guyana', 'South America', 'Georgetown', 'GYD', '+592'),
    ('HTI', 'Haiti', 'North America', 'Port-au-Prince', 'HTG', '+509'),
    ('HMD', 'Heard Island and McDonald Islands', 'Antarctica', NULL, 'AUD', NULL),
    ('VAT', 'Holy See', 'Europe', 'Vatican City', 'EUR', '+39'),
    ('HND', 'Honduras', 'North America', 'Tegucigalpa', 'HNL', '+504'),
    ('HKG', 'Hong Kong', 'Asia', 'Hong Kong', 'HKD', '+852'),
    ('HUN', 'Hungary', 'Europe', 'Budapest', 'HUF', '+36'),
    ('ISL', 'Iceland', 'Europe', 'Reykjavik', 'ISK', '+354'),
    ('IND', 'India', 'Asia', 'New Delhi', 'INR', '+91'),
    ('IDN', 'Indonesia', 'Asia', 'Jakarta', 'IDR', '+62'),
    ('IRN', 'Iran', 'Asia', 'Tehran', 'IRR', '+98'),
    ('IRQ', 'Iraq', 'Asia', 'Baghdad', 'IQD', '+964'),
    ('IRL', 'Ireland', 'Europe', 'Dublin', 'EUR', '+353'),
    ('IMN', 'Isle of Man', 'Europe', 'Douglas', 'GBP', '+44'),
    ('ISR', 'Israel', 'Asia', 'Jerusalem', 'ILS', '+972'),
    ('ITA', 'Italy', 'Europe', 'Rome', 'EUR', '+39'),
    ('JAM', 'Jamaica', 'North America', 'Kingston', 'JMD', '+1876'),
    ('JPN', 'Japan', 'Asia', 'Tokyo', 'JPY', '+81'),
    ('JEY', 'Jersey', 'Europe', 'Saint Helier', 'GBP', '+44'),
    ('JOR', 'Jordan', 'Asia', 'Amman', 'JOD', '+962'),
    ('KAZ', 'Kazakhstan', 'Asia', 'Astana', 'KZT', '+7'),
    ('KEN', 'Kenya', 'Africa', 'Nairobi', 'KES', '+254'),
    ('KIR', 'Kiribati', 'Oceania', 'Tarawa', 'AUD', '+686'),
    ('PRK', 'Korea, Democratic People''s Republic of', 'Asia', 'Pyongyang', 'KPW', '+850'),
    ('KOR', 'Korea, Republic of', 'Asia', 'Seoul', 'KRW', '+82'),
    ('KWT', 'Kuwait', 'Asia', 'Kuwait City', 'KWD', '+965'),
    ('KGZ', 'Kyrgyzstan', 'Asia', 'Bishkek', 'KGS', '+996'),
    ('LAO', 'Lao People''s Democratic Republic', 'Asia', 'Vientiane', 'LAK', '+856'),
    ('LVA', 'Latvia', 'Europe', 'Riga', 'EUR', '+371'),
    ('LBN', 'Lebanon', 'Asia', 'Beirut', 'LBP', '+961'),
    ('LSO', 'Lesotho', 'Africa', 'Maseru', 'LSL', '+266'),
    ('LBR', 'Liberia', 'Africa', 'Monrovia', 'LRD', '+231'),
    ('LBY', 'Libya', 'Africa', 'Tripoli', 'LYD', '+218'),
    ('LIE', 'Liechtenstein', 'Europe', 'Vaduz', 'CHF', '+423'),
    ('LTU', 'Lithuania', 'Europe', 'Vilnius', 'EUR', '+370'),
    ('LUX', 'Luxembourg', 'Europe', 'Luxembourg', 'EUR', '+352'),
    ('MAC', 'Macao', 'Asia', 'Macao', 'MOP', '+853'),
    ('MDG', 'Madagascar', 'Africa', 'Antananarivo', 'MGA', '+261'),
    ('MWI', 'Malawi', 'Africa', 'Lilongwe', 'MWK', '+265'),
    ('MYS', 'Malaysia', 'Asia', 'Kuala Lumpur', 'MYR', '+60'),
    ('MDV', 'Maldives', 'Asia', 'Male', 'MVR', '+960'),
    ('MLI', 'Mali', 'Africa', 'Bamako', 'XOF', '+223'),
    ('MLT', 'Malta', 'Europe', 'Valletta', 'EUR', '+356'),
    ('MHL', 'Marshall Islands', 'Oceania', 'Majuro', 'USD', '+692'),
    ('MTQ', 'Martinique', 'North America', 'Fort-de-France', 'EUR', '+596'),
    ('MRT', 'Mauritania', 'Africa', 'Nouakchott', 'MRU', '+222'),
    ('MUS', 'Mauritius', 'Africa', 'Port Louis', 'MUR', '+230'),
    ('MYT', 'Mayotte', 'Africa', 'Mamoudzou', 'EUR', '+262'),
    ('MEX', 'Mexico', 'North America', 'Mexico City', 'MXN', '+52'),
    ('FSM', 'Micronesia', 'Oceania', 'Palikir', 'USD', '+691'),
    ('MDA', 'Moldova', 'Europe', 'Chisinau', 'MDL', '+373'),
    ('MCO', 'Monaco', 'Europe', 'Monaco', 'EUR', '+377'),
    ('MNG', 'Mongolia', 'Asia', 'Ulaanbaatar', 'MNT', '+976'),
    ('MNE', 'Montenegro', 'Europe', 'Podgorica', 'EUR', '+382'),
    ('MSR', 'Montserrat', 'North America', 'Plymouth', 'XCD', '+1664'),
    ('MAR', 'Morocco', 'Africa', 'Rabat', 'MAD', '+212'),
    ('MOZ', 'Mozambique', 'Africa', 'Maputo', 'MZN', '+258'),
    ('MMR', 'Myanmar', 'Asia', 'Naypyidaw', 'MMK', '+95'),
    ('NAM', 'Namibia', 'Africa', 'Windhoek', 'NAD', '+264'),
    ('NRU', 'Nauru', 'Oceania', 'Yaren', 'AUD', '+674'),
    ('NPL', 'Nepal', 'Asia', 'Kathmandu', 'NPR', '+977'),
    ('NLD', 'Netherlands', 'Europe', 'Amsterdam', 'EUR', '+31'),
    ('NCL', 'New Caledonia', 'Oceania', 'Noumea', 'XPF', '+687'),
    ('NZL', 'New Zealand', 'Oceania', 'Wellington', 'NZD', '+64'),
    ('NIC', 'Nicaragua', 'North America', 'Managua', 'NIO', '+505'),
    ('NER', 'Niger', 'Africa', 'Niamey', 'XOF', '+227'),
    ('NGA', 'Nigeria', 'Africa', 'Abuja', 'NGN', '+234'),
    ('NIU', 'Niue', 'Oceania', 'Alofi', 'NZD', '+683'),
    ('NFK', 'Norfolk Island', 'Oceania', 'Kingston', 'AUD', '+672'),
    ('MKD', 'North Macedonia', 'Europe', 'Skopje', 'MKD', '+389'),
    ('MNP', 'Northern Mariana Islands', 'Oceania', 'Saipan', 'USD', '+1670'),
    ('NOR', 'Norway', 'Europe', 'Oslo', 'NOK', '+47'),
    ('OMN', 'Oman', 'Asia', 'Muscat', 'OMR', '+968'),
    ('PAK', 'Pakistan', 'Asia', 'Islamabad', 'PKR', '+92'),
    ('PLW', 'Palau', 'Oceania', 'Ngerulmud', 'USD', '+680'),
    ('PSE', 'Palestine, State of', 'Asia', 'Ramallah', NULL, '+970'),
    ('PAN', 'Panama', 'North America', 'Panama City', 'PAB', '+507'),
    ('PNG', 'Papua New Guinea', 'Oceania', 'Port Moresby', 'PGK', '+675'),
    ('PRY', 'Paraguay', 'South America', 'Asuncion', 'PYG', '+595'),
    ('PER', 'Peru', 'South America', 'Lima', 'PEN', '+51'),
    ('PHL', 'Philippines', 'Asia', 'Manila', 'PHP', '+63'),
    ('PCN', 'Pitcairn', 'Oceania', 'Adamstown', 'NZD', '+64'),
    ('POL', 'Poland', 'Europe', 'Warsaw', 'PLN', '+48'),
    ('PRT', 'Portugal', 'Europe', 'Lisbon', 'EUR', '+351'),
    ('PRI', 'Puerto Rico', 'North America', 'San Juan', 'USD', '+1787'),
    ('QAT', 'Qatar', 'Asia', 'Doha', 'QAR', '+974'),
    ('REU', 'Reunion', 'Africa', 'Saint-Denis', 'EUR', '+262'),
    ('ROU', 'Romania', 'Europe', 'Bucharest', 'RON', '+40'),
    ('RUS', 'Russian Federation', 'Europe', 'Moscow', 'RUB', '+7'),
    ('RWA', 'Rwanda', 'Africa', 'Kigali', 'RWF', '+250'),
    ('BLM', 'Saint Barthelemy', 'North America', 'Gustavia', 'EUR', '+590'),
    ('SHN', 'Saint Helena, Ascension and Tristan da Cunha', 'Africa', 'Jamestown', 'SHP', '+290'),
    ('KNA', 'Saint Kitts and Nevis', 'North America', 'Basseterre', 'XCD', '+1869'),
    ('LCA', 'Saint Lucia', 'North America', 'Castries', 'XCD', '+1758'),
    ('MAF', 'Saint Martin (French part)', 'North America', 'Marigot', 'EUR', '+590'),
    ('SPM', 'Saint Pierre and Miquelon', 'North America', 'Saint-Pierre', 'EUR', '+508'),
    ('VCT', 'Saint Vincent and the Grenadines', 'North America', 'Kingstown', 'XCD', '+1784'),
    ('WSM', 'Samoa', 'Oceania', 'Apia', 'WST', '+685'),
    ('SMR', 'San Marino', 'Europe', 'San Marino', 'EUR', '+378'),
    ('STP', 'Sao Tome and Principe', 'Africa', 'Sao Tome', 'STN', '+239'),
    ('SAU', 'Saudi Arabia', 'Asia', 'Riyadh', 'SAR', '+966'),
    ('SEN', 'Senegal', 'Africa', 'Dakar', 'XOF', '+221'),
    ('SRB', 'Serbia', 'Europe', 'Belgrade', 'RSD', '+381'),
    ('SYC', 'Seychelles', 'Africa', 'Victoria', 'SCR', '+248'),
    ('SLE', 'Sierra Leone', 'Africa', 'Freetown', 'SLL', '+232'),
    ('SGP', 'Singapore', 'Asia', 'Singapore', 'SGD', '+65'),
    ('SXM', 'Sint Maarten (Dutch part)', 'North America', 'Philipsburg', 'ANG', '+1721'),
    ('SVK', 'Slovakia', 'Europe', 'Bratislava', 'EUR', '+421'),
    ('SVN', 'Slovenia', 'Europe', 'Ljubljana', 'EUR', '+386'),
    ('SLB', 'Solomon Islands', 'Oceania', 'Honiara', 'SBD', '+677'),
    ('SOM', 'Somalia', 'Africa', 'Mogadishu', 'SOS', '+252'),
    ('ZAF', 'South Africa', 'Africa', 'Pretoria', 'ZAR', '+27'),
    ('SGS', 'South Georgia and the South Sandwich Islands', 'South America', 'Grytviken', 'GBP', NULL),
    ('SSD', 'South Sudan', 'Africa', 'Juba', 'SSP', '+211'),
    ('ESP', 'Spain', 'Europe', 'Madrid', 'EUR', '+34'),
    ('LKA', 'Sri Lanka', 'Asia', 'Colombo', 'LKR', '+94'),
    ('SDN', 'Sudan', 'Africa', 'Khartoum', 'SDG', '+249'),
    ('SUR', 'Suriname', 'South America', 'Paramaribo', 'SRD', '+597'),
    ('SJM', 'Svalbard and Jan Mayen', 'Europe', 'Longyearbyen', 'NOK', '+47'),
    ('SWE', 'Sweden', 'Europe', 'Stockholm', 'SEK', '+46'),
    ('CHE', 'Switzerland', 'Europe', 'Bern', 'CHF', '+41'),
    ('SYR', 'Syrian Arab Republic', 'Asia', 'Damascus', 'SYP', '+963'),
    ('TWN', 'Taiwan', 'Asia', 'Taipei', 'TWD', '+886'),
    ('TJK', 'Tajikistan', 'Asia', 'Dushanbe', 'TJS', '+992'),
    ('TZA', 'Tanzania', 'Africa', 'Dodoma', 'TZS', '+255'),
    ('THA', 'Thailand', 'Asia', 'Bangkok', 'THB', '+66'),
    ('TLS', 'Timor-Leste', 'Asia', 'Dili', 'USD', '+670'),
    ('TGO', 'Togo', 'Africa', 'Lome', 'XOF', '+228'),
    ('TKL', 'Tokelau', 'Oceania', 'Fakaofo', 'NZD', '+690'),
    ('TON', 'Tonga', 'Oceania', 'Nuku''alofa', 'TOP', '+676'),
    ('TTO', 'Trinidad and Tobago', 'North America', 'Port of Spain', 'TTD', '+1868'),
    ('TUN', 'Tunisia', 'Africa', 'Tunis', 'TND', '+216'),
    ('TUR', 'Turkey', 'Asia', 'Ankara', 'TRY', '+90'),
    ('TKM', 'Turkmenistan', 'Asia', 'Ashgabat', 'TMT', '+993'),
    ('TCA', 'Turks and Caicos Islands', 'North America', 'Cockburn Town', 'USD', '+1649'),
    ('TUV', 'Tuvalu', 'Oceania', 'Funafuti', 'AUD', '+688'),
    ('UGA', 'Uganda', 'Africa', 'Kampala', 'UGX', '+256'),
    ('UKR', 'Ukraine', 'Europe', 'Kyiv', 'UAH', '+380'),
    ('ARE', 'United Arab Emirates', 'Asia', 'Abu Dhabi', 'AED', '+971'),
    ('GBR', 'United Kingdom', 'Europe', 'London', 'GBP', '+44'),
    ('USA', 'United States', 'North America', 'Washington D.C.', 'USD', '+1'),
    ('UMI', 'United States Minor Outlying Islands', 'Oceania', NULL, 'USD', '+1'),
    ('URY', 'Uruguay', 'South America', 'Montevideo', 'UYU', '+598'),
    ('UZB', 'Uzbekistan', 'Asia', 'Tashkent', 'UZS', '+998'),
    ('VUT', 'Vanuatu', 'Oceania', 'Port Vila', 'VUV', '+678'),
    ('VEN', 'Venezuela', 'South America', 'Caracas', 'VES', '+58'),
    ('VNM', 'Viet Nam', 'Asia', 'Hanoi', 'VND', '+84'),
    ('VGB', 'Virgin Islands (British)', 'North America', 'Road Town', 'USD', '+1284'),
    ('VIR', 'Virgin Islands (U.S.)', 'North America', 'Charlotte Amalie', 'USD', '+1340'),
    ('WLF', 'Wallis and Futuna', 'Oceania', 'Mata-Utu', 'XPF', '+681'),
    ('ESH', 'Western Sahara', 'Africa', 'El Aaiun', NULL, '+212'),
    ('YEM', 'Yemen', 'Asia', 'Sana''a', 'YER', '+967'),
    ('ZMB', 'Zambia', 'Africa', 'Lusaka', 'ZMW', '+260'),
    ('ZWE', 'Zimbabwe', 'Africa', 'Harare', 'ZWL', '+263');