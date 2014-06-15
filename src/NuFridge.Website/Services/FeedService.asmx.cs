using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Services;
using System.Web.Services;

using NuFridge.DataAccess.Entity;
using NuFridge.Common.Manager;
using NuFridge.DataAccess.Entity.Feeds;
using NuFridge.DataAccess.Repositories;
using MongoDB.Driver;
using NuFridge.Website.Services.Messages;

namespace NuFridge.Website.Services
{
    [WebService(Namespace = "http://tempuri.org/feedservice")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class FeedService : WebService
    {
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public GetFeedResponse GetFeed(GetFeedRequest request)
        {
            FeedEntity feed = null;
            try
            {
                //feed = FeedManager.FindFeed(request.FeedName);
            }
            catch (Exception)
            {
                return new GetFeedResponse(false, null);
            }

            return new GetFeedResponse(feed != null, feed);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public GetRetentionPolicyResponse GetRetentionPolicy(GetRetentionPolicyRequest request)
        {
            RetentionPolicyEntity retentionPolicy;
            try
            {
                retentionPolicy = RetentionPolicyManager.FindRetentionPolicy(request.FeedName);

            }
            catch (Exception)
            {
                return new GetRetentionPolicyResponse(false, null);
            }

            return new GetRetentionPolicyResponse(true, retentionPolicy);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public SaveRetentionPolicyResponse SaveRetentionPolicy(SaveRetentionPolicyRequest request)
        {
            RetentionPolicyEntity retentionPolicy;
            try
            {
                bool createNew = false;
                retentionPolicy = RetentionPolicyManager.FindRetentionPolicy(request.FeedName);
                if (retentionPolicy == null)
                {
                    createNew = true;
                    retentionPolicy = new RetentionPolicyEntity();
                    retentionPolicy.FeedName = request.FeedName;
                }

                retentionPolicy.Enabled = request.Enabled;
                retentionPolicy.DaysToKeepPackages = request.DaysToKeepPackages;
                retentionPolicy.ExcludedPackages = request.ExcludedPackages;
                retentionPolicy.VersionsToKeep = request.VersionsToKeep;

                if (createNew)
                {
                    RetentionPolicyManager.InsertRetentionPolicy(retentionPolicy);
                }
                else
                {
                    RetentionPolicyManager.UpdateRetentionPolicy(retentionPolicy);
                }
            }
            catch (Exception)
            {
                return new SaveRetentionPolicyResponse(false);
            }

            return new SaveRetentionPolicyResponse(true);
        }

        [WebMethod(MessageName = "GetFeedNames")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public GetFeedsResponse GetFeeds()
        {
            var feeds = new List<string>();
            try
            {
                MongoDbRepository<FeedEntity> repository = new MongoDbRepository<FeedEntity>();
                feeds = repository.GetAll().Select(fd => fd.Name).ToList();
            }
            catch (MongoConnectionException)
            {
                return new GetFeedsResponse(false, null, "Failed to connect to the MongoDB server. Please check it is running on the server.");
            }

            return new GetFeedsResponse(true, feeds);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public CreateFeedResponse CreateFeed(CreateFeedRequest request)
        {

            string message = string.Empty;
          //  var success = FeedManager.CreateFeed(request.FeedName, out message);
            return new CreateFeedResponse(false, message);
        }

        [WebMethod(MessageName = "GetRetentionPolicyHistoryList")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public GetRetentionPolicyHistoryResponse GetRetentionPolicyHistory()
        {
            var history = RetentionPolicyManager.GetRetentionPolicyHistory();
            return new GetRetentionPolicyHistoryResponse(history);
        }

        [WebMethod(MessageName = "GetRetentionPolicyHistoryListLog")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetRetentionPolicyHistoryLog(Guid id)
        {
            return RetentionPolicyManager.GetRetentionPolicyLog(id);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public DeleteFeedResponse DeleteFeed(DeleteFeedRequest request)
        {
            string message;
           // var success = FeedManager.DeleteFeed(request.FeedName, out message);
            return new DeleteFeedResponse(true, "");
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public ImportFeedResponse ImportFeed(ImportFeedRequest request)
        {
            string jobName = "";
           // FeedManager.ScheduleImportPackagesJob(request.SourceFeedUrl, request.FeedName, request.ApiKey, out jobName);
            return new ImportFeedResponse() { Success = true, JobName = jobName };
        }
    }
}