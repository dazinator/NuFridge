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
            _repository.Insert(item);

            var response = Request.CreateResponse<Feed>(HttpStatusCode.Created, item);

            return response;
        }

        [System.Web.Mvc.HttpPut]
        public void PutFeed(Guid id, Feed feed)
        {
            feed.Id = id;
            if (!_repository.Update(feed))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
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

            _repository.Delete(item);
        }
    }
}