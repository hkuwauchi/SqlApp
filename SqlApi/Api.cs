namespace SqlApi
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Dapper;

    public class Api
    {
        string filedir = @"C:\Users\hisa\Desktop\sql";

        List<SqlFile> sqlFiles;

        public List<SqlFile> SqlFiles => sqlFiles ?? (sqlFiles = GetSqlFiles());

        private List<SqlFile> GetSqlFiles()
        {
            try
            {
                var files = Directory.EnumerateFiles(filedir);
                return files.Where(c => c.ToLower().EndsWith(".sql")).Select(c => new SqlFile(c)).ToList();
            }
            catch (Exception)
            { }
            return null;
        }

        public void Execute(int id)
        {
            var query = SqlFiles.FirstOrDefault(c => c.Id == id);
            if (query == null) return;

            var cnnStr = @"Data Source=localhost\dev;Initial Catalog=test;Connect Timeout=60;Persist Security Info=True;User ID=sa;Password=sqladmin";

            using (var cnn = new SqlConnection(cnnStr))
            {

                var res = cnn.Query(query.SqlText);
                foreach (var row in res)
                {
                    Console.WriteLine(string.Join(",", row));
                }
            }
        }

        public void Execute(int id, object paramList)
        {
            var query = SqlFiles.FirstOrDefault(c => c.Id == id);
            if (query == null) return;

            var cnnStr = @"Data Source=localhost\dev;Initial Catalog=test;Connect Timeout=60;Persist Security Info=True;User ID=sa;Password=sqladmin";

            using (var cnn = new SqlConnection(cnnStr))
            {
                var res = cnn.Query(query.SqlText, paramList);
                foreach (var row in res)
                {
                    Console.WriteLine(string.Join(",", row));
                }
            }
        }

        public void Execute(int id, string[] paramList)
        {
            var query = SqlFiles.FirstOrDefault(c => c.Id == id);
            if (query == null) return;

            var cnnStr = @"Data Source=localhost\dev;Initial Catalog=test;Connect Timeout=60;Persist Security Info=True;User ID=sa;Password=sqladmin";

            using (var cnn = new SqlConnection(cnnStr))
            {
                var sqlText = query.SqlText;
                foreach (var p in paramList.Select((v, i) => new { v, i }))
                {
                    sqlText = sqlText.Replace($"@p{p.i + 1}", $"'{p.v}'");
                }

                var res = cnn.Query(sqlText);
                foreach (var row in res)
                {
                    Console.WriteLine(string.Join(",", row));
                }
            }
        }
    }
}
