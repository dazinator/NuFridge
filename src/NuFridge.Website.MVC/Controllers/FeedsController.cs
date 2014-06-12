using NuFridge.Common.Manager;
using NuFridge.DataAccess.Repositories;
using NuFridge.Website.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

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
        public IEnumerable<Feed> GetAllFeeds()
        {
            return _repository.GetAll();
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
            var result = FeedManager.CreateFeed(item.Name, out message);

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

            var currentFeed = _repository.GetById(id);
            if (currentFeed == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            if (newFeed.Name != currentFeed.Name)
            {
                string message;
                var result = FeedManager.UpdateFeed(currentFeed.Name, newFeed.Name, out message);
                if (result)
                {
                    _repository.Update(newFeed);
                }
                else
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
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
            var result = FeedManager.DeleteFeed(item.Name, out message);

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