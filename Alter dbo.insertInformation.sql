USE [Inter_Test]
GO

/****** Object: SqlProcedure [dbo].[insertInformation] Script Date: 10-Oct-24 10:43:05 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[insertInformation]
	@p_Name varchar(255),
	@p_Email varchar(255),
	@p_Password varchar(10),
	@ChildrenInfo NVARCHAR(MAX)  -- Pass JSON data for multiple children
AS
	BEGIN
		BEGIN TRANSACTION;
			BEGIN TRY
				INSERT INTO Information(Name, Email, Password) VALUES (@p_Name, @p_Email, @p_Password);

				DECLARE @p_InfoId INT = SCOPE_IDENTITY();

				-- Parse the JSON to insert into Information_Extended table
				DECLARE	@Facebook NVARCHAR(255), @Instagram NVARCHAR(255), @Twitter NVARCHAR(255);

				INSERT INTO Information_Extended (Facebook, Instagram, Twitter, Info_Id) 
				SELECT 
					JSON_VALUE(value, '$.Facebook'),
					JSON_VALUE(value, '$.Instagram'),
					JSON_VALUE(value, '$.Twitter'),
					@p_InfoId  -- Use the parent's Id for the foreign key
				FROM OPENJSON(@ChildrenInfo);
				COMMIT TRANSACTION;
			END TRY
			BEGIN CATCH
				ROLLBACK TRANSACTION;
				THROW;
			END CATCH
	END
RETURN 0
