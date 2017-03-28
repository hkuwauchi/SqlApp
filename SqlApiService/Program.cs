namespace SqlApiService
{
    using Topshelf;

    class Program
    {
        private static readonly string _serviceName = "SqlApiService";
        private static readonly string _displayName = "SqlApiService";
        private static readonly string _description = "SqlApiService";

        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                // Automate recovery
                x.EnableServiceRecovery(recover =>
                {
                    recover.RestartService(0);
                });

                // Reference to Logic Class
                x.Service<Startup>(s =>
                {
                    s.ConstructUsing(name => new Startup(_serviceName));
                    s.WhenStarted(sc => sc.Start());
                    s.WhenStopped(sc => sc.Stop());
                });

                // Service Start mode
                x.StartAutomaticallyDelayed();

                // Service RunAs
                x.RunAsLocalSystem();

                // Service information
                x.SetServiceName(_serviceName);
                x.SetDisplayName(_displayName);
                x.SetDescription(_description);
            });
        }
    }
}
