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
        static readonly IFeedRepository repository = new FeedRepository();

                [System.Web.Mvc.HttpGet]
        public IEnumerable<Feed> GetAllFeeds()
        {
            return repository.GetAll();
        }

        [System.Web.Mvc.HttpGet]
        public Feed GetFeed(string id)
        {
            Feed item = repository.Get(id);
            if (item == null)
            {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return item;
        }

        [System.Web.Mvc.HttpPost]
        public HttpResponseMessage PostFeed(Feed item)
        {
            item = repository.Add(item);
            var response = Request.CreateResponse<Feed>(HttpStatusCode.Created, item);

            string uri = Url.Link("DefaultApi", new { id = item.Id });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        [System.Web.Mvc.HttpPut]
        public void PutFeed(string id, Feed feed)
        {
            feed.Id = id;
            if (!repository.Update(feed))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        [System.Web.Mvc.HttpDelete]
        public void DeleteFeed(string id)
        {
            Feed item = repository.Get(id);
            if (item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            repository.Remove(id);
        }
    }
}