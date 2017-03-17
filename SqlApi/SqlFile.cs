namespace SqlApi
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    public class SqlFile
    {
        public string FilePath { get; internal set; }
        public string FileName { get; internal set; }
        public string RawText { get; internal set; }
        public List<string> Lines {

            get
            {
                return RawText?.Replace("\r\n", "\n")?.Split('\n').ToList();
            }
        }

        public SqlFile() { }

        public SqlFile(string path)
        {
            FilePath = path;
            FileName = Path.GetFileNameWithoutExtension(path);
            RawText = File.ReadAllText(path);
        }
    }
}
