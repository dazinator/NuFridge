using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuFridge.Website.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuFridge.Website.MVC.Tests.Controllers
{
    [TestClass]
    public class FeedsControllerTest
    {
        private Mock<IFeedRepository> _repository;
        private List<Feed> feedsInMemory;

        [TestInitialize]
        public void Configure()
        {
            _repository = new Mock<IFeedRepository>();

            feedsInMemory = new List<Feed>()
            {
                new Feed() {Id = Guid.NewGuid().ToString(), Name = "Test Feed 1"},
                new Feed() {Id = Guid.NewGuid().ToString(), Name = "Test Feed 2"},
                new Feed() {Id = Guid.NewGuid().ToString(), Name = "Test Feed 3"},
                new Feed() {Id = Guid.NewGuid().ToString(), Name = "Test Feed 4"},
                new Feed() {Id = Guid.NewGuid().ToString(), Name = "Test Feed 5"},
                new Feed() {Id = Guid.NewGuid().ToString(), Name = "Test Feed 6"}
            };
        }

        [TestCategory("Repository")]
        [TestMethod]
        public void AddFeedTest()
        {
            _repository.Setup(rep => rep.Add(It.IsAny<Feed>())).Callback((Feed fd) => 
                {
                    feedsInMemory.Add(fd);
                });

            var beforeAddCountOfFeeds = feedsInMemory.Count();

            Feed feed = new Feed();
            feed.Id = Guid.NewGuid().ToString();
            feed.Name = "Add Feed Test";

            _repository.Object.Add(feed);

            var afterAddCountOfFeeds = feedsInMemory.Count();

            Assert.AreNotEqual(beforeAddCountOfFeeds, afterAddCountOfFeeds);

            Assert.AreEqual(feed, feedsInMemory.Last());
        }

        [TestCategory("Repository")]
        [TestMethod]
        public void GetAllFeedsTest()
        {
            _repository.Setup(rep => rep.GetAll()).Returns(feedsInMemory);

            var returnedFeeds = _repository.Object.GetAll();

            Assert.AreEqual(feedsInMemory.Count(), returnedFeeds.Count());
        }

        [TestCategory("Repository")]
        [TestMethod]
        public void GetFeedTest()
        {
            _repository.Setup(rep => rep.Get(It.IsAny<string>())).Returns((string i) => feedsInMemory.Single(fd => fd.Id == i));

            var returnedFeed = _repository.Object.Get(feedsInMemory.First().Id);

            Assert.IsNotNull(returnedFeed);
        }

        [TestCategory("Repository")]
        [TestMethod]
        public void DeleteFeedTest()
        {
            _repository.Setup(rep => rep.Get(It.IsAny<string>())).Returns((string i) => feedsInMemory.FirstOrDefault(fd => fd.Id == i));


            _repository.Setup(rep => rep.Remove(It.IsAny<string>())).Callback((string id) =>
            {
                var feed = feedsInMemory.Single(fd => fd.Id == id);
                feedsInMemory.Remove(feed);
            });

            var returnedFeed = _repository.Object.Get(feedsInMemory.Last().Id);

            _repository.Object.Remove(returnedFeed.Id);

            var deletedReturnedFeed = _repository.Object.Get(returnedFeed.Id);

            Assert.IsNull(deletedReturnedFeed);

            _repository.Verify(x => x.Remove(It.IsAny<string>()), Times.Once);
        }
    }
}