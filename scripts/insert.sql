        PRAGMA foreign_keys = OFF;

        BEGIN TRANSACTION;

        -- ==========================================================
-- 1. DICTIONARIES (Lookups)
-- ==========================================================

-- === SIZES ===
        INSERT INTO Sizes (Id, Name) VALUES
                                         (1, 'S'),
                                         (2, 'M'),
                                         (3, 'L'),
                                         (4, 'XL');

        -- === STATUSES ===
-- NEW SCHEME: 1=Registered, 2=Submitted, 3=In Transit, 4=Delivered, 5=Canceled
        INSERT INTO Statuses (Id, Name) VALUES
                                            (1, 'Registered'),
                                            (2, 'Submitted'),
                                            (3, 'In Transit'),
                                            (4, 'Delivered'),
                                            (5, 'Canceled');

-- === ROLES ===
        INSERT INTO Roles (Id, Name, Description) VALUES
                                                      (1, 'Admin', 'Full access to the system'),
                                                      (2, 'Operator', 'Can manage packages and trackings'),
                                                      (3, 'User', 'Can create packages and view history');

-- === OCCUPATIONS ===
        INSERT INTO Occupations (Id, Name, BaseSalary) VALUES
                                                           (1, 'Administrator', 40000),
                                                           (2, 'Manager', 25000),
                                                           (3, 'Courier', 18000),
                                                           (4, 'Warehouse Worker', 16000),
                                                           (5, 'Logistics Specialist', 22000);

-- === LOCATIONS ===
        INSERT INTO Locations (Id, Country, City, Address, Postcode) VALUES
                                                                         (1, 'Ukraine', 'Kyiv', 'Khreshchatyk 1', 01001),
                                                                         (2, 'United Kingdom', 'London', 'Baker Street 221B', 10000),
                                                                         (3, 'France', 'Paris', 'Rue de Rivoli 14', 75001),
                                                                         (4, 'Germany', 'Berlin', 'Friedrichstrasse 101', 10117),
                                                                         (5, 'Italy', 'Rome', 'Via del Corso 50', 00186),
                                                                         (6, 'Spain', 'Madrid', 'Calle Mayor 12', 28013),
                                                                         (7, 'Poland', 'Warsaw', 'Nowy Swiat 4', 00042),
                                                                         (8, 'Czech Republic', 'Prague', 'Charles Bridge 11', 11000),
                                                                         (9, 'Austria', 'Vienna', 'Stephansplatz 2', 1010),
                                                                         (10, 'Switzerland', 'Zurich', 'Bahnhofstrasse 7', 8001),
                                                                         (11, 'Ukraine', 'Lviv', 'Rynok Square 10', 79000),
                                                                         (12, 'Poland', 'Krakow', 'Florianska 15', 31021),
                                                                         (13, 'Germany', 'Munich', 'Marienplatz 1', 80331),
                                                                         (14, 'Netherlands', 'Amsterdam', 'Dam Square 5', 1012),
                                                                         (15, 'Belgium', 'Brussels', 'Grand Place 3', 1000);

        -- ==========================================================
-- 2. PEOPLE & ACCOUNTS
-- ==========================================================

-- === PEOPLE ===
        INSERT INTO People (Id, FirstName, LastName, Email, Phone) VALUES
                                                                       (1, 'Admin', 'Super', 'admin@mininova.com', '+380000000001'),
                                                                       (2, 'Emma', 'Watson', 'emma.watson@uk.co', '+442071234567'),
                                                                       (3, 'Benedict', 'Cumberbatch', 'ben.c@uk.co', '+442089876543'),
                                                                       (4, 'Jean', 'Reno', 'jean.reno@fr.com', '+33140123456'),
                                                                       (5, 'Elon', 'Musk', 'elon@tesla.com', '+493011122233'),
                                                                       (6, 'Cillian', 'Murphy', 'thomas.shelby@ie.com', '+35316123456'),
                                                                       (7, 'Monica', 'Bellucci', 'monica@it.it', '+390612345678'),
                                                                       (8, 'Robert', 'Lewandowski', 'robert@pl.com', '+48221234567'),
                                                                       (9, 'Mads', 'Mikkelsen', 'mads@dk.dk', '+4533123456'),
                                                                       (10, 'Penelope', 'Cruz', 'penelope@es.es', '+34911234567'),
                                                                       (11, 'Keanu', 'Reeves', 'keanu@matrix.com', '+13105550100'),
                                                                       (12, 'Margot', 'Robbie', 'barbie@hollywood.com', '+13105550199'),
                                                                       (13, 'Pedro', 'Pascal', 'mando@starwars.com', '+56912345678'),
                                                                       (14, 'Zendaya', 'Coleman', 'mj@spiderman.com', '+15550102030'),
                                                                       (15, 'Tom', 'Holland', 'peter.parker@avengers.com', '+442012345678');

