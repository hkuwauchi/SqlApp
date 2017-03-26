namespace SqlWebApi.Controllers
{
    using SqlApi;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    public class ListController : ApiController
    {
        // GET: api/List
        public IEnumerable<int> Get()
        {
            var api = new Api();
            return api.SqlDic.Keys;
        }

        // GET: api/List/5
        public IEnumerable<string> Get(int id)
        {
            var api = new Api();
            return api.GetParamNames(id);
        }
    }
}
