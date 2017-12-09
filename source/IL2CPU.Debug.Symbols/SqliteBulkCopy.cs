using Microsoft.Data.Sqlite;
using System;
using System.Data;

namespace Cosmos.Debug.Symbols
{
    public class SqliteBulkCopy : IDisposable
    {
        private bool mDisposed = false;

        public void Dispose()
        {
            if (mDisposed)
            {
                return;
            }
            mDisposed = true;
            GC.SuppressFinalize(this);
        }

        private readonly SqliteConnection mConnection;
        private string mDestinationTableName;

        public SqliteBulkCopy(SqliteConnection connection, string aDestinationTableName)
        {
            mConnection = connection;
            mDestinationTableName = aDestinationTableName;
        }

        public void WriteToServer(IDataReader reader)
        {
            if (reader.Read())
            {
                // initialize bulk copy
                using (var trans = mConnection.BeginTransaction())
                {
                    using (var command = mConnection.CreateCommand())
                    {
                        var fieldNames = "";
                        var paramNames = "";
                        SqliteParameter[] parms = new SqliteParameter[reader.FieldCount];
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string xFieldName = reader.GetName(i);
                            fieldNames += $"{xFieldName},";
                            paramNames += $"@_{xFieldName},";
                            parms[i] = new SqliteParameter($"@_{xFieldName}", SqliteType.Text);
                            command.Parameters.Add(parms[i]);
                        }
                        fieldNames = fieldNames.TrimEnd(',');
                        paramNames = paramNames.TrimEnd(',');

                        command.Transaction = trans;
                        command.CommandText = $"insert into [{mDestinationTableName}] ({fieldNames}) values ({paramNames})";
                        command.Prepare();
                        do
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (parms[i] != null)
                                {
                                    parms[i].Value = reader.GetValue(i);
                                }
                            }
                            command.ExecuteNonQuery();
                        }
                        while (reader.Read());
                    }
                    trans.Commit();
                }
            }
        }
    }
}
