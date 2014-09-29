using FluentValidation;
using NuFridge.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuFridge.Common.Validators
{
    public class FeedValidator : AbstractValidator<Feed>
    {
        public FeedValidator()
        {
            RuleFor(feed => feed.Name).NotEmpty().WithMessage("Feed name is mandatory");
            RuleFor(feed => feed.Name).Matches(@"^[A-Za-z\d\s\-\.]+$").WithMessage("Only alphanumeric characters are allowed in the feed name");
            RuleFor(feed => feed.Name).Length(4, 64);
        }
    }
}
