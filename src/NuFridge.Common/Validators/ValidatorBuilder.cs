 using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuFridge.Common.Validators
{
   public class ValidatorBuilder
    {
       private Dictionary<Type, object> _validators = new Dictionary<Type, object>();

       public void Add<T>(object param) where T : IValidator, new()
       {
           _validators.Add(typeof(T), param);
       }

       public IEnumerable<ValidationResult> Validate()
       {
           foreach (var item in _validators)
           {
               var validator = Activator.CreateInstance(item.Key) as IValidator;
               var value = item.Value;

               yield return validator.Validate(value);
           }
       }
    }
}