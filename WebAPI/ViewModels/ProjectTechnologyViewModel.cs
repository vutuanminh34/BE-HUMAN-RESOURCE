using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.ViewModels
{
    public class ProjectTechnologyViewModel : BaseViewModel
    {
        private ProjectTechnology _projectTechnology;

        public ProjectTechnologyViewModel()
        {
            if (_projectTechnology == null)
                _projectTechnology = new ProjectTechnology();
        }

        public ProjectTechnology ProjectTechnology
        {
            get { return _projectTechnology; }
            set { _projectTechnology = value; }
        }
    }
}
