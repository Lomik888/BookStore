CREATE TABLE books
(
    id              INT PRIMARY KEY IDENTITY,
    title           NVARCHAR(255) NOT NULL,
    author          NVARCHAR(255) NOT NULL,
    published_year   INT NOT NULL,
    table_of_contents XML NULL
);