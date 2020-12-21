namespace WebAPI.Constants
{
    public class Constant
    {
        public static string CONNECT_ERROR = "Please specify the correct connection string.";

        public static string INFO_PERSON_BY_ID = "Info person.";

        public static string UPDATE_SUCCESS = "Update Successful!";

        public static string INSERT_SUCCESS = "Insert Successful!";

        public static string DELETE_SUCCESS = "Delete Successful!";

        public static string ACCOUNT_ERROR = "Account not exist!";

        public static string PASSWORD_ERROR = "Password should not be empty";

        public static string USERNAME_ERROR = "Invalid User Name!";

        public static string FULLNAME_ERROR = "Invalid Full Name!";

        public static string EMAIL_ERROR = "Invalid Email!";

        public static string PHONE_ERROR = "Invalid Phone number!";

        public static string ADDRESS_ERROR = "Invalid Address!";

        public static string ROLE_ERROR = "Invalid Role!";

        public static string SWAP_ERROR = "CurrentId or TurnedId not exist!";

        public static string SWAP_SUCCESS = "Swap OrderIndex Successful!";

        public static string ID_ERROR = "Id not exist!";

        public static string PROJECT_ERROR = "Project not exist!";

        public static string PERSONID_ERROR = "PersonId not exist!";

        public static string TEAMSIZE_ERROR = "TeamSize value need to be an integer greater than 0!";

        public static string NAME_ERROR = "Invalid Name!";

        public static string DESCRIPTION_ERROR = "Invalid Description!";

        public static string POSITION_ERROR = "Invalid Position!";

        public static string RESPONSIBILITIES_ERROR = "Invalid Responsibilities!";

        public static string DATETIME_ERROR = "Invalid StartDate and EndDate!";

        public static string TECHNOLOGY_ERROR = "Technology not exist!";

        public static string CHECKTECHNOLOGY_ERROR = "This technology does not exist in Person!";

        public static string IPAGENUM_ERROR = "Invalid iPaggeNum!";

        public static string COMPANY_ERROR = "Invalid CompanyName";

        public static string WORKHISTORY_ERROR = "WorkHistory not exist!";

        public static string DELETE_FAIL = "Delete Fail";

        public static string RESOURCE_ERROR = "Resource not found";

        public static string GET_SUCCESS = "Get data success";

        public static string INFO_PERSONS = "These are info persons you are looking for";

        public static string ALL_PROFILES="All profile person";

        public static string CREATE_PERSON_SUCCESS = "Successfully created! The request has succeeded and a new person has been created as a result.!";

        public static string UPDATE_PERSON_SUCCESS = "Successfully updated! The request has succeeded and a new person has been updated as a result.!";

        public static string IMAGE_INVALID = "Image invalid!";

        public static string PAGINATION = "You need insert page and limit value ";

        public static string PAGE_INVALID = "You page value invalid";

        public static string LIMIT_INVALID = "The limit value invalid";

        public static string YEAROFBIRTH_INVALID = "The year of birth value invalid";

        public static string GENDER_INVALID = "The gender value invalid";

        public static string LOCATION_INVALID = "The location value invalid";

        public static string ALL_PARAMETER_INVALID="All parameter is null";

        public enum eLocation : byte
        {
            HAN,
            DAD,
            HCM
        }
        public enum eGender : byte
        {
            Male,
            Female,
            Sexless
        }
    }
    
}
