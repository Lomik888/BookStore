using System.Data;
using System.Data.SqlClient;

namespace BookStore.Extations;

public static class SqlExtation
{
    public static void ExecuteTransaction(
        this IDbCommand command,
        IDbConnection connection,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        if (connection == null)
            throw new ArgumentNullException(nameof(connection));

        if (connection.State != ConnectionState.Open)
            connection.Open();

        using var transaction = connection.BeginTransaction(isolationLevel);

        try
        {
            command.Transaction = transaction;
            command.ExecuteNonQuery();

            // можно добавить более точно какие
            Console.WriteLine("Транзакции прошли");
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw new Exception("Ошибка применения транзакций");
        }
    }

    public static async Task ExecuteTransactionAsync(
        Func<SqlConnection, SqlTransaction, Task<IDbCommand?>> func,
        SqlConnection connection,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        if (connection == null)
            throw new ArgumentNullException(nameof(connection));

        await using var transaction = connection.BeginTransaction(isolationLevel);

        try
        {
            using var command = await func(connection, transaction);
            if (command == null)
            {
                await transaction.RollbackAsync();
                return;
            }

            command.ExecuteNonQuery();

            await transaction.CommitAsync();

            // можно добавить более точно какие
            Console.WriteLine("Транзакции прошли");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception(ex.Message);
        }
    }
}