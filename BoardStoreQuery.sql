USE [BoardStoreApp];
/*DROP TABLE IF EXISTS ShoppingCart ;
DROP TABLE IF EXISTS Purchases ;
DROP TABLE IF EXISTS Items ;
DROP TABLE IF EXISTS Users ;
DROP TRIGGER dbo.trgCalculateAmountForSpecificItem;
DROP TRIGGER dbo.trgEnforceStockAndReduceQuantity;
DROP TRIGGER dbo.trgPreventSellerPurchase;*/


CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY,
    Username VARCHAR(50) NOT NULL UNIQUE,
	Email VARCHAR(50)  NOT NULL UNIQUE,
    Pass VARCHAR(100) NOT NULL,
    Name VARCHAR(100),
    Age INT,
	Visited BIT DEFAULT 0,
    Photo VARCHAR(200),
    Contacts VARCHAR(200),
    Address VARCHAR(200)
);


CREATE TABLE Items (
    ItemId INT PRIMARY KEY IDENTITY,
    Name VARCHAR(100) NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    AvailableStock INT NOT NULL,
    SellerId INT,
    FOREIGN KEY (SellerId) REFERENCES Users(UserId)
);

CREATE TABLE Purchases (
    PurchaseId INT PRIMARY KEY IDENTITY,
    UserId INT,
    ItemId INT,
    Quantity INT,
	Amount INT ,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (ItemId) REFERENCES Items(ItemId)
);

CREATE TABLE ShoppingCart (
    ShoppingCartItemId INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL,
    ItemId INT NOT NULL,
    Quantity INT NOT NULL,
      FOREIGN KEY (UserId) REFERENCES Users (UserId),
      FOREIGN KEY (ItemId) REFERENCES Items (ItemId)
);

GO
CREATE TRIGGER trgCalculateAmountForSpecificItem
ON Purchases
AFTER INSERT
AS
BEGIN
    UPDATE p
    SET p.Amount = p.Quantity * itm.Price
    FROM Purchases p
    INNER JOIN Items itm ON p.ItemId = itm.ItemId
END;

GO
CREATE TRIGGER trgPreventSellerPurchase
ON Purchases
AFTER INSERT
AS
BEGIN
    -- Check if the inserted row's UserId is the same as SellerId
    IF EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN Items itm ON i.ItemId = itm.ItemId
        WHERE i.UserId = itm.SellerId
    )
    
        -- Seller cannot buy their own items, raise an error
        THROW 50001, 'A seller cannot buy their own items.', 1;
    ELSE
    
    
        -- Insert the rows that don't violate the restriction
        INSERT INTO Purchases (UserId, ItemId, Quantity, Amount)
     
	 
	 SELECT UserId, i.ItemId, Quantity, Quantity * itm.Price
        FROM inserted i
        INNER JOIN Items itm ON i.ItemId = itm.ItemId
    
END;


GO
CREATE TRIGGER trgEnforceStockAndReduceQuantity
ON Purchases
AFTER INSERT
AS
BEGIN
    DECLARE @ItemId INT;
    DECLARE @Quantity INT;

    SELECT @ItemId = i.ItemId, @Quantity = i.Quantity
    FROM inserted i;

    -- Check available stock and prevent purchase if quantity exceeds
    IF EXISTS (
        /*SELECT 1
        FROM Items itm
        WHERE itm.ItemId = @ItemId AND @Quantity > itm.AvailableStock*/
		SELECT 1
        FROM inserted i
        INNER JOIN Items itm ON i.ItemId = itm.ItemId
        WHERE i.Quantity > itm.AvailableStock
    )
	
    -- Quantity exceeds available stock, raise an error
        THROW 50001, 'Quantity exceeds available stock.', 1 ;
      
    
    ELSE
    
        -- Update available stock and insert the purchase
        UPDATE Items 
        SET AvailableStock = AvailableStock - i.Quantity
		FROM inserted i
        WHERE Items.ItemId = i.ItemId;
    
END;


