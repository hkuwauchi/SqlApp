namespace SqlApi
{
    using Reactive.Bindings;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;

    public class Sql
    {
        public int Id => int.Parse(SqlFile.FileName?.Substring(0, SqlFile?.FileName?.IndexOf(".") ?? 0) ?? "-1");
        public SqlFile SqlFile { get; internal set; }
        public List<string> ParamNames { get; private set; }
        public string QueryText => GetQueryText(SqlFile.Lines);

        public ReactiveProperty<string> InfoMessage { get; private set; } = new ReactiveProperty<string>();
        public ReadOnlyReactiveCollection<string> InfoMessages;

        public Sql()
        {
            InfoMessages = InfoMessage.ToReadOnlyReactiveCollection(InfoMessage.Where(s => s == null).Select(_ => Unit.Default));
        }

        public Sql(string path)
            : this()
        {
            try
            {
                SqlFile = new SqlFile(path);
            }
            catch (Exception e)
            {
                InfoMessage.Value = e.Message;
            }
        }

        private string GetParamName(string line)
        {
            var parts = line.Split();
            return parts[1]?.Replace("@", "");
        }
        private List<string> GetParamNames(List<string> lines)
        {
            try
            {
                if (lines == null || lines.Count == 0) return null;

                if (lines.Count(c => c.ToLower().StartsWith("--param")) != 1) return null;

                if (lines.Count(c => c.ToLower().StartsWith("--sql")) != 1) return null;

                var s = lines.IndexOf(lines.First(c => c.ToLower().StartsWith("--param"))) + 1;
                var e = lines.IndexOf(lines.First(c => c.ToLower().StartsWith("--sql")));

                var num = (e - s) / 2;

                return Enumerable.Range(0, num).Select(i => GetParamName(lines[s + i * 2])).ToList();
            }
            catch (Exception e)
            {
                InfoMessage.Value = e.Message;
            }
            return null;
        }

        private string GetQueryText(List<string> lines)
        {
            if (lines == null || lines.Count == 0) return null;

            if (lines.Count(c => c.ToLower().StartsWith("--sql")) != 1) return null;

            var s = lines.IndexOf(lines.First(c => c.ToLower().StartsWith("--sql"))) + 1;

            var sql = string.Join(Environment.NewLine, lines.Skip(s));

            return sql;
        }
    }
}
