using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace WebAPI.Common
{
    public static class Functions
    {
        public static bool ValidateYearOfBirth(DateTime YearOfBirth)
        {
            if (YearOfBirth == null)
            {
                return false;
            }
            if ((DateTime.Now.Year - YearOfBirth.Year) < 18)
            {
                return false;
            }
            return true;
        }


        public static bool HandlingEmail(string entity_Email)
        {
            string conditionStringEmail = @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|'(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*')@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])";
            Regex regexCheckEmail = new Regex(conditionStringEmail);
            bool resultCheckEmail = regexCheckEmail.IsMatch(entity_Email);
            return resultCheckEmail;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public static bool IsPhoneNumber(string phoneNumber)
        {
            string conditionPhone = @"(\+[0-9]{2}|\+[0-9]{2}\(0\)|\(\+[0-9]{2}\)\(0\)|00[0-9]{2}|0)([0-9]{9}|[0-9\-\s]{9,18})";
            Regex regexCheckPhone = new Regex(conditionPhone);
            bool resultCheckPhone = regexCheckPhone.IsMatch(phoneNumber);
            return resultCheckPhone;
        }

        /// <summary>
        /// Check list exist had any value or type int
        /// </summary>
        /// <param name="myList"></param>
        /// <returns></returns>
        public static bool CheckList(List<int> myList)
        {
            if (myList ==null || myList.GetType()!=typeof(List<int>))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool CheckPageIndex(int myIntValue)
        {
            if (myIntValue.GetType()!=typeof(int)||myIntValue<1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool HasImageExtension(this string source)
        {
            return (source.EndsWith(".png") || source.EndsWith(".jpg") || source.EndsWith(".raw")|| source.EndsWith(".cr2") || source.EndsWith(".nef") || source.EndsWith(".orf") || source.EndsWith(".sr2") || source.EndsWith(".eps") || source.EndsWith(".gif") || source.EndsWith(".bmp") || source.EndsWith(".tif") || source.EndsWith(".tiff") || source.EndsWith(".jpeg"));
        }
        public static bool  CheckLocation (int location)
        {
            try
            {
                if ((location >=0) && (location <=2))
                {
                    
                    return true;
                }
            }
            catch (Exception )
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// Validate Start/End Date for table Education, Certificate
        /// </summary>
        /// <param name="startYear"></param>
        /// <param name="endYear"></param>
        /// <returns></returns>
        public static bool ValidateInputTime(object startYear, object endYear)
        {
            if (startYear is null) return false;

            DateTime startDateTemp = DateTime.Now;
            try
            {
                startDateTemp = Convert.ToDateTime(startYear);
            }
            catch
            {
                // Validate Start/End Date with string type
                if (startYear is string startDate && endYear is string endDate)
                {
                    int yearStartDate = -1;
                    int yearEndDate = -1;
                    try
                    {
                        yearStartDate = int.Parse(startDate);
                        if (!string.IsNullOrEmpty(endDate)) yearEndDate = int.Parse(endDate);
                    }
                    catch
                    {
                        return false;
                    }

                    if (string.IsNullOrEmpty(endDate))
                    {
                        if (yearStartDate > DateTime.Now.Year) return false;
                    }
                    else
                    {
                        if (yearStartDate > DateTime.Now.Year) return false;
                        if (yearStartDate > yearEndDate) return false;
                        if (!Regex.IsMatch(endDate, @"(?<=\s|^)(?=1|2)\d{4}(?=\s|$)")) return false;
                    }
                    return true;
                }
            }

            // Validate Start/End Date with DateTime type
            if (endYear is null)
            {
                if (startDateTemp.Year > DateTime.Now.Year) return false;
            }
            else
            {
                int tempEndDate = -1;
                try
                {
                    tempEndDate = Convert.ToDateTime(endYear).Year;
                }
                catch
                {
                    return false;
                }

                if (startDateTemp.Year > DateTime.Now.Year) return false;
                if (startDateTemp.Year > tempEndDate) return false;
                if (!Regex.IsMatch(tempEndDate.ToString(), @"(?<=\s|^)(?=1|2)\d{4}(?=\s|$)")) return false;
            }
            return true;
        }

        /// <summary>
        /// Convert Year to DateTime type
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime ConvertDateTime(object year)
        {
            // Define DateTime.Year = "1111" is null
            if (year is null) return new DateTime(1111, 01, 01, 01, 01, 01);

            DateTime tempDateTimeNow = DateTime.Now;
            try
            {
                tempDateTimeNow = Convert.ToDateTime(year);
            }
            catch
            {
                try
                {
                    tempDateTimeNow = new DateTime(Convert.ToInt32(year), 01, 01, 01, 01, 01);
                }
                catch
                {
                    tempDateTimeNow = new DateTime(1111, 01, 01, 01, 01, 01);
                }
            }
            return tempDateTimeNow;
        }

        /// <summary>
        /// Check password following this require
        /// </summary>
        /// <param name="password"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        public static bool ValidatePassword(string password, out string ErrorMessage)
        {
            var input = password;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                throw new Exception("Password should not be empty");
            }

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{8,15}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasLowerChar.IsMatch(input))
            {
                ErrorMessage = "Password should contain At least one lower case letter";
                return false;
            }
            else if (!hasUpperChar.IsMatch(input))
            {
                ErrorMessage = "Password should contain At least one upper case letter";
                return false;
            }
            else if (!hasMiniMaxChars.IsMatch(input))
            {
                ErrorMessage = "Password should not be less than or greater than 12 characters";
                return false;
            }
            else if (!hasNumber.IsMatch(input))
            {
                ErrorMessage = "Password should contain At least one numeric value";
                return false;
            }

            else if (!hasSymbols.IsMatch(input))
            {
                ErrorMessage = "Password should contain At least one special case characters";
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// check phone number following vn phone number
        /// </summary>
        /// <param name="Phone"></param>
        /// <returns></returns>
        public static bool ValidatePhone(string Phone)
        {
            try
            {
                if (string.IsNullOrEmpty(Phone))
                    return false;
                var r = new Regex(@"(\+[0-9]{2}|\+[0-9]{2}\(0\)|\(\+[0-9]{2}\)\(0\)|00[0-9]{2}|0)([0-9]{9}|[0-9\-\s]{9,18})");
                return r.IsMatch(Phone);

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// validate role 
        /// </summary>
        /// <param name="Role"></param>
        /// <returns></returns>
        public static bool ValidateRole(string Role)
        {
            switch (Role)
            {
                case "Admin": return true;
                case "Mod": return true;
                case "User": return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Encryption Password
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }
    }
}
