        PRAGMA foreign_keys = OFF;

        BEGIN TRANSACTION;

        INSERT INTO Destinations (Id, City, Street) VALUES
                                                        (1, 'Kyiv', 'Khreshchatyk 1'),
                                                        (2, 'Lviv', 'Rynok Square 10'),
                                                        (3, 'Odesa', 'Derybasivska 5'),
                                                        (4, 'Kharkiv', 'Sumska 12'),
                                                        (5, 'Dnipro', 'Yavornytskoho 50'),
                                                        (6, 'Vinnytsia', 'Soborna 22'),
                                                        (7, 'Ivano-Frankivsk', 'Nezalezhnosti 4'),
                                                        (8, 'Poltava', 'Nebesnoi Sotni 11'),
                                                        (9, 'Chernivtsi', 'Kobylianskoi 2'),
                                                        (10, 'Uzhhorod', 'Korzo 7');

        INSERT INTO Occupations (Id, Name, BaseSalary) VALUES
                                                           (1, 'Administrator', 40000),
                                                           (2, 'Manager', 25000),
                                                           (3, 'Courier', 18000),
                                                           (4, 'Warehouse Worker', 16000);

        INSERT INTO Roles (Id, Name, Description) VALUES
                                                      (1, 'Admin', 'Full access to the system'),
                                                      (2, 'Operator', 'Can manage packages and trackings'),
                                                      (3, 'User', 'Can create packages and view history');

        INSERT INTO People (Id, FirstName, LastName, Email, Phone) VALUES
                                                                       (1, 'Admin', 'Super', 'admin@mininova.com', '+380000000001'),
                                                                       (2, 'Olena', 'Menedzher', 'olena.mgr@mininova.com', '+380502223344'),
                                                                       (3, 'Petro', 'Vodziy', 'petro.courier@mininova.com', '+380501112233'),
                                                                       (4, 'Ivan', 'Sklad', 'ivan.sklad@mininova.com', '+380503334455'),
                                                                       (5, 'Taras', 'Shevchenko', 'taras@poetry.ua', '+380971234567'),
                                                                       (6, 'Lesia', 'Ukrainka', 'lesia@poetry.ua', '+380977654321'),
                                                                       (7, 'Ivan', 'Franko', 'ivan@kamenyar.ua', '+380631111111'),
                                                                       (8, 'Bohdan', 'Khmelnytsky', 'bohdan@hetman.ua', '+380632222222'),
                                                                       (9, 'Hryhoriy', 'Skovoroda', 'hryhoriy@phil.ua', '+380633333333'),
                                                                       (10, 'Mykola', 'Gogol', 'mykola@writer.ua', '+380634444444');


        -- admin -> 'admin123'
-- manager_olena -> 'pass123'
-- courier_petro -> 'pass123'
        INSERT INTO Accounts (Id, Login, Password, RoleId, PersonId) VALUES
                                                                         (1, 'admin', 'AQAAAAIAAYagAAAAEDO5+eS422XAfnhc22j+UZpFmdJ1O73L23UM9XSvGdjFH02ATLrdkC5AkWJhFLNWtg==', 1, 1),
                                                                         (2, 'manager_olena', 'AQAAAAIAAYagAAAAEFneVWuAdfXyFmNSgRuZ9nGkkgRSwmKrAcbiqyGvL0sHbXdSHHGGxjFudDhgaREJew==', 2, 2),
                                                                         (3, 'courier_petro', 'AQAAAAIAAYagAAAAEFneVWuAdfXyFmNSgRuZ9nGkkgRSwmKrAcbiqyGvL0sHbXdSHHGGxjFudDhgaREJew==', 2, 3);

        INSERT INTO Operators (Id, Salary, HireDate, OccupationId, PersonId) VALUES
                                                                                 (1, 45000, '2023-01-10', 1, 1),
                                                                                 (2, 27000, '2023-03-20', 2, 2), -- Manager
                                                                                 (3, 20000, '2023-02-15', 3, 3), -- Courier
                                                                                 (4, 17000, '2023-04-01', 4, 4); -- Warehouse

        INSERT INTO Packages (Id, SenderId, ReceiverId, Description, Size, Weight, DestinationId) VALUES
                                                                                                      (1, 5, 6, 'Books specifically Kobzar', 'Small', 1.5, 2),   -- Taras -> Lesia (Lviv)
                                                                                                      (2, 6, 7, 'Manuscripts', 'Medium', 3.0, 1),                -- Lesia -> Ivan (Kyiv)
                                                                                                      (3, 7, 5, 'Documents archive', 'Large', 10.5, 3),          -- Ivan -> Taras (Odesa)
                                                                                                      (4, 8, 9, 'Souvenirs', 'Small', 0.5, 4),                   -- Bohdan -> Hryhoriy (Kharkiv)
                                                                                                      (5, 9, 10, 'Philosophy books', 'Medium', 5.2, 5),          -- Hryhoriy -> Mykola (Dnipro)
                                                                                                      (6, 10, 6, 'Old painting', 'Large', 12.0, 2),              -- Mykola -> Lesia (Lviv)
                                                                                                      (7, 5, 8, 'Personal belongings', 'Large', 20.0, 6),        -- Taras -> Bohdan (Vinnytsia)
                                                                                                      (8, 6, 9, 'Letter collection', 'Small', 0.2, 4);           -- Lesia -> Hryhoriy (Kharkiv)

        INSERT INTO Invoices (Id, PackageId, PayerId, Amount, Status, PaymentDate) VALUES
                                                                                       (1, 1, 5, 150.00, 'Paid', '2023-10-01'),
                                                                                       (2, 2, 6, 200.50, 'Paid', '2023-10-02'),
                                                                                       (3, 3, 7, 500.00, 'Unpaid', NULL),
                                                                                       (4, 4, 8, 80.00, 'Paid', '2023-10-05'),
                                                                                       (5, 5, 9, 250.00, 'Pending', NULL),
                                                                                       (6, 6, 10, 600.00, 'Paid', '2023-10-07'),
                                                                                       (7, 7, 5, 1000.00, 'Unpaid', NULL),
                                                                                       (8, 8, 6, 50.00, 'Paid', '2023-10-10');

        INSERT INTO Trackings (PackageId, OperatorId, Status, UpdateTime) VALUES
                                                                              (1, 2, 'Registered', '2023-10-01 09:00:00'),
                                                                              (1, 2, 'Confirmed', '2023-10-01 10:30:00'),
                                                                              (1, 3, 'Delivery', '2023-10-02 08:00:00'),
                                                                              (1, 3, 'Delivered', '2023-10-03 14:00:00'),

                                                                              (2, 2, 'Registered', '2023-10-02 09:15:00'),
                                                                              (2, 2, 'Confirmed', '2023-10-02 11:00:00'),
                                                                              (2, 3, 'Delivery', '2023-10-03 09:30:00'),

                                                                              (3, 2, 'Registered', '2023-10-04 10:00:00'),
                                                                              (3, 2, 'Confirmed', '2023-10-04 12:45:00'),

                                                                              (4, NULL, 'Registered', '2023-10-05 16:20:00');

        COMMIT;

                PRAGMA foreign_keys = ON;