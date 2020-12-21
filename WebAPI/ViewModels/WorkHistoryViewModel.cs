using WebAPI.Models;

namespace WebAPI.ViewModels
{
    public class WorkHistoryViewModel : BaseViewModel
    {
        private WorkHistoryInfo _workHistory;
        public WorkHistoryViewModel()
        {
            if (_workHistory == null)
                _workHistory = new WorkHistoryInfo();
        }

        public WorkHistoryInfo WorkHistory
        {
            get { return _workHistory; }
            set { _workHistory = value; }
        } 
    }
}
