namespace WebAPI.Models
{
    public class AppResult
    {
        /// <summary>
        /// Process result
        /// </summary>
        public bool Result { get; set; } = true;

        /// <summary>
        /// Status code
        /// </summary>
        public string StatusCd { get; set; } = "0";

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Data Result
        /// </summary>
        public object DataResult { get; set; } = null;

    }
}
