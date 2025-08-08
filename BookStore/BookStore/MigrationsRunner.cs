using System.Data;
using System.Data.SqlClient;
using BookStore.Extations;

namespace BookStore;

public class MigrationsRunner
{
    private const string SEPARATOR = "\n";
    private const string CREATE_MIGRATION_TABLE = "create_migrations_table.sql";

    private readonly string _connectionString =
        "Server=db;Database=master;User Id=sa;Password=Root!1234;TrustServerCertificate=True;";

    private readonly string _migrationsPath = Path.Combine(AppContext.BaseDirectory, "Migrations");

    public MigrationsRunner(IConfiguration configuration)
    {
    }

    public async Task RunAsync()
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await SqlExtation.ExecuteTransactionAsync(
            async (sqlConnection, trasaction) => await GetCommand(sqlConnection, trasaction),
            connection,
            IsolationLevel.Serializable);
    }

    /// <summary>
    /// Метод, который полностью отвечает за формирование команды 
    /// </summary>
    private async ValueTask<SqlCommand?> GetCommand(SqlConnection connection, SqlTransaction trasaction)
    {
        var files = Directory.GetFiles(_migrationsPath, "*.sql")
            .OrderBy(x => x)
            .ToList();

        var createMigrationsTable = files.Single(x => Path.GetFileName(x) == CREATE_MIGRATION_TABLE);
        var createMigrationsTableScript = await File.ReadAllTextAsync(createMigrationsTable);

        await using var migrationCommand = new SqlCommand(createMigrationsTableScript, connection, trasaction);
        await migrationCommand.ExecuteNonQueryAsync();

        var completedMigrationsTask = GetCompletedMigrationsAsync(connection, trasaction);

        string? sql = "";
        var parameters = new List<SqlParameter>();
        var completedMigrations = await completedMigrationsTask;
        var counter = 0;

        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);

            if (completedMigrations.Contains(fileName)) continue;

            var scriptMigration = await File.ReadAllTextAsync(file);

            var paramName = $"@fileName{counter}";

            var scriptInsertMigration = $"""
                                            insert into __migrations_history 
                                            (script_name)
                                            values ({paramName});
                                         """;

            sql = string.Join(SEPARATOR, sql, scriptMigration, scriptInsertMigration);
            var parameter = new SqlParameter(paramName, fileName);
            parameters.Add(parameter);
            counter += 1;
        }

        if (string.IsNullOrEmpty(sql)) return null;

        var command = new SqlCommand(sql, connection);
        command.Parameters.AddRange(parameters.ToArray());
        command.Transaction = trasaction;

        return command;
    }

    /// <summary>
    /// Вернёт все текущие применённые миграции
    /// </summary>
    private async Task<List<string>> GetCompletedMigrationsAsync(SqlConnection conn, SqlTransaction trasaction)
    {
        var result = new List<string>();

        var sql = $"""
                   SELECT script_name FROM __migrations_history
                   """;

        await using var cmd = new SqlCommand(sql, conn, trasaction);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            result.Add(reader.GetString(0));

        return result;
    }
}