using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using WebAPI.Models.Token;

namespace WebAPI.Models
{
    [DataContract]
    public class AccountInfo
    {
        /// <summary>
        /// USER_NO
        /// </summary>
        [DataMember]
        public int USER_NO { get; set; }

        /// <summary>
        /// USER_CD
        /// </summary>
        [DataMember]
        public string USER_CD { get; set; }

        /// <summary>
        /// USER_NAME
        /// </summary>
        [DataMember]
        public string USER_NAME { get; set; }

        /// <summary>
        /// PASSWORD
        /// </summary>
        [DataMember]
        public string PASSWORD { get; set; }

        /// <summary>
        /// FULL_NAME
        /// </summary>
        [DataMember]
        public string FULL_NAME { get; set; }

        /// <summary>
        /// EMAIL
        /// </summary>
        [DataMember]

        public string EMAIL { get; set; }

        /// <summary>
        /// PHONE
        /// </summary>
        [DataMember]
        public string PHONE { get; set; }

        /// <summary>
        /// ADDRESS
        /// </summary>
        [DataMember]
        public string ADDRESS { get; set; }

        /// <summary>
        /// ROLE
        /// </summary>
        [DataMember]
        public string ROLE { get; set; }

        /// <summary>
        /// STATUS FLG
        /// </summary>
        [DataMember]
        public string STATUS_FLG { get; set; }

        /// <summary>
        /// CREATED AT
        /// </summary>
        public DateTime CREATED_AT { get; set; }

        /// <summary>
        /// CREATED BY
        /// </summary>
        public string CREATED_BY { get; set; }

        /// <summary>
        /// UPDATED AT
        /// </summary>
        public DateTime UPDATED_AT { get; set; }

        /// <summary>
        /// UPDATED BY
        /// </summary>
        public string UPDATED_BY { get; set; }

        /// <summary>
        /// RefreshToken
        /// </summary>
        [JsonIgnore]
        public RefreshToken RefreshToken { get; set; }

        /// <summary>
        /// TOTAL COUNT
        /// </summary>
        [JsonIgnore]
        public int TOTAL_COUNT { get; set; }

    }
}
