namespace SqlApi
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using Dapper;
    using System.Dynamic;
    using Reactive.Bindings;
    using System.Reactive.Linq;
    using System.Reactive;

    public class Api
    {

        string cnnStr = @"Data Source=localhost\dev;Initial Catalog=test;Connect Timeout=60;Persist Security Info=True;User ID=sa;Password=sqladmin";

        public ReactiveProperty<string> InfoMessage { get; private set; } = new ReactiveProperty<string>();
        public ReadOnlyReactiveCollection<string> InfoMessages;

        public HashSet<string> DirPathSet { get; set; } = new HashSet<string>();
        //public HashSet<Sql> SqlSet { get; private set; } = new HashSet<Sql>(AnonymousComparer.Create<Sql>((x, y) => x.Id == y.Id, obj => obj.Id.GetHashCode()));
        public Dictionary<int, Sql> SqlDic { get; private set; } = new Dictionary<int, Sql>();

        public Api()
        {
            InfoMessages = InfoMessage.ToReadOnlyReactiveCollection(InfoMessage.Where(s => s == null).Select(_ => Unit.Default));
            AddSqlDic(SqlFactory.CreateDefaultSqlSet());
        }

        public Api(string dirPath)
            : this()
        {
            AddDirPath(dirPath);
            AddSqlDic(GetSqlList(dirPath));
        }

        public Api(IEnumerable<string> dirs)
            : this()
        {
            foreach (var path in dirs)
            {
                AddDirPath(path);
                AddSqlDic(GetSqlList(path));
            }
        }

        private void AddSqlDic(IEnumerable<Sql> sqlList)
        {
            if (sqlList == null) return;
            foreach (var d in sqlList.Where(c => !SqlDic.ContainsKey(c.Id)))
            {
                SqlDic.Add(d.Id, d);
            }
        }


        private void UpdateSqlSet()
        {
            SqlDic.Clear();
            AddSqlDic(SqlFactory.CreateDefaultSqlSet());
            foreach (var path in DirPathSet)
            {
                AddSqlDic(GetSqlList(path));
            }
        }

        private bool AddDirPath(string path)
        {
            if (!Directory.Exists(path)) return false;
            return DirPathSet.Add(path);
        }

        private IEnumerable<Sql> GetSqlList(string dirPath)
        {
            try
            {
                if (!Directory.Exists(dirPath))
                {
                    InfoMessage.Value = $"{dirPath}が存在しません。";
                    return null;
                }
                var files = Directory.EnumerateFiles(dirPath);
                return files.Where(c => c.ToLower().EndsWith(".sql")).Select(c => new Sql(c));
            }
            catch (Exception e)
            {
                InfoMessage.Value = e.Message;
            }
            return null;
        }

        public List<string> GetParamNames(int id)
        {
            if (!SqlDic.ContainsKey(id)) return null;
            return SqlDic[id].ParamNames;
        }
        private IDictionary<string, object> GetArgs(int id, params string[] args)
        {
            var names = GetParamNames(id);
            if (names == null) return null;
            return names.Zip(args, (n, v) => new { n, v }).ToDictionary(d => d.n, d => (object)d.v);
        }

        private IEnumerable<IDictionary<string, object>> GetArgs(int id, IEnumerable<string[]> args)
        {
            var names = GetParamNames(id);
            if (names == null) return null;
            if (args == null) return null;
            return args.Select(a => names.Zip(a, (n, v) => new { n, v }).ToDictionary(d => d.n, d => (object)d.v)).ToList();
        }

        public IEnumerable<ExpandoObject> Query(int id, params string[] args)
        {
            if (!SqlDic.ContainsKey(id)) return null;
            var sql = SqlDic[id];

            var paramList = GetArgs(id, args);

            try
            {
                using (var cnn = new SqlConnection(cnnStr))
                {
                    return cnn.Query(sql.QueryText, paramList).Select(c => (ExpandoObject)ToExpandoDynamic(c));
                }
            }
            catch (Exception e)
            {
                InfoMessage.Value = e.Message;
            }
            return null;
        }

        private void Cnn_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            InfoMessage.Value = e.Message;
        }

        public int Execute(int id, IEnumerable<string[]> args = null)
        {
            if (!SqlDic.ContainsKey(id)) return -1;
            var sql = SqlDic[id];

            var paramList = GetArgs(id, args);

            try
            {
                using (var cnn = new SqlConnection(cnnStr))
                {
                    cnn.InfoMessage += Cnn_InfoMessage;
                    return cnn.Execute(sql.QueryText, paramList);
                }
            }
            catch (Exception e)
            {
                InfoMessage.Value = e.Message;
            }
            return -1;
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
