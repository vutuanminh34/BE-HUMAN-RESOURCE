using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.RequestModel;

namespace WebAPI.ViewModels
{
    public class SkillViewModel:BaseViewModel
    {
        private SkillRequestModel _skillRequestModel;
        public SkillViewModel()
        {
            if (this._skillRequestModel == null)
                this._skillRequestModel = new SkillRequestModel();
        }
        public SkillRequestModel SkillRequestModel
        {
            get { return _skillRequestModel; }
            set { _skillRequestModel = value; }
        }
    }
}
