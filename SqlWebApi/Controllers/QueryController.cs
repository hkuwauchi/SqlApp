namespace SqlWebApi.Controllers
{
    using SqlApi;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    public class QueryController : ApiController
    {
        // GET: api/Query
        public IEnumerable<IDictionary<string, object>> Get(int id, [FromBody]IEnumerable<string[]> values)
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
    }
}