-- === ACCOUNTS ===
        INSERT INTO Accounts (Id, Login, PasswordHash, RoleId, PersonId) VALUES
                                                                             (1, 'admin', 'AQAAAAIAAYagAAAAEDO5+eS422XAfnhc22j+UZpFmdJ1O73L23UM9XSvGdjFH02ATLrdkC5AkWJhFLNWtg==', 1, 1),
                                                                             (2, 'manager_olena', 'AQAAAAIAAYagAAAAEFneVWuAdfXyFmNSgRuZ9nGkkgRSwmKrAcbiqyGvL0sHbXdSHHGGxjFudDhgaREJew==', 2, 2),
                                                                             (3, 'courier_petro', 'AQAAAAIAAYagAAAAEFneVWuAdfXyFmNSgRuZ9nGkkgRSwmKrAcbiqyGvL0sHbXdSHHGGxjFudDhgaREJew==', 2, 3),
                                                                             (4, 'jean_r', 'AQAAAAIAAYagAAAAEFneVWuAdfXyFmNSgRuZ9nGkkgRSwmKrAcbiqyGvL0sHbXdSHHGGxjFudDhgaREJew==', 3, 4),
                                                                             (5, 'elon_m', 'AQAAAAIAAYagAAAAEFneVWuAdfXyFmNSgRuZ9nGkkgRSwmKrAcbiqyGvL0sHbXdSHHGGxjFudDhgaREJew==', 3, 5),
                                                                             (6, 'peaky_b', 'AQAAAAIAAYagAAAAEFneVWuAdfXyFmNSgRuZ9nGkkgRSwmKrAcbiqyGvL0sHbXdSHHGGxjFudDhgaREJew==', 3, 6),
                                                                             (7, 'monica_b', 'AQAAAAIAAYagAAAAEFneVWuAdfXyFmNSgRuZ9nGkkgRSwmKrAcbiqyGvL0sHbXdSHHGGxjFudDhgaREJew==', 3, 7),
                                                                             (8, 'lewy_9', 'AQAAAAIAAYagAAAAEFneVWuAdfXyFmNSgRuZ9nGkkgRSwmKrAcbiqyGvL0sHbXdSHHGGxjFudDhgaREJew==', 3, 8),
                                                                             (9, 'user_keanu', 'AQAAAAIAAYagAAAAEFneVWuAdfXyFmNSgRuZ9nGkkgRSwmKrAcbiqyGvL0sHbXdSHHGGxjFudDhgaREJew==', 3, 11),
                                                                             (10, 'user_tom', 'AQAAAAIAAYagAAAAEFneVWuAdfXyFmNSgRuZ9nGkkgRSwmKrAcbiqyGvL0sHbXdSHHGGxjFudDhgaREJew==', 3, 15);

-- === OPERATORS ===
        INSERT INTO Operators (Id, Salary, HireDate, OccupationId, PersonId) VALUES
                                                                                 (1, 45000, '2023-01-10 09:00:00', 1, 1),
                                                                                 (2, 27000, '2023-03-20 10:00:00', 2, 2),
                                                                                 (3, 20000, '2023-02-15 11:00:00', 3, 3),
                                                                                 (4, 18500, '2023-06-01 09:00:00', 4, 12),
                                                                                 (5, 21000, '2023-07-15 14:00:00', 5, 13);

-- ==========================================================
-- 3. SHIPMENTS & INVOICES 
-- ==========================================================

