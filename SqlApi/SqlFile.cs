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
        public List<string> ParamList { get; private set; }
        public string SqlText { get; private set; }

        public SqlFile(string path)
        {
            FilePath = path;
            FileName = Path.GetFileName(FilePath);
            Id = int.Parse(FileName.Substring(0, FileName.IndexOf(".")));
            var lines = File.ReadLines(path).ToList();
            ParamList = GetParamList(lines);
            SqlText = GetSqlText(lines, ParamList);
        }

        private List<string> GetParamList(List<string> lines)
        {
            var paramList = new List<string>();

            try
            {
                if (lines == null || lines.Count == 0) return paramList;

                if (lines.Count(c => c.ToLower().StartsWith("--param")) != 1) return paramList;

                if (lines.Count(c => c.ToLower().StartsWith("--sql")) != 1) return paramList;

                var s = lines.IndexOf(lines.First(c => c.ToLower().StartsWith("--param"))) + 1;
                var e = lines.IndexOf(lines.First(c => c.ToLower().StartsWith("--sql")));

                var num = (e - s) / 2;

                foreach (var i in Enumerable.Range(0, num))
                {
                    var parts = lines[s + i * 2].Split();

                    paramList.Add(parts[1].Replace("@", ""));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return paramList;
        }

        private string GetSqlText(List<string> lines, List<string> paramList)
        {
            if (lines.Count(c => c.ToLower().StartsWith("--sql")) != 1) return null;

            var s = lines.IndexOf(lines.First(c => c.ToLower().StartsWith("--sql"))) + 1;

            var sql = string.Join(Environment.NewLine, lines.Skip(s));

            return sql;
        }
    }
}
