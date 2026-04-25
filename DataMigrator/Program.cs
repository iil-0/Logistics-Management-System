using System;
using System.Data;
using Npgsql;

class Program
{
    static void Main()
    {
        string localConnStr = "Host=localhost; Port=5432; Database=LogiTechDB; Username=postgres; Password=12345";
        string remoteConnStr = "Host=ep-royal-butterfly-alb9v85s.c-3.eu-central-1.aws.neon.tech; Port=5432; Database=neondb; Username=neondb_owner; Password=npg_omrekC1Pub6F; SslMode=Require; TrustServerCertificate=true";

        using var localConn = new NpgsqlConnection(localConnStr);
        using var remoteConn = new NpgsqlConnection(remoteConnStr);

        localConn.Open();
        remoteConn.Open();

        Console.WriteLine("Migrating Users...");
        MigrateTable(localConn, remoteConn, "Users", "\"Id\", \"FirstName\", \"LastName\", \"Email\", \"Phone\", \"PasswordHash\", \"Role\", \"CreatedAt\"", "@p1, @p2, @p3, @p4, @p5, @p6, @p7, CAST(@p8 AS timestamp with time zone)");

        Console.WriteLine("Migrating Shipments...");
        MigrateTable(localConn, remoteConn, "Shipments", "\"Id\", \"TrackingNo\", \"SenderName\", \"SenderEmail\", \"SenderPhone\", \"ReceiverName\", \"ReceiverAddress\", \"ReceiverCity\", \"ReceiverPhone\", \"PackageType\", \"Extras\", \"TransportMethod\", \"TotalPrice\", \"Notes\", \"Status\", \"UserId\", \"CreatedAt\"", "@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15, @p16, CAST(@p17 AS timestamp with time zone)");

        Console.WriteLine("Migrating StatusHistories...");
        MigrateTable(localConn, remoteConn, "StatusHistories", "\"Id\", \"Status\", \"Date\", \"Message\", \"GonderiId\"", "@p1, @p2, CAST(@p3 AS timestamp with time zone), @p4, @p5");

        // Update sequences so new inserts don't fail
        UpdateSequence(remoteConn, "Users", "Id");
        UpdateSequence(remoteConn, "Shipments", "Id");
        UpdateSequence(remoteConn, "StatusHistories", "Id");

        Console.WriteLine("Data migration complete!");
    }

    static void MigrateTable(NpgsqlConnection localConn, NpgsqlConnection remoteConn, string tableName, string columns, string paramNames)
    {
        // First delete existing remote data to avoid primary key conflicts
        using (var deleteCmd = new NpgsqlCommand($"TRUNCATE TABLE \"{tableName}\" CASCADE", remoteConn))
        {
            deleteCmd.ExecuteNonQuery();
        }

        using var selectCmd = new NpgsqlCommand($"SELECT {columns} FROM \"{tableName}\"", localConn);
        using var reader = selectCmd.ExecuteReader();

        while (reader.Read())
        {
            using var insertCmd = new NpgsqlCommand($"INSERT INTO \"{tableName}\" ({columns}) VALUES ({paramNames})", remoteConn);
            
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var val = reader.GetValue(i);
                var colName = reader.GetName(i);
                
                if (colName == "CreatedAt" || colName == "Date")
                {
                    if (val is string s) 
                    {
                        if (string.IsNullOrWhiteSpace(s)) val = DateTime.UtcNow;
                        else val = DateTime.Parse(s).ToUniversalTime();
                    }
                    else if (val is DateTime dt) val = dt.ToUniversalTime();
                }
                else if (val is DateTime dtVal)
                {
                    val = dtVal.ToUniversalTime();
                }
                
                var p = insertCmd.Parameters.AddWithValue($"@p{i + 1}", val == DBNull.Value ? DBNull.Value : val);
                if (val is DateTime)
                {
                    p.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.TimestampTz;
                }
            }

            insertCmd.ExecuteNonQuery();
        }
    }

    static void UpdateSequence(NpgsqlConnection conn, string tableName, string idCol)
    {
        using var cmd = new NpgsqlCommand($"SELECT setval(pg_get_serial_sequence('\"{tableName}\"', '{idCol}'), coalesce(max(\"{idCol}\"), 1) + 1, false) FROM \"{tableName}\";", conn);
        cmd.ExecuteNonQuery();
    }
}
