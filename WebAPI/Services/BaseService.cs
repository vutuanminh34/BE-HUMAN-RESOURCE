using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Services
{
    public abstract class BaseService<T> where T: new()
    {
        public T model = new T();
        public BaseService() {
            
        }
    }
}
