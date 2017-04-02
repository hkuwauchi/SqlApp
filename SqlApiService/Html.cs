namespace SqlApiService
{
    using LightNode.Server;
    using LightNode.Formatter;

    /// <summary>
    /// 
    /// </summary>
    public class Html : OperationOptionAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="acceptVerbs"></param>
        public Html(AcceptVerbs acceptVerbs = AcceptVerbs.Get | AcceptVerbs.Post)
            : base(acceptVerbs, typeof(HtmlContentFormatterFactory))
        {

        }
    }
}
