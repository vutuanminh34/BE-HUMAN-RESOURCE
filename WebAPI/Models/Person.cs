using System;
using System.Collections.Generic;
using static WebAPI.Constants.Constant;

namespace WebAPI.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string StaffId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public eLocation Location { get; set; }
        public string Avatar { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
        public DateTime YearOfBirth { get; set; }
        public eGender  Gender { get; set; }
        public Boolean Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }

        public IList<Category> Categories { get; set; }

        #region View
        public string GenderName
        {
            get
            {
                string item = null;
                switch (Gender)
                {
                    case Constants.Constant.eGender.Male:
                        item = "Male";
                        break;
                    case Constants.Constant.eGender.Female:
                        item = "Female";
                        break;
                    case Constants.Constant.eGender.Sexless:
                        item = "Sexless";
                        break;
                    default:
                        break;
                }
                return item;
            }
        }


        public string LocationName
        {
            get
            {
                string item = null;
                switch (Location)
                {
                    case Constants.Constant.eLocation.HAN:
                        item = "HAN";
                        break;
                    case Constants.Constant.eLocation.DAD:
                        item = "DAD";
                        break;
                    case Constants.Constant.eLocation.HCM:
                        item = "HCM";
                        break;
                    default:
                        break;
                }
                return item;
            }
        }
        #endregion
    }
}
