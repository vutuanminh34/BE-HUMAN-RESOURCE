using Newtonsoft.Json;

namespace WebAPI.RequestModel
{
    public class ReCaptchaRequestModel
    {
        public static string Validate(string EncodedResponse)
        {
            using(var client = new System.Net.WebClient())
            {
                string PrivateKey = "6LdB_O0ZAAAAAFRzovItusULndfTCFdBQ9Avv8-B";

                var GoogleReply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", PrivateKey, EncodedResponse));

                var captchaResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ReCaptchaRequestModel>(GoogleReply);

                return captchaResponse.Success.ToLower();
            }
        }

        [JsonProperty("success")]
        public string Success
        {
            get { return m_Success; }
            set { m_Success = value; }
        }

        private string m_Success;
    }
}
