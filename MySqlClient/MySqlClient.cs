using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace ClassLibrary
{
    public class MySqlClient : IDisposable
    {
        private readonly string connStr;
        private MySqlConnection conn;

        public MySqlClient(string connStr)
        {
            if (string.IsNullOrWhiteSpace(connStr))
            {
                throw new ArgumentException("IsNullOrWhiteSpace(connStr)", nameof(connStr));
            }

            this.connStr = connStr;
            conn = new MySqlConnection(connStr);
            conn.Open();
        }

        public void ExecuteReader(string sql, Func<MySqlDataReader, bool> func)
        {
            if (conn.State != System.Data.ConnectionState.Open)
                conn.Open();
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            using (MySqlDataReader rdr = cmd.ExecuteReader())
                while (rdr.Read())
                {
                    if (!func(rdr))
                        break;
                }
        }

        public object ExecuteScalar(string sql)
        {
            if (conn.State != System.Data.ConnectionState.Open)
                conn.Open();
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                return cmd.ExecuteScalar();
        }


        public int ExecuteNonQuery(string sql)
        {
            int res = 0;
            if (conn.State != System.Data.ConnectionState.Open)
                conn.Open();
            var tr = conn.BeginTransaction();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Transaction = tr;
                    res = cmd.ExecuteNonQuery();
                    tr.Commit();
                }
            }
            catch (Exception ex)
            {
                // Handle the exception if the transaction fails to commit.
                Console.WriteLine(ex.Message);
                try
                {
                    // Attempt to roll back the transaction.
                    tr.Rollback();
                }
                catch (Exception exRollback)
                {
                    // Throws an InvalidOperationException if the connection 
                    // is closed or the transaction has already been rolled 
                    // back on the server.
                    Console.WriteLine(exRollback.Message);
                }
            }
            return res;
        }

        //public int Insert(string sql)
        //{
        //    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
        //        return cmd.ExecuteNonQuery();
        //}

        public DataTable FillTable(string sql)
        {
            DataTable dt = new DataTable();

            using (var cmd = new MySqlCommand(sql, conn))
            {
                var da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        internal DataTable FillVerticalTable(string sql)
        {
            var ds = FillTable(sql);
            var dt = new DataTable();
            dt.Columns.Add("id");
            dt.Columns.Add("value");
            var rs = ds.Rows[0];
            for (int i = 0; i < ds.Columns.Count; i++)
            {
                dt.Rows.Add(ds.Columns[i].ColumnName, rs[i]);
            }
            return dt;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    conn?.Dispose();
                    conn = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MySqlClient()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}