using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.ViewModels
{
    public class ProjectViewModel : BaseViewModel
    {
        private Project _project;
        public ProjectViewModel()
        {
            if (_project == null)
                _project = new Project();
        }
        public Project Project 
        {
            get { return _project; }
            set { _project = value; }
        }
    }
}
