using ExtendedMongoMembership;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuFridge.DataAccess.Entity
{
    public class AccountEntity : MembershipAccountBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
