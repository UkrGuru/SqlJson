BEGIN /***** Init Tables *****/

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Products]') AND type in (N'U'))
BEGIN
CREATE TABLE [Products](
	[ProductId] [int] IDENTITY(1,1) NOT NULL,
	[ProductName] [varchar](50) NULL,
	[CategoryName] [varchar](20) NULL,
	[QuantityPerUnit] [varchar](20) NULL,
	[UnitPrice] [smallmoney] NULL,
	[UnitsInStock] [int] NULL,
	[UnitsOnOrder] [int] NULL,
	[ReorderLevel] [int] NULL,
	[Discontinued] [bit] NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END

END

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [Products_Del]
    @Data int
AS
DELETE Products
WHERE (ProductId = @Data)
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [Products_Get]
    @Data int
AS
SELECT ProductId, ProductName, CategoryName, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued
FROM Products
WHERE (ProductId = @Data)
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [Products_Grd]
AS
SELECT ProductId, ProductName, CategoryName, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued
FROM Products
FOR JSON PATH
'; 

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [Products_Ins]
	@Data nvarchar(500)  
AS
INSERT INTO Products (ProductName, CategoryName, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued)
SELECT ProductName, CategoryName, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued
FROM OPENJSON(@Data) 
WITH (ProductName varchar(50), CategoryName varchar(20), QuantityPerUnit varchar(20), 
	UnitPrice smallmoney, UnitsInStock int, UnitsOnOrder int, ReorderLevel int, Discontinued bit
)
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [Products_Upd]
	@Data nvarchar(500)  
AS
UPDATE P
SET P.ProductName = D.ProductName, P.CategoryName = D.CategoryName, P.QuantityPerUnit = D.QuantityPerUnit,
	P.UnitPrice = D.UnitPrice, P.UnitsInStock = D.UnitsInStock, P.UnitsOnOrder = D.UnitsOnOrder,
	P.ReorderLevel = D.ReorderLevel, P.Discontinued = D.Discontinued
FROM Products P
CROSS JOIN (SELECT * FROM OPENJSON(@Data) 
    WITH (ProductName varchar(50), CategoryName varchar(20), QuantityPerUnit varchar(20), 
	UnitPrice smallmoney, UnitsInStock int, UnitsOnOrder int, ReorderLevel int, Discontinued bit)) D
WHERE P.ProductId = JSON_VALUE(@Data,''$.ProductId'')
';

TRUNCATE TABLE [Products]

SET IDENTITY_INSERT [dbo].[Products] ON 
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (1, N'Chai', N'Beverages', N'10 boxes x 20 bags', 18.0000, 39, 0, 10, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (2, N'Chang', N'Beverages', N'24 - 12 oz bottles', 19.0000, 17, 40, 25, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (3, N'Aniseed Syrup', N'Condiments', N'12 - 550 ml bottles', 10.0000, 13, 70, 25, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (4, N'Chef Anton''s Cajun Seasoning', N'Condiments', N'48 - 6 oz jars', 22.0000, 53, 0, 0, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (5, N'Chef Anton''s Gumbo Mix', N'Condiments', N'36 boxes', 21.3500, 0, 0, 0, 1)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (6, N'Grandma''s Boysenberry Spread', N'Condiments', N'12 - 8 oz jars', 25.0000, 120, 0, 25, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (7, N'Uncle Bob''s Organic Dried Pears', N'Produce', N'12 - 1 lb pkgs.', 30.0000, 15, 0, 10, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (8, N'Northwoods Cranberry Sauce', N'Condiments', N'12 - 12 oz jars', 40.0000, 6, 0, 0, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (9, N'Mishi Kobe Niku', N'Meat/Poultry', N'18 - 500 g pkgs.', 97.0000, 29, 0, 0, 1)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (10, N'Ikura', N'Seafood', N'12 - 200 ml jars', 31.0000, 31, 0, 0, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (11, N'Queso Cabrales', N'Dairy Products', N'1 kg pkg.', 21.0000, 22, 30, 30, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (12, N'Queso Manchego La Pastora', N'Dairy Products', N'10 - 500 g pkgs.', 38.0000, 86, 0, 0, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (13, N'Konbu', N'Seafood', N'2 kg box', 6.0000, 24, 0, 5, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (14, N'Tofu', N'Produce', N'40 - 100 g pkgs.', 23.2500, 35, 0, 0, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (15, N'Genen Shouyu', N'Condiments', N'24 - 250 ml bottles', 15.5000, 39, 0, 5, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (16, N'Pavlova', N'Confections', N'32 - 500 g boxes', 17.4500, 29, 0, 10, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (17, N'Alice Mutton', N'Meat/Poultry', N'20 - 1 kg tins', 39.0000, 0, 0, 0, 1)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (18, N'Carnarvon Tigers', N'Seafood', N'16 kg pkg.', 62.5000, 42, 0, 0, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (19, N'Teatime Chocolate Biscuits', N'Confections', N'10 boxes x 12 pieces', 9.2000, 25, 0, 5, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (20, N'Sir Rodney''s Marmalade', N'Confections', N'30 gift boxes', 81.0000, 40, 0, 0, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (21, N'Sir Rodney''s Scones', N'Confections', N'24 pkgs. x 4 pieces', 10.0000, 3, 40, 5, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (22, N'Gustaf''s Knäckebröd', N'Grains/Cereals', N'24 - 500 g pkgs.', 21.0000, 104, 0, 25, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (23, N'Tunnbröd', N'Grains/Cereals', N'12 - 250 g pkgs.', 9.0000, 61, 0, 25, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (24, N'Guaraná Fantástica', N'Beverages', N'12 - 355 ml cans', 4.5000, 20, 0, 0, 1)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (25, N'NuNuCa Nuß-Nougat-Creme', N'Confections', N'20 - 450 g glasses', 14.0000, 76, 0, 30, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (26, N'Gumbär Gummibärchen', N'Confections', N'100 - 250 g bags', 31.2300, 15, 0, 0, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (27, N'Schoggi Schokolade', N'Confections', N'100 - 100 g pieces', 43.9000, 49, 0, 30, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (28, N'Rössle Sauerkraut', N'Produce', N'25 - 825 g cans', 45.6000, 26, 0, 0, 1)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (29, N'Thüringer Rostbratwurst', N'Meat/Poultry', N'50 bags x 30 sausgs.', 123.7900, 0, 0, 0, 1)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (30, N'Nord-Ost Matjeshering', N'Seafood', N'10 - 200 g glasses', 25.8900, 10, 0, 15, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (31, N'Gorgonzola Telino', N'Dairy Products', N'12 - 100 g pkgs', 12.5000, 0, 70, 20, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (32, N'Mascarpone Fabioli', N'Dairy Products', N'24 - 200 g pkgs.', 32.0000, 9, 40, 25, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (33, N'Geitost', N'Dairy Products', N'500 g', 2.5000, 112, 0, 20, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (34, N'Sasquatch Ale', N'Beverages', N'24 - 12 oz bottles', 14.0000, 111, 0, 15, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (35, N'Steeleye Stout', N'Beverages', N'24 - 12 oz bottles', 18.0000, 20, 0, 15, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (36, N'Inlagd Sill', N'Seafood', N'24 - 250 g  jars', 19.0000, 112, 0, 20, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (37, N'Gravad lax', N'Seafood', N'12 - 500 g pkgs.', 26.0000, 11, 50, 25, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (38, N'Côte de Blaye', N'Beverages', N'12 - 75 cl bottles', 263.5000, 17, 0, 15, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (39, N'Chartreuse verte', N'Beverages', N'750 cc per bottle', 18.0000, 69, 0, 5, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (40, N'Boston Crab Meat', N'Seafood', N'24 - 4 oz tins', 18.4000, 123, 0, 30, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (41, N'Jack''s New England Clam Chowder', N'Seafood', N'12 - 12 oz cans', 9.6500, 85, 0, 10, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (42, N'Singaporean Hokkien Fried Mee', N'Grains/Cereals', N'32 - 1 kg pkgs.', 14.0000, 26, 0, 0, 1)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (43, N'Ipoh Coffee', N'Beverages', N'16 - 500 g tins', 46.0000, 17, 10, 25, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (44, N'Gula Malacca', N'Condiments', N'20 - 2 kg bags', 19.4500, 27, 0, 15, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (45, N'Rogede sild', N'Seafood', N'1k pkg.', 9.5000, 5, 70, 15, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (46, N'Spegesild', N'Seafood', N'4 - 450 g glasses', 12.0000, 95, 0, 0, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (47, N'Zaanse koeken', N'Confections', N'10 - 4 oz boxes', 9.5000, 36, 0, 0, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (48, N'Chocolade', N'Confections', N'10 pkgs.', 12.7500, 15, 70, 25, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (49, N'Maxilaku', N'Confections', N'24 - 50 g pkgs.', 20.0000, 10, 60, 15, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (50, N'Valkoinen suklaa', N'Confections', N'12 - 100 g bars', 16.2500, 65, 0, 30, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (51, N'Manjimup Dried Apples', N'Produce', N'50 - 300 g pkgs.', 53.0000, 20, 0, 10, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (52, N'Filo Mix', N'Grains/Cereals', N'16 - 2 kg boxes', 7.0000, 38, 0, 25, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (53, N'Perth Pasties', N'Meat/Poultry', N'48 pieces', 32.8000, 0, 0, 0, 1)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (54, N'Tourtière', N'Meat/Poultry', N'16 pies', 7.4500, 21, 0, 10, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (55, N'Pâté chinois', N'Meat/Poultry', N'24 boxes x 2 pies', 24.0000, 115, 0, 20, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (56, N'Gnocchi di nonna Alice', N'Grains/Cereals', N'24 - 250 g pkgs.', 38.0000, 21, 10, 30, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (57, N'Ravioli Angelo', N'Grains/Cereals', N'24 - 250 g pkgs.', 19.5000, 36, 0, 20, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (58, N'Escargots de Bourgogne', N'Seafood', N'24 pieces', 13.2500, 62, 0, 20, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (59, N'Raclette Courdavault', N'Dairy Products', N'5 kg pkg.', 55.0000, 79, 0, 0, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (60, N'Camembert Pierrot', N'Dairy Products', N'15 - 300 g rounds', 34.0000, 19, 0, 0, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (61, N'Sirop d''érable', N'Condiments', N'24 - 500 ml bottles', 28.5000, 113, 0, 25, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (62, N'Tarte au sucre', N'Confections', N'48 pies', 49.3000, 17, 0, 0, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (63, N'Vegie-spread', N'Condiments', N'15 - 625 g jars', 43.9000, 24, 0, 5, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (64, N'Wimmers gute Semmelknödel', N'Grains/Cereals', N'20 bags x 4 pieces', 33.2500, 22, 80, 30, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (65, N'Louisiana Fiery Hot Pepper Sauce', N'Condiments', N'32 - 8 oz bottles', 21.0500, 76, 0, 0, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (66, N'Louisiana Hot Spiced Okra', N'Condiments', N'24 - 8 oz jars', 17.0000, 4, 100, 20, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (67, N'Laughing Lumberjack Lager', N'Beverages', N'24 - 12 oz bottles', 14.0000, 52, 0, 10, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (68, N'Scottish Longbreads', N'Confections', N'10 boxes x 8 pieces', 12.5000, 6, 10, 15, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (69, N'Gudbrandsdalsost', N'Dairy Products', N'10 kg pkg.', 36.0000, 26, 0, 15, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (70, N'Outback Lager', N'Beverages', N'24 - 355 ml bottles', 15.0000, 15, 10, 30, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (71, N'Flotemysost', N'Dairy Products', N'10 - 500 g pkgs.', 21.5000, 26, 0, 0, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (72, N'Mozzarella di Giovanni', N'Dairy Products', N'24 - 200 g pkgs.', 34.8000, 14, 0, 0, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (73, N'Röd Kaviar', N'Seafood', N'24 - 150 g jars', 15.0000, 101, 0, 5, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (74, N'Longlife Tofu', N'Produce', N'5 kg pkg.', 10.0000, 4, 20, 5, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (75, N'Rhönbräu Klosterbier', N'Beverages', N'24 - 0.5 l bottles', 7.7500, 125, 0, 25, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (76, N'Lakkalikööri', N'Beverages', N'500 ml', 18.0000, 57, 0, 20, 0)
INSERT [dbo].[Products] ([ProductId], [ProductName], [CategoryName], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) VALUES (77, N'Original Frankfurter grüne Soße', N'Condiments', N'12 boxes', 13.0000, 32, 0, 15, 0)
SET IDENTITY_INSERT [dbo].[Products] OFF
