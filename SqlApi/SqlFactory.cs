namespace SqlApi
{
    using LibGit2Sharp;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class SqlFactory
    {
        public static IEnumerable<Sql> CreateDefaultSqlSet()
        {
            var repoPath = "SqlFiles";

            if (!Directory.Exists(repoPath))
            {
                Repository.Clone(
                    @"D:\work\repo\SqlFiles",
                    repoPath
                    );
            }
            using (var repo = new Repository(repoPath))
            {
                var po = new PullOptions()
                {
                    FetchOptions = new FetchOptions() { }
                };

                var signature = new Signature("hkuwauchi", "hkuwauchi@gmail.com", new DateTimeOffset(DateTime.Now));

                Commands.Pull(repo, signature, po);
            }

            return Directory
                    .EnumerateFiles(repoPath, "*.sql", SearchOption.AllDirectories)
                    .Select(c => new Sql(c))
                    .ToList();
        }
    }
}
