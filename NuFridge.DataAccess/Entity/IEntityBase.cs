using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuFridge.DataAccess.Entity
{
    /// <summary>
    /// A non-instantiable base entity which defines 
    /// members available across all entities.
    /// </summary>
    public  interface IEntityBase
    {
        Guid Id { get; set; }
    }
}
