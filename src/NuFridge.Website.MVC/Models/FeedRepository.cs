using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuFridge.Website.MVC.Models
{
    public class FeedRepository : IFeedRepository
    {
        private List<Feed> feeds = new List<Feed>();
        private int _nextId = 1;

        public FeedRepository()
        {
            Add(new Feed { Name = "Test Feed 1" });
            Add(new Feed { Name = "Test Feed 2" });
            Add(new Feed { Name = "Test Feed 3" });
        }

        public IEnumerable<Feed> GetAll()
        {
            return feeds;
        }

        public Feed Get(string id)
        {
            return feeds.Find(p => p.Id == id);
        }

        public Feed Add(Feed item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            feeds.Add(item);
            return item;
        }

        public void Remove(string id)
        {
            feeds.RemoveAll(p => p.Id == id);
        }

        public bool Update(Feed item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            int index = feeds.FindIndex(p => p.Id == item.Id);
            if (index == -1)
            {
                return false;
            }
            feeds.RemoveAt(index);
            feeds.Add(item);
            return true;
        }
    }
}