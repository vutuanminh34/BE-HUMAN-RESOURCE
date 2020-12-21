namespace WebAPI.ViewModels
{
    public class CertificateViewModel<T> : BaseViewModel where T : new()
    {
        private T _objModel;
        public CertificateViewModel()
        {
            if (_objModel == null)
                _objModel = new T();
        }
        public T ObjModel
        {
            get { return _objModel; }
            set { _objModel = value; }
        }
    }
}
