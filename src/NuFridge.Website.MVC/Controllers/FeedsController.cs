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
        private readonly IRepository<Feed> _repository;

        public FeedsController(IRepository<Feed> repository)
        {
            _repository = repository;
        }

        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetAllFeeds()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _repository.GetAll());
            }
            catch (MongoDB.Driver.MongoConnectionException ex)
            {
               return Request.CreateResponse(HttpStatusCode.InternalServerError, new HttpError("There was an error connecting to MongoDB. " + ex.Message));
            }
        }


        [System.Web.Mvc.HttpGet]
        public Feed GetFeed(Guid id)
        {
            Feed item = _repository.GetById(id);
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
            var result = FeedManager.CreateFeed(item, out message);

            if (result)
            {
                _repository.Insert(item);

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
            var result = FeedManager.UpdateFeed(newFeed, out message);
            if (result)
            {
                _repository.Update(newFeed);
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [System.Web.Mvc.HttpDelete]
        public void DeleteFeed(Guid id)
        {
            Feed item = _repository.GetById(id);
            if (item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            string message;
            var result = FeedManager.DeleteFeed(item, out message);

            if (result)
            {
                _repository.Delete(item);
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }
    }
}