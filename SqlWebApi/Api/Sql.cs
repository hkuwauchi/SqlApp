namespace SqlWebApi.Api
{
    using LightNode.Server;
    using System.Collections.Generic;
    using System.Linq;
    using SqlApi;
    using System.Web.Http;

    /// <summary>
    /// Sql
    /// </summary>
    public class Sql: LightNodeContract
    {
        /// <summary>
        /// SQLを実行します。
        /// </summary>
        /// <param name="id"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        [Post]
        public IEnumerable<IDictionary<string, object>> Query(int id, [FromBody]IEnumerable<string[]> values)
        {
            var api = new Api();
            if (values.Count() == 1)
            {
                return api.Query(id, values.First());
            }
            else
            {
                var cnt = api.Execute(id, values);
                var list = new List<IDictionary<string, object>>();

                return new List<IDictionary<string, object>>()
                {
                    new Dictionary<string, object>()
                    {
                        ["row"] = cnt
                    }
                };
            }
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