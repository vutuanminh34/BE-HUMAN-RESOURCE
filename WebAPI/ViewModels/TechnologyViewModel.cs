using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Models.Resource.Technology;

namespace WebAPI.ViewModels
{
    public class TechnologyViewModel : BaseViewModel
    {

        private TechnologyResource _technology;
        public TechnologyViewModel()
        {
            if (_technology == null)
                this._technology = new TechnologyResource();
        }
        public TechnologyResource Technology
        {
            get { return _technology; }
            set { _technology = value; }
        }
    }
}
