using NuFridge.Common.Helpers;
using NuFridge.DataAccess.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NuFridge.Website.MVC.Controllers.API
{
    public class MongoDBController : ApiController
    {
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetMongoDBServer()
        {
            try
            {
                string serverName = ConfigHelper.GetMongoDBServerName();

                return Request.CreateResponse<string>(HttpStatusCode.OK, serverName);
            }
            catch (Exception ex)
            {

            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }

        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetMongoDBDatabase()
        {
            try
            {
                string databaseName = ConfigHelper.GetMongoDBDatabaseName();

                return Request.CreateResponse<string>(HttpStatusCode.OK, databaseName);
            }
            catch (Exception ex)
            {

            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }



        [System.Web.Mvc.HttpPost]
        [ActionName("TestDatabaseExists")]
        public HttpResponseMessage PostTestDatabaseExists(string server, string database)
        {
            var result = MongoRead.TestDatabaseExists(string.Format("mongodb://{0}", server), database);

            return Request.CreateResponse<bool>(HttpStatusCode.OK, result);
        }

        [System.Web.Mvc.HttpPost]
        [ActionName("TestDatabaseConnection")]
        public HttpResponseMessage PostTestDatabaseConnection(string server)
        {
            var result = MongoRead.TestConnectionString(string.Format("mongodb://{0}", server));

            return Request.CreateResponse<bool>(HttpStatusCode.OK, result);
        }
    }
}