namespace SqlApiService.Api
{
    using LightNode.Server;
    using System.Collections.Generic;
    using SqlApi;
    using System.Linq;

    /// <summary>
    /// Sql
    /// </summary>
    public class Sql : LightNodeContract
    {

        /// <summary>
        /// 実装されているAPIの一覧を返します。
        /// </summary>
        /// <remarks>APIのUriを返します。</remarks>
        /// <returns></returns>
        [Post]
        public string[] ListApi()
        {
            var apis = LightNodeServerMiddleware.GetRegisteredHandlersInfo();
            var key = apis.Select(x => x.Key).First();
            return apis[key].SelectMany(x => x.RegisteredHandlers).Select(x => x.Key).ToArray();
        }

        /// <summary>
        /// SQLを実行します。
        /// </summary>
        /// <param name="id"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        [Post]
        public IEnumerable<IDictionary<string, object>> Query(int id, string[] values)
        {
            var api = new Api();
            return api.Query(id, values);
        }

        /// <summary>
        /// クエリのリストを取得します。
        /// </summary>
        /// <returns></returns>
        [Get]
        public IEnumerable<int> List()
        {
            var api = new Api();
            return api.SqlDic.Keys;
        }

        /// <summary>
        /// パラメータ名を取得します。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Get]
        public IEnumerable<string> GetParamNames(int id)
        {
            var api = new Api();
            return api.GetParamNames(id);
        }
    }
}
