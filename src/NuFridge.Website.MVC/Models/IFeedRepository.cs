using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuFridge.Website.MVC.Models
{
    public interface IFeedRepository
    {
        IEnumerable<Feed> GetAll();
        Feed Get(string id);
        Feed Add(Feed item);
        void Remove(string id);
        bool Update(Feed item);
    }
}
