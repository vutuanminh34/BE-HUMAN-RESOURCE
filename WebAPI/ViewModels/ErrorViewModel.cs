using System;
using WebAPI.Models;

namespace WebAPI.ViewModels
{
    public class ErrorViewModel : BaseViewModel
    {
        public string RequestId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Time { get; set; }
        public string PathFile { get; set; }
        public string OS { get; set; }
        public string ErrorNumber { get; set; }
        public string ErrorCategory { get; set; }
        public string AppVersion { get; set; }
        public string Description { get; set; }
        public string StackTrace { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
