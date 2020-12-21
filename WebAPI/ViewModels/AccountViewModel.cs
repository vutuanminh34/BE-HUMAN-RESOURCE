using System.Text.Json.Serialization;
using WebAPI.Models;

namespace WebAPI.ViewModels
{
    public class AccountViewModel : BaseViewModel
    {
        private AccountInfo _accountInfo;
        public AccountViewModel() {
            if (_accountInfo == null)
                _accountInfo = new AccountInfo();
            if (Pagging is null) Pagging = new Paggings();
        }

        [JsonIgnore]
        public AccountInfo AccountInfo
        {
            get { return _accountInfo; }
            set { _accountInfo = value; }
        }

        public Paggings Pagging{ get; set; }

        public object ListAccount { get; set; }
    }
}
