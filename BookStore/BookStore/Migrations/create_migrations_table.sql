IF
NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__migrations_history')
BEGIN
CREATE TABLE __migrations_history
(
    id          INT IDENTITY PRIMARY KEY,
    script_name NVARCHAR(255) NOT NULL,
    applied_on  DATETIME NOT NULL DEFAULT GETDATE()
)
END
