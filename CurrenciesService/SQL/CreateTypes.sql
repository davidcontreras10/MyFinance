BEGIN
IF OBJECT_ID('BccrVentanillaObject', 'U') IS NULL
	BEGIN
		CREATE TYPE [dbo].[BccrVentanillaObject] AS TABLE
		(
			EntityName VARCHAR(500) NOT NULL,
			Purchase FLOAT NOT NULL,
			Sell FLOAT NOT NULL,
			LastUpdate DATETIME NOT NULL
		);
	END
END


