using WebAPI.Models;

namespace WebAPI.ViewModels
{
    public abstract class BaseViewModel
    {
        public BaseViewModel() {
            if (this._appResult == null)
                this._appResult = new AppResult();
        }
        private AppResult _appResult;
        public AppResult AppResult
        {
            get { return _appResult; }
            set { _appResult = value; }
        }
    }
}
