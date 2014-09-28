using NuFridge.Common.Manager;
using NuFridge.DataAccess.Model;
using NuFridge.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace NuFridge.Website.MVC.Controllers.API
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
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new HttpError(ex, true));
            }
        }


        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetFeed(Guid id)
        {
            Feed feed;
            try
            {
                feed = _feedManager.GetById(id);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new HttpError(ex, true));
            }

            if (feed == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, feed);
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

        [HttpGet]
        public HttpResponseMessage DownloadPackage(Guid feedId, string packageId, string version)
        {
            var stream = _feedManager.DownloadPackage(feedId, packageId, version);
            if (stream == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "No NuGet package could be found");
            }

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = string.Format("{0}.{1}.nupkg", packageId, version);

            return response;
        }

        [HttpPost]
        public HttpResponseMessage UploadPackage(Guid feedId)
        {

                // Get the uploaded image from the Files collection
                var httpPostedFile = HttpContext.Current.Request.Files["package"];

                if (httpPostedFile != null)
                {
                    // Validate the uploaded image(optional)

                    if (!httpPostedFile.FileName.EndsWith(".nupkg"))
                    {
                        return Request.CreateResponse(HttpStatusCode.Forbidden, "Only NuGet packages can be uploaded. Please check the file type.");
                    }
                    
                    string message;
                    var result = _feedManager.UploadPackage(_feedManager.GetById(feedId), httpPostedFile.ContentLength, httpPostedFile.InputStream, out message);
                    if (!result)
                    {
                    return Request.CreateResponse(HttpStatusCode.Conflict, message);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, message);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "No NuGet package was specified in the request.");
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