namespace SqlApi
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using Dapper;
    using System.Dynamic;

    public class Api
    {
        string filedir = @"..\..\..\sql";

        string cnnStr = @"Data Source=localhost\dev;Initial Catalog=test;Connect Timeout=60;Persist Security Info=True;User ID=sa;Password=sqladmin";

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

        public List<string> GetParamList(int id)
        {
            var query = SqlFiles.FirstOrDefault(c => c.Id == id);
            if (query == null) return null;
            return query.ParamList;
        }

        public IEnumerable<ExpandoObject> Execute(int id, IDictionary<string, object> paramList = null)
        {
            var query = SqlFiles.FirstOrDefault(c => c.Id == id);
            if (query == null) return null;

            using (var cnn = new SqlConnection(cnnStr))
            {
                return cnn.Query(query.SqlText, paramList).Select(c => (ExpandoObject)ToExpandoDynamic(c));
            }
        }

        private static dynamic ToExpandoDynamic(object value)
        {
            var dapperRowProperties = value as IDictionary<string, object>;

            IDictionary<string, object> expando = new ExpandoObject();

            foreach (KeyValuePair<string, object> property in dapperRowProperties)
            {
                expando.Add(property.Key, property.Value);
            }

            return expando as ExpandoObject;
        }
    }
}
