using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuFridge.DataAccess.Entity
{
    public class AccountInviteEntity : IEntityBase
    {
        public Guid Id { get; set; }
        public bool IsValid { get; set; }
        public string EmailAddress { get; set; }
    }
}