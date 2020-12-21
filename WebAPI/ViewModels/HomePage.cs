using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.ViewModels
{
    public class HomePage: BaseViewModel
    {
        private Person _person;
        public HomePage()
        {
            if (_person == null)
                _person = new Person();
        }

        public Person Person
        {
            get { return _person; }
            set { _person = value; }
        }

        public IList<Technology> Technologies { get; set; }

        public IList<Category> Categories { get; set; }
    }
}
