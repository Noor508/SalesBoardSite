USE[BoardStoreApp];


-- Insert Users
INSERT INTO Users (Username, Email, Pass, Name, Age, Visited, Photo, Contacts, Address)
VALUES
    ('user1', 'user1@example.com', 'password1', 'User One', 25, 0, 'photo1.jpg', '1234567890', '123 Main St'),
    ('user2', 'user2@example.com', 'password2', 'User Two', 30, 0, 'photo2.jpg', '9876543210', '456 Elm St'),
    ('user3', 'user3@example.com', 'password3', 'User Three', 28, 0, 'photo3.jpg', '5555555555', '789 Oak St'),
    ('user4', 'user4@example.com', 'password4', 'User Four', 22, 0, 'photo4.jpg', '9999999999', '456 Pine St'),
    ('user5', 'user5@example.com', 'password5', 'User Five', 27, 0, 'photo5.jpg', '8888888888', '789 Maple St'),
    ('user6', 'user6@example.com', 'password6', 'User Six', 35, 0, 'photo6.jpg', '7777777777', '234 Cedar St'),
	('user7', 'user7@example.com', 'password7', 'User Seven', 31, 0, 'photo7.jpg', '6666666666', '567 Oak St'),
    -- Add more users...
    ('user8', 'user8@example.com', 'password8', 'User Eight', 29, 0, 'photo8.jpg', '5555555555', '890 Maple St'),
    ('user9', 'user9@example.com', 'password9', 'User Nine', 26, 0, 'photo9.jpg', '4444444444', '123 Elm St'),
    ('user10', 'user10@example.com', 'password10', 'User Ten', 32, 0, 'photo10.jpg', '3333333333', '456 Oak St');


-- Insert Items
-- Insert 20 items
INSERT INTO Items (Name, Price, AvailableStock, SellerId)
VALUES
    ('Item 1', 10.00, 100, 1),
    ('Item 2', 20.00, 50, 2),
    ('Item 3', 15.00, 80, 3),
    ('Item 4', 30.00, 20, 4),
    -- Add more items...
    ('Item 5', 25.00, 60, 5),
    -- Add more items...
    ('Item 6', 12.00, 40, 6),
    -- Add more items...
    ('Item 7', 18.00, 70, 7),
    -- Add more items...
    ('Item 8', 22.00, 90, 8),
    -- Add more items...
    ('Item 9', 28.00, 30, 9),
    ('Item 10', 23.00, 85, 10);


-- Insert Purchases
-- Insert 20 purchases (Make sure to modify UserId and ItemId values according to your data)
INSERT INTO Purchases (UserId, ItemId, Quantity)
VALUES
    (1, 2, 5),
    (2, 3, 3),
    (3, 4, 2),
    (4, 5, 4),
    -- Add more purchases...
    (5, 6, 6),
    -- Add more purchases...
    (6, 7, 1),
    -- Add more purchases...
    (7, 8, 4),
    -- Add more purchases...
    (8, 9, 7),
    -- Add more purchases...
    (9, 10, 2),
    (10, 2, 3);
USE[BoardStoreApp];
SELECT * FROM Users;
SELECT * FROM Purchases;
SELECT * FROM Items;
SELECT * FROM ShoppingCart;

SELECT * FROM Purchases P
LEFT JOIN Items i ON p.ItemId = i.ItemId
LEFT JOIN Users u ON p.UserId = u.UserId
WHERE p.UserId != i.SellerId and 
i.SellerId = 2;

SELECT * FROM Purchases p
LEFT JOIN Items i ON p.ItemId = i.ItemId
LEFT JOIN Users U ON i.SellerId = U.UserId
WHERE p.UserId != i.SellerId AND i.SellerId = 2;


SELECT U.Username, SUM(P.Amount) AS TotalSpending  
                                 FROM Purchases P  
                                 LEFT JOIN Items I ON P.ItemId = I.ItemId  
                                 LEFT JOIN Users U ON P.UserId = U.UserId  
                                 WHERE P.UserId != 5 AND  
                                 I.SellerID = 5  
                                 GROUP BY U.Username ;

SELECT * FROM Items WHERE
Name LIKE ('%e%');

SELECT * FROM ShoppingCart C 
LEFT JOIN Items I ON C.ItemId = I.ItemId
WHERE C.UserId = 2;