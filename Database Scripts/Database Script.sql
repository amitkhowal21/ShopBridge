/*

Author:			AMIT SHARMA
Create date:	13-OCT-2021
Description:	THIS WILL CREATE DB & TABLES FOR SHOPBRIDGE POC

*/

--CHECK AND CREATE DB
IF NOT EXISTS (SELECT 1 FROM master.sys.databases WHERE name = N'ShopBridge')
BEGIN
	CREATE DATABASE ShopBridge
END
GO

USE ShopBridge

GO
--==========================
--CREATE BASE TABLES
--============================
IF NOT EXISTS (SELECT 1 FROM   sys.tables WHERE  NAME = 'ProductBrand' )
BEGIN
	CREATE TABLE [dbo].[ProductBrand](
	[Id] [INT]  PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Code] [VARCHAR](20) NOT NULL,
	[Name] [NVARCHAR](250) NOT NULL,
	[Description] [NVARCHAR](1000) NULL,
	[IsDeleted] [BIT] NOT NULL DEFAULT(0),
	[IsActive] [BIT] NOT NULL DEFAULT(1),
	[CreatedBy] [INT] NOT NULL,
	[CreatedDate] [DATETIME] NOT NULL DEFAULT(1),
	[ModifiedBy] [INT] NULL,
	[ModifiedDate] [DATETIME] NULL,
	)
END

GO

IF NOT EXISTS (SELECT 1 FROM   sys.tables WHERE  NAME = 'Product' )
BEGIN
	CREATE TABLE [dbo].[Product](
	[Id] [INT]  PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[SKU] [NVARCHAR](100) NULL,
	[Name] [NVARCHAR](250) NOT NULL,
	[ShortDescription] [NVARCHAR](500) NULL,
	[FullDescription] [NVARCHAR](MAX) NULL,	
	[LowPrice] [DECIMAL](18,4) NOT NULL,
	[HighPrice] [DECIMAL](18,4) NOT NULL,
	[Quantity] INT NOT NULL,
	[IsFinished] [bit] NULL DEFAULT(0),
	[IsDeleted] [BIT] NOT NULL DEFAULT(0),
	[IsActive] [BIT] NOT NULL DEFAULT(1),
	[CreatedBy] [INT] NOT NULL,
	[CreatedDate] [DATETIME] NOT NULL DEFAULT(1),
	[ModifiedBy] [INT] NULL,
	[ModifiedDate] [DATETIME] NULL,
	)
END

GO

IF NOT EXISTS (SELECT 1 FROM   sys.tables WHERE  NAME = 'ProductBrandMapping' )
BEGIN
	CREATE TABLE [dbo].[ProductBrandMapping](
	[Id] [INT] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[ProductId] [INT] NOT NULL,
	[BrandId] [INT] NOT NULL,
	[IsDeleted] [BIT] NOT NULL DEFAULT(0),
	[IsActive] [BIT] NOT NULL DEFAULT(1),
	[CreatedBy] [INT] NOT NULL,
	[CreatedDate] [DATETIME] NOT NULL DEFAULT(1),
	[ModifiedBy] [INT] NULL,
	[ModifiedDate] [DATETIME] NULL,
	CONSTRAINT FK_ProductBrandMapping_Product FOREIGN KEY (ProductId)
			REFERENCES Product(Id),
	CONSTRAINT FK_ProductBrandMapping_ProductBrand FOREIGN KEY (BrandId)
			REFERENCES ProductBrand(Id)
) 
END

--GO


IF NOT EXISTS (SELECT 1 FROM   sys.tables WHERE  NAME = 'ProductTags' )
BEGIN
	CREATE TABLE [dbo].[ProductTags](
	[Id] [INT] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[ProductId] [INT] NOT NULL,
	[TagName] [NVARCHAR](500) NOT NULL,
	[IsDeleted] [BIT] NOT NULL DEFAULT(0),
	[IsActive] [BIT] NOT NULL DEFAULT(1),
	[CreatedBy] [INT] NOT NULL,
	[CreatedDate] [DATETIME] NOT NULL DEFAULT(1),
	[ModifiedBy] [INT] NULL,
	[ModifiedDate] [DATETIME] NULL,
	CONSTRAINT FK_ProductTags_Product FOREIGN KEY (ProductId)
			REFERENCES Product(Id)
) 
END

GO

IF NOT EXISTS (SELECT 1 FROM   sys.tables WHERE  NAME = 'ProductStockMovement' )
BEGIN
	CREATE TABLE [dbo].[ProductStockMovement](
	[Id] [INT] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[ProductId] [INT] NOT NULL,
	[LowPrice] [DECIMAL](18,4) NOT NULL,
	[HighPrice] [DECIMAL](18,4) NOT NULL,
	[IsDeleted] [BIT] NOT NULL DEFAULT(0),
	[IsActive] [BIT] NOT NULL DEFAULT(1),
	[CreatedBy] [INT] NOT NULL,
	[CreatedDate] [DATETIME] NOT NULL DEFAULT(1)
	CONSTRAINT FK_ProductStockMovement_Product FOREIGN KEY (ProductId)
			REFERENCES Product(Id)
) 
END

GO
--================================
--CREATE OTHER DDL ITEMS
--================================
/*

EXEC DeleteProduct 1,1
*/

CREATE OR ALTER PROCEDURE DeleteProduct 
@ProductId INT,
@DeletedBy INT
AS
BEGIN
BEGIN TRY
	SET NOCOUNT ON;
--	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	IF NOT EXISTS(SELECT 1 FROM Product WITH (NOLOCK) WHERE Id = @ProductId AND IsActive=1 AND IsDeleted=0)
	BEGIN
		SELECT 0 [Status], 'Product not found' [Message]
		RETURN;
	END

	BEGIN TRAN   
	-- DELETE PRODUCT TAGS
	UPDATE ProductTags SET IsActive=0,IsDeleted=1,ModifiedBy=@DeletedBy,ModifiedDate=GETDATE() WHERE ProductId=@ProductId
	--DELETE PRODUCT BRAND
	UPDATE ProductBrandMapping SET IsActive=0,IsDeleted=1,ModifiedBy=@DeletedBy,ModifiedDate=GETDATE() WHERE ProductId=@ProductId
	-- DELETE PRODUCT
	UPDATE Product SET IsActive=0,IsDeleted=1,ModifiedBy=@DeletedBy,ModifiedDate=GETDATE() WHERE Id=@ProductId
	COMMIT TRAN  
	SELECT 200 [Status], 'Product deleted successfully' [Message]
END TRY                                                                                                
BEGIN CATCH                                                                                                                  
	IF @@TRANCOUNT > 0        
	ROLLBACK TRAN   
		SELECT 0 [Status], ERROR_MESSAGE() [Message] 
	RETURN -1                                                                                                
END CATCH                                                                   
SET NOCOUNT OFF 
END 