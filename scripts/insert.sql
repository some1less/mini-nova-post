        PRAGMA foreign_keys = OFF;

        BEGIN TRANSACTION;

-- === DESTINATIONS ===
        INSERT INTO Destinations (Id, City, Street) VALUES
                                                        (1, 'Kyiv', 'Khreshchatyk 1'),
                                                        (2, 'London', 'Baker Street 221B'),
                                                        (3, 'Paris', 'Rue de Rivoli 14'),
                                                        (4, 'Berlin', 'Friedrichstrasse 101'),
                                                        (5, 'Rome', 'Via del Corso 50'),
                                                        (6, 'Madrid', 'Calle Mayor 12'),
                                                        (7, 'Warsaw', 'Nowy Swiat 4'),
                                                        (8, 'Prague', 'Charles Bridge 11'),
                                                        (9, 'Vienna', 'Stephansplatz 2'),
                                                        (10, 'Zurich', 'Bahnhofstrasse 7');

-- === OCCUPATIONS ===
        INSERT INTO Occupations (Id, Name, BaseSalary) VALUES
                                                           (1, 'Administrator', 40000),
                                                           (2, 'Manager', 25000),
                                                           (3, 'Courier', 18000),
                                                           (4, 'Warehouse Worker', 16000);

-- === ROLES ===
        INSERT INTO Roles (Id, Name, Description) VALUES
                                                      (1, 'Admin', 'Full access to the system'),
                                                      (2, 'Operator', 'Can manage packages and trackings'),
                                                      (3, 'User', 'Can create packages and view history');

        INSERT INTO People (Id, FirstName, LastName, Email, Phone) VALUES
                                                                       (1, 'Admin', 'Super', 'admin@mininova.com', '+380000000001'),
                                                                       (2, 'Emma', 'Watson', 'emma.watson@uk.co', '+442071234567'),   -- UK
                                                                       (3, 'Benedict', 'Cumberbatch', 'ben.c@uk.co', '+442089876543'), -- UK
                                                                       (4, 'Jean', 'Reno', 'jean.reno@fr.com', '+33140123456'),        -- France
                                                                       (5, 'Elon', 'Musk', 'elon@tesla.com', '+493011122233'),         -- Germany (Giga Berlin)
                                                                       (6, 'Cillian', 'Murphy', 'thomas.shelby@ie.com', '+35316123456'),-- Ireland
                                                                       (7, 'Monica', 'Bellucci', 'monica@it.it', '+390612345678'),     -- Italy
                                                                       (8, 'Robert', 'Lewandowski', 'robert@pl.com', '+48221234567'),  -- Poland
                                                                       (9, 'Mads', 'Mikkelsen', 'mads@dk.dk', '+4533123456'),          -- Denmark
                                                                       (10, 'Penelope', 'Cruz', 'penelope@es.es', '+34911234567');     -- Spain

-- admin -> 'admin123'
-- manager_olena -> 'pass123'
-- courier_petro -> 'pass123'
        INSERT INTO Accounts (Id, Login, Password, RoleId, PersonId) VALUES
                                                                         (1, 'admin', 'AQAAAAIAAYagAAAAEDO5+eS422XAfnhc22j+UZpFmdJ1O73L23UM9XSvGdjFH02ATLrdkC5AkWJhFLNWtg==', 1, 1),
                                                                         (2, 'manager_olena', 'AQAAAAIAAYagAAAAEFneVWuAdfXyFmNSgRuZ9nGkkgRSwmKrAcbiqyGvL0sHbXdSHHGGxjFudDhgaREJew==', 2, 2),
                                                                         (3, 'courier_petro', 'AQAAAAIAAYagAAAAEFneVWuAdfXyFmNSgRuZ9nGkkgRSwmKrAcbiqyGvL0sHbXdSHHGGxjFudDhgaREJew==', 2, 3);
-- === OPERATORS ===
        INSERT INTO Operators (Id, Salary, HireDate, OccupationId, PersonId) VALUES
                                                                                 (1, 45000, '2023-01-10', 1, 1),
                                                                                 (2, 27000, '2023-03-20', 2, 2),
                                                                                 (3, 20000, '2023-02-15', 3, 3),
                                                                                 (4, 17000, '2023-04-01', 4, 4);

-- === PACKAGES ===
        INSERT INTO Packages (Id, SenderId, ReceiverId, Description, Size, Weight, DestinationId) VALUES
                                                                                                      (1, 5, 6, 'Starlink Terminal', 'Large', 15.0, 7),
                                                                                                      (2, 6, 7, 'Peaky Blinders Script', 'Small', 0.5, 5),
                                                                                                      (3, 7, 5, 'Dior Fashion Collection', 'Medium', 5.0, 4),
                                                                                                      (4, 8, 9, 'Golden Boot Trophy', 'Small', 2.0, 9),
                                                                                                      (5, 9, 10, 'Lego Exclusive Set', 'Medium', 3.2, 6);

        INSERT INTO Invoices (Id, PackageId, PayerId, Amount, Status, PaymentDate) VALUES
                                                                                       (1, 1, 5, 500.00, 'Paid', '2023-11-01'),
                                                                                       (2, 2, 6, 45.00, 'Paid', '2023-11-02'),
                                                                                       (3, 3, 7, 120.00, 'Unpaid', NULL),
                                                                                       (4, 4, 8, 300.00, 'Paid', '2023-11-05'),
                                                                                       (5, 5, 9, 65.00, 'Pending', NULL);

        INSERT INTO Trackings (PackageId, OperatorId, Status, UpdateTime) VALUES
                                                                              -- Package 1: Full cycle
                                                                              (1, 2, 'Registered', '2023-11-01 09:00:00'),
                                                                              (1, 2, 'In Transit', '2023-11-01 14:30:00'),
                                                                              (1, 3, 'Delivered', '2023-11-03 10:00:00'),

                                                                              -- Package 2: In progress
                                                                              (2, 2, 'Registered', '2023-11-02 10:00:00'),
                                                                              (2, 3, 'In Transit', '2023-11-02 16:00:00'),

                                                                              -- Package 3: Canceled
                                                                              (3, 2, 'Registered', '2023-11-04 11:00:00'),
                                                                              (3, 2, 'Canceled', '2023-11-04 15:20:00'),

                                                                              -- Package 4: Only Registered
                                                                              (4, 2, 'Registered', '2023-11-05 12:00:00'),

                                                                              -- Package 5: Registered and Transit
                                                                              (5, 2, 'Registered', '2023-11-06 08:30:00'),
                                                                              (5, 3, 'In Transit', '2023-11-06 13:00:00');

        COMMIT;

                PRAGMA foreign_keys = ON;