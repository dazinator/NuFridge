using NuFridge.Common.Manager;
using NuFridge.DataAccess.Model;
using NuFridge.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NuFridge.Website.MVC.Controllers
{
    public class FeedsController : ApiController
    {
        private FeedManager _feedManager;

        public FeedsController(FeedManager feedManager)
        {
            _feedManager = feedManager;
        }

        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetAllFeeds()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _feedManager.GetAll());
            }
            catch (MongoDB.Driver.MongoConnectionException ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                              new HttpError("There was an error connecting to MongoDB. " + ex.Message));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new HttpError(ex.Message));
            }
        }


        [System.Web.Mvc.HttpGet]
        public Feed GetFeed(Guid id)
        {
            Feed item = _feedManager.GetById(id);
            if (item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return item;
        }

        [System.Web.Mvc.HttpPost]
        public HttpResponseMessage PostFeed(Feed item)
        {
            string message;
            var result = _feedManager.CreateFeed(item, out message);

            if (result)
            {
                return Request.CreateResponse<Feed>(HttpStatusCode.Created, item);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, message);
            }
        }

        [System.Web.Mvc.HttpPut]
        public void PutFeed(Guid id, Feed newFeed)
        {
            newFeed.Id = id;

            string message;
            var result = _feedManager.UpdateFeed(newFeed, out message);
            if (!result)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [System.Web.Mvc.HttpDelete]
        public void DeleteFeed(Guid id)
        {
            Feed item = _feedManager.GetById(id);
            if (item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            string message;
            var result = _feedManager.DeleteFeed(item, out message);

            if (!result)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }
    }
}