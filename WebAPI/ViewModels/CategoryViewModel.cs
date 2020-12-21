using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.ViewModels
{
    public class CategoryViewModel : BaseViewModel
    {
        private Category _category;
        public CategoryViewModel()
        {
            if (this._category == null)
                this._category = new Category();
        }
        public Category Category
        {
            get { return _category; }
            set { _category = value; }
        }
    }

}
