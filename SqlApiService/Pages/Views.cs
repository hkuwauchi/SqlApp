namespace SqlApiService.Pages
{
    using LightNode.Server;

    /// <summary>
    /// View を返します。
    /// </summary>

    class Views : RazorContractBase
    {
        [IgnoreClientGenerate]
        [Html]
        public string Index()
        {
            return View("Index.cshtml");
        }
    }
}