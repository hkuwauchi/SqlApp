namespace SqlApi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class SqlFile
    {
        public int Id { get; private set; }
        public string FilePath { get; private set; }
        public string FileName { get; private set; }
        public List<Declare> ParamList { get; private set; }
        public string SqlText { get; private set; }

        public SqlFile(string path)
        {
            FilePath = path;
            FileName = Path.GetFileName(FilePath);
            Id = int.Parse(FileName.Substring(0, FileName.IndexOf(".")));
            var lines = File.ReadLines(path).ToList();
            ParamList = GetDeclare(lines);
            SqlText = GetSqlText(lines, ParamList);
        }

        private List<Declare> GetDeclare(List<string> lines)
        {
            var declares = new List<Declare>();

            try
            {
                if (lines == null || lines.Count == 0) return declares;

                if (lines.Count(c => c.ToLower().StartsWith("--param")) != 1) return declares;

                if (lines.Count(c => c.ToLower().StartsWith("--sql")) != 1) return declares;

                var s = lines.IndexOf(lines.First(c => c.ToLower().StartsWith("--param"))) + 1;
                var e = lines.IndexOf(lines.First(c => c.ToLower().StartsWith("--sql")));

                var num = (e - s) / 2;

                foreach (var i in Enumerable.Range(0, num))
                {
                    var parts = lines[s + i * 2].Split();

                    var declare = new Declare() { Name = parts[1], Type = GetParamType(parts[2]) };
                    declares.Add(declare);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return declares;
        }

        private Type GetParamType(string type)
        {
            var text = string.Join("", type.Trim().TakeWhile(c => c == '(')).ToLower();
            switch (text)
            {
                case "int":
                    return Type.GetType("System.Int32");
                case "nchar":
                    return Type.GetType("System.String");
                case "varchar":
                    return Type.GetType("System.String");
                case "datetime":
                    return Type.GetType("System.DateTime");
                default:
                    break;
            }
            return null;
        }

        private string GetSqlText(List<string> lines, List<Declare> paramList)
        {
            if (lines.Count(c => c.ToLower().StartsWith("--sql")) != 1) return null;

            var s = lines.IndexOf(lines.First(c => c.ToLower().StartsWith("--sql"))) + 1;

            var sql = string.Join(Environment.NewLine, lines.Skip(s));

            if (paramList.Count == 0) return sql;

            foreach (var item in paramList.Select((v, i) => new { v, i }))
            {
                sql = sql.Replace(item.v.Name, $"@p{item.i + 1}");
            }

            return sql;
        }
    }
}
