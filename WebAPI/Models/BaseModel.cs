using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class BaseModel : AppResult
    {
        public int totalCount { get; set; }
    }
}