-- 1. Completed shipment (UA -> PL)
        INSERT INTO Shipments (Id, TrackId, ShipperId, ConsigneeId, Description, SizeId, Weight, OriginId, DestinationId, CreatedAt) VALUES
                                                                                                                                         (1, 'UA 3150 4921 555 3', 1, 8, 'Starlink Terminal', 3, 15.0, 1, 7, '2023-11-01 08:30:00'),
                                                                                                                                         (3, 'UA 1002 7741 229 1', 11, 12, 'Tactical Medical Kit', 2, 2.5, 11, 12, '2023-11-05 10:00:00'),
                                                                                                                                         (4, 'PL 2005 1109 443 8', 8, 4, 'Signed Football Jersey', 2, 0.8, 7, 3, '2023-11-06 14:20:00');

        INSERT INTO Invoices (Id, ShipmentId, PayerId, Amount, Status, PaymentDate) VALUES
                                                                                        (1, 1, 1, 500.00, 'Paid', '2023-11-01 08:35:00'),
                                                                                        (3, 3, 11, 120.00, 'Paid', '2023-11-05 10:15:00'),
                                                                                        (4, 4, 8, 85.50, 'Paid', '2023-11-06 14:45:00');

-- 2. In Transit / Submitted (GB, DE, IT, FR)
        INSERT INTO Shipments (Id, TrackId, ShipperId, ConsigneeId, Description, SizeId, Weight, OriginId, DestinationId, CreatedAt) VALUES
                                                                                                                                         (2, 'GB 1005 8812 333 7', 2, 7, 'Peaky Blinders Script', 1, 0.5, 2, 5, '2023-11-02 09:45:00'),
                                                                                                                                         (5, 'DE 4100 9928 112 4', 5, 9, 'Tesla Spare Parts', 4, 45.0, 4, 8, '2023-11-10 11:00:00'),
                                                                                                                                         (6, 'FR 2020 5561 889 2', 4, 10, 'Vintage Wine Set', 2, 5.0, 3, 6, '2023-11-12 16:30:00'),
                                                                                                                                         (7, 'IT 1010 3342 771 0', 7, 13, 'Leather Jacket', 2, 1.2, 5, 1, '2023-11-15 09:00:00');

        INSERT INTO Invoices (Id, ShipmentId, PayerId, Amount, Status, PaymentDate) VALUES
                                                                                        (2, 2, 2, 45.00, 'Paid', '2023-11-02 09:50:00'),
                                                                                        (5, 5, 5, 1200.00, 'Pending', NULL), -- Ще не оплачено
                                                                                        (6, 6, 4, 210.00, 'Paid', '2023-11-12 16:40:00'),
                                                                                        (7, 7, 7, 95.00, 'Paid', '2023-11-15 09:10:00');

-- 3. New / Registered (Recent)
        INSERT INTO Shipments (Id, TrackId, ShipperId, ConsigneeId, Description, SizeId, Weight, OriginId, DestinationId, CreatedAt) VALUES
                                                                                                                                         (8, 'NL 1001 4455 667 9', 14, 15, 'Spider-Man Suit Props', 1, 2.0, 14, 2, '2023-11-20 13:00:00'),
                                                                                                                                         (9, 'PL 3110 8822 114 5', 12, 11, 'Action Camera Gear', 3, 3.5, 12, 11, '2023-11-21 10:15:00');

        INSERT INTO Invoices (Id, ShipmentId, PayerId, Amount, Status, PaymentDate) VALUES
                                                                                        (8, 8, 14, 60.00, 'Pending', NULL),
                                                                                        (9, 9, 12, 150.00, 'Paid', '2023-11-21 10:30:00');

        -- === TRACKINGS ===

        INSERT INTO Trackings (ShipmentId, StatusId, UpdateTime, OperatorId) VALUES
                                                                                 (1, 1, '2023-11-01 08:30:00', 1), -- Registered
                                                                                 (1, 2, '2023-11-01 10:00:00', 2), -- Submitted
                                                                                 (1, 3, '2023-11-02 12:00:00', 3), -- In Transit
                                                                                 (1, 4, '2023-11-04 15:30:00', 3); -- Delivered

        INSERT INTO Trackings (ShipmentId, StatusId, UpdateTime, OperatorId) VALUES
                                                                                 (2, 1, '2023-11-02 09:45:00', 1), -- Registered
                                                                                 (2, 2, '2023-11-02 11:00:00', 2), -- Submitted
                                                                                 (2, 3, '2023-11-03 08:00:00', 3); -- In Transit

        INSERT INTO Trackings (ShipmentId, StatusId, UpdateTime, OperatorId) VALUES
                                                                                 (5, 1, '2023-11-10 11:00:00', 1),
                                                                                 (5, 2, '2023-11-10 13:00:00', 2);
        
        COMMIT;