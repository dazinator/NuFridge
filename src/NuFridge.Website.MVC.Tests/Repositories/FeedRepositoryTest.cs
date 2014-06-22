using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuFridge.DataAccess.Model;
using NuFridge.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuFridge.Website.MVC.Tests.Repositories
{
    [TestClass]
    public class FeedRepositoryTest
    {
        private Mock<IRepository<Feed>> _repository;
        private List<Feed> feedsInMemory;

        [TestInitialize]
        public void Configure()
        {
            _repository = new Mock<IRepository<Feed>>();

            feedsInMemory = new List<Feed>()
            {
                new Feed() {Id = Guid.NewGuid(), Name = "Test Feed 1"},
                new Feed() {Id = Guid.NewGuid(), Name = "Test Feed 2"},
                new Feed() {Id = Guid.NewGuid(), Name = "Test Feed 3"},
                new Feed() {Id = Guid.NewGuid(), Name = "Test Feed 4"},
                new Feed() {Id = Guid.NewGuid(), Name = "Test Feed 5"},
                new Feed() {Id = Guid.NewGuid(), Name = "Test Feed 6"}
            };
        }

        [TestCategory("Repository")]
        [TestMethod]
        public void AddFeedTest()
        {
            _repository.Setup(rep => rep.Insert(It.IsAny<Feed>())).Callback((Feed fd) => 
                {
                    feedsInMemory.Add(fd);
                });

            var beforeAddCountOfFeeds = feedsInMemory.Count();

            Feed feed = new Feed();
            feed.Id = Guid.NewGuid();
            feed.Name = "Add Feed Test";

            _repository.Object.Insert(feed);

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
            _repository.Setup(rep => rep.GetById(It.IsAny<Guid>())).Returns((Guid i) => feedsInMemory.Single(fd => fd.Id == i));

            var returnedFeed = _repository.Object.GetById(feedsInMemory.First().Id);

            Assert.IsNotNull(returnedFeed);
        }

        [TestCategory("Repository")]
        [TestMethod]
        public void DeleteFeedTest()
        {
            _repository.Setup(rep => rep.GetById(It.IsAny<Guid>())).Returns((Guid i) => feedsInMemory.FirstOrDefault(fd => fd.Id == i));


            _repository.Setup(rep => rep.Delete(It.IsAny<Feed>())).Callback((Feed fd) =>
            {
                feedsInMemory.Remove(fd);
            });

            var returnedFeed = _repository.Object.GetById(feedsInMemory.Last().Id);

            _repository.Object.Delete(returnedFeed);

            var deletedReturnedFeed = _repository.Object.GetById(returnedFeed.Id);

            Assert.IsNull(deletedReturnedFeed);

            _repository.Verify(x => x.Delete(It.IsAny<Feed>()), Times.Once);
        }
    }
}