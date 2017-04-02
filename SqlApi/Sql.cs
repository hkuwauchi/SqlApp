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
        public Dictionary<string, string> Header { get; private set; }
        public string QueryText => GetQueryText(SqlFile.Lines);

        private HashSet<string> KeyWords = new HashSet<string>() { "description", "author", "client" };

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
                ParamNames = GetParamNames(SqlFile.Lines);
                Header = CreateHeaderDic(SqlFile.Lines);
            }
            catch (Exception e)
            {
                InfoMessage.Value = e.Message;
            }
        }

        private List<string> GetHeader(List<string> lines)
        {
            try
            {
                if (lines == null || lines.Count == 0) return null;

                var s = lines.IndexOf(lines.First(c => c.ToLower().StartsWith("/*"))) + 1;
                var e = lines.IndexOf(lines.First(c => c.ToLower().StartsWith("*/")));

                var num = (e - s);

                return Enumerable
                    .Range(0, num)
                    .Select(i => lines[s + i])
                    .ToList();
            }
            catch (Exception e)
            {
                InfoMessage.Value = e.Message;
            }
            return null;
        }

        private bool IsKeyWordValue(string keyWord, string line)
        {
            if (line.StartsWith(keyWord)) return false;
            var otherKeyWords = new HashSet<string>(KeyWords);
            otherKeyWords.ExceptWith(new[] { keyWord });
            if (otherKeyWords.Any(c => line.StartsWith(c))) return false;
            return true;
        }

        private Dictionary<string, string> CreateHeaderDic(List<string> lines)
        {
            try
            {
                var header = GetHeader(lines);

                if (header == null) return null;

                var dic = new Dictionary<string, List<string>>();

                foreach (var keyWord in KeyWords)
                {
                    dic[keyWord] = new List<string>();
                    foreach (var line in header.Where(c => IsKeyWordValue(keyWord, c)))
                    {
                        dic[keyWord].Add(line);
                    }

                    foreach(var del in dic[keyWord])
                    {
                        header.Remove(del);
                    }
                }

                var headerDic = new Dictionary<string, string>
                {
                    ["description"] = dic["description"].Count == 0 ? "no description" : string.Join(Environment.NewLine, dic["description"]),
                    ["author"] = dic["author"].Count == 0 ? "unknown" : string.Join(Environment.NewLine, dic["author"]),
                    ["client"] = dic["client"].Count == 0 ? "unknown" : string.Join(Environment.NewLine, dic["client"])
                };
                return headerDic;
            }
            catch (Exception e)
            {
                InfoMessage.Value = e.Message;
            }
            return null;
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

                var num = (e - s);

                return Enumerable
                    .Range(0, num)
                    .Select(i => lines[s + i])
                    .Where(c => c.ToLower().StartsWith("declare"))
                    .Select(c => GetParamName(c))
                    .ToList();
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
