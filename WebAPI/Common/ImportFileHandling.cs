using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebAPI.Common;
using WebAPI.Models;
using WebAPI.Models.Resource.Certificate;
using WebAPI.Models.Resource.Education;
using WebAPI.Models.Resource.WorkHistory;
using static WebAPI.Constants.Constant;

namespace WebAPI.HandlingFiles
{
    public static class ImportFileHandling
    {
        public static bool HasFileExtension(this string source)
        {
            return (source.EndsWith(".xlsx") || source.EndsWith(".xlsm") || source.EndsWith(".xltx") || source.EndsWith(".xltm") || source.EndsWith(".xlam"));
        }
        public static async Task<List<String>> GetInformationCV(IFormFile file)
        {
            List<string> list = new List<string>();
            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (ExcelPackage package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
                        if (worksheet.Dimension != null)
                        {
                            int totalRows = worksheet.Dimension.Rows;
                            int totalColums = worksheet.Dimension.Columns;
                            for (int row = 1; row <= 60; row++)
                            {
                                for (int col = 1; col <= totalRows; col++)
                                {
                                    if (!string.IsNullOrEmpty(worksheet.Cells[row, col].Value?.ToString().Trim()))
                                    {
                                        list.Add(worksheet.Cells[row, col].Value?.ToString().Trim());
                                    }
                                }
                            }
                        }
                    }
                    return list;
                }
            }
            catch { return list; }
        }

        /// <summary>
        /// Get Image Avatar of Person
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static async Task<Image> GetImageCV(IFormFile file)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (ExcelPackage package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
                        var pic = worksheet.Drawings[1] as ExcelPicture;
                        Image image = pic.Image;
                        var name = image.FrameDimensionsList[0].ToString() + "-" + image.VerticalResolution.ToString().Replace('.', '-') + "." + image.RawFormat.ToString().ToLower();
                        image.Tag = name;
                        return image;
                    }
                }
            }
            catch
            {
                return null;
            }

        }

        #region Handling Person
        /// <summary>
        /// Get Person
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Person GetPerson(List<string> list)
        {
            int indexOffice = list.IndexOf("Office");
            int indexPhone = list.IndexOf("Phone");
            int indexYear = list.IndexOf("Year of Birth");
            int indexGender = list.IndexOf("Gender");
            int indexEmail = list.IndexOf("Email");
            int indexOverView = list.IndexOf("PROFESSIONAL  OVERVIEW");
            string office = (!list[indexOffice + 1].Equals("Phone")) ? list[indexOffice + 1] : null;
            string year = (!list[indexYear + 1].Equals("Gender")) ? list[indexYear + 1] : null;
            string gender = (!list[indexGender + 1].Equals("Email")) ? list[indexGender + 1] : null;
            Hashtable hashtable = FormatYear(year);

            Person person = new Person
            {
                StaffId = "",
                FullName = (!list[1].Equals("Office")) ? list[1] : null,
                Email = (!list[indexEmail + 1].Equals("PROFESSIONAL  OVERVIEW")) ? list[indexEmail + 1] : null,
                Location = (eLocation)FormatLocation(office),
                Avatar = "",
                Description = (!list[indexOverView + 1].Equals("SKILLS")) ? list[indexOverView + 1] : null,
                Phone = (!list[indexPhone + 1].Equals("Year of Birth")) ? list[indexPhone + 1] : null,
                YearOfBirth = (hashtable["StartDate"] != null) ? (DateTime)hashtable["StartDate"] : DateTime.MinValue,
                Gender = (eGender)FormatGender(gender),
                CreatedBy = WebAPI.Helpers.HttpContext.CurrentUser,
                UpdatedBy = WebAPI.Helpers.HttpContext.CurrentUser,
                Status = true
            };
            return person;
        }

        /// <summary>
        /// Format Gender
        /// </summary>
        /// <param name="genderStr"></param>
        /// <returns></returns>
        public static int FormatGender(string genderStr)
        {
            int gerder = 2;
            if (!String.IsNullOrEmpty(genderStr))
            {
                switch (genderStr)
                {
                    case "Male":
                        gerder = 0;
                        break;
                    case "Female":
                        gerder = 1;
                        break;
                    case "Sexless":
                        gerder = 2;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                gerder = 255;
            }
            return gerder;
        }

        /// <summary>
        /// Format Gender
        /// </summary>
        /// <param name="genderStr"></param>
        /// <returns></returns>
        public static int FormatLocation(string locationStr)
        {
            int location = 255;
            if (!String.IsNullOrEmpty(locationStr))
            {
                switch (locationStr.Trim())
                {
                    case "HAN":
                        location = 0;
                        break;
                    case "DAD":
                        location = 1;
                        break;
                    case "HCM":
                        location = 2;
                        break;
                    default:
                        break;
                }
            }
            return location;
        }

        #endregion
        #region Handling Skill
        /// <summary>
        /// Get List Category
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<Category> GetListCategory(List<string> list)
        {
            List<Category> listCategory = new List<Category>();
            int indexSkill = list.IndexOf("SKILLS");
            int indexWorkHistory = list.IndexOf("WORKING HISTORY");
            for (int i = indexSkill; i < indexWorkHistory; i++)
            {
                if ((i - (indexSkill + 1)) % 2 == 0)
                {
                    int index = -1;
                    for (int j = 0; j < listCategory.Count; j++)
                    {
                        if (listCategory[j].Name.Equals(list[i]))
                            index = j;
                    }
                    if (index > -1)
                    {
                        List<string> listTechnology = SplitTechnology(list[i + 1]);
                        foreach (var item in listTechnology)
                        {
                            Technology technology = new Technology
                            {
                                Name = item
                            };
                            listCategory[index].Technologies.Add(technology);
                        }
                    }
                    else
                    {
                        Category category = new Category
                        {
                            Name = list[i],
                            Technologies = new List<Technology>(),
                        };
                        List<string> listTechnology = SplitTechnology(list[i + 1]);
                        foreach (var item in listTechnology)
                        {
                            Technology technology = new Technology
                            {
                                Name = item
                            };
                            category.Technologies.Add(technology);
                        }
                        listCategory.Add(category);
                    }
                }
            }

            return listCategory;

        }
        /// <summary>
        /// Get List Technology
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<Technology> GetListTechnology(List<string> list)
        {
            List<Technology> listTechnology = new List<Technology>();
            int indexSkill = list.IndexOf("SKILLS");
            int indexWorkHistory = list.IndexOf("WORKING HISTORY");
            for (int i = indexSkill; i < indexWorkHistory; i++)
            {
                if (((i - (indexSkill)) % 2 == 0) && i > indexSkill)
                {
                    var listItemTechnology = SplitTechnology(list[i].Trim());
                    if (listItemTechnology != null)
                    {
                        foreach (var item in listItemTechnology)
                        {
                            Technology technology = new Technology
                            {
                                Name = item
                            };
                            listTechnology.Add(technology);
                        }
                    }

                }
            }

            return listTechnology;
        }
        /// <summary>
        /// Split Technology
        /// </summary>
        /// <param name="technoloryStr"></param>
        /// <returns></returns>
        public static List<string> SplitTechnology(string technoloryStr)
        {
            List<string> listTechnology = new List<string>();
            if (!String.IsNullOrEmpty(technoloryStr))
            {
                if (technoloryStr.Contains(","))
                {
                    String[] arrListStr = technoloryStr.Split(',');
                    for (int i = 0; i < arrListStr.Length; i++)
                    {
                        listTechnology.Add(arrListStr[i]);
                    }
                }
                else
                {
                    listTechnology.Add(technoloryStr.Trim());
                }
            }
            return listTechnology;
        }
        #endregion
        #region Handling WorkHistory
        /// <summary>
        /// Get List WorkHistory
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<CreateWorkHistoryResource> GetListWorkHistory(List<string> list, int id)
        {
            List<CreateWorkHistoryResource> listWorkHistory = new List<CreateWorkHistoryResource>();
            int indexWorkHistory = list.IndexOf("WORKING HISTORY");
            int indexEducation = list.IndexOf("EDUCATION");
            for (int i = indexWorkHistory + 4; i < indexEducation - 4; i = i + 4)
            {
                Hashtable hashtable = FormatYearAndMonth(list[i + 2]);
                CreateWorkHistoryResource workHistoryInfo = new CreateWorkHistoryResource
                {
                    StartDate = hashtable["StartDate"],
                    EndDate = hashtable["EndDate"],
                    CompanyName = list[i + 3],
                    Position = list[i + 4],
                    PersonId = id
                };
                listWorkHistory.Add(workHistoryInfo);
            }
            return listWorkHistory;


        }
        #endregion
        #region Handling Education
        /// <summary>
        /// Get List Education
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<CreateEducationResource> GetListEducation(List<string> list, int id)
        {
            List<CreateEducationResource> listEducation = new List<CreateEducationResource>();
            int indexEducation = list.IndexOf("EDUCATION");
            int indexCertificate = list.IndexOf("CERTIFICATION");
            int index = 0;
            for (int i = indexEducation; i < indexCertificate - 2; i = i + 2)
            {
                index++;
                CreateEducationResource educationInfo = new CreateEducationResource();
                string dateEducation = GetDate(list[i + 1]);
                Hashtable hashtable = FormatYear(dateEducation);
                educationInfo = new CreateEducationResource
                {
                    StartDate = hashtable["StartDate"],
                    EndDate = (hashtable.Count > 1) ? hashtable["EndDate"] : null,
                    CollegeName = GetCollegeName(list[i + 1]),
                    Major = GetInformation(list[i + 2]),
                    PersonId = id
                };
                listEducation.Add(educationInfo);
            }
            return listEducation;

        }

        /// <summary>
        /// Get ColleName
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetCollegeName(string str)
        {
            string result = null;
            if (!String.IsNullOrEmpty(str))
            {
                if (str.Contains("|"))
                {
                    string[] arrStr = str.Split("|");
                    if (arrStr != null && arrStr.Length > 1)
                        result = arrStr[1].Trim();
                }
            }
            return result;
        }
        #endregion
        #region Handling Certification
        /// <summary>
        /// Get List Certifiate
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<CreateCertificateResource> GetListCertifiate(List<string> list, int id)
        {
            List<CreateCertificateResource> listCertificate = new List<CreateCertificateResource>();
            int indexCertificate = list.IndexOf("CERTIFICATION");
            int indexProject = list.IndexOf("PROJECTS");
            int index = 0;
            for (int i = indexCertificate + 1; i < indexProject; i++)
            {
                index++;
                CreateCertificateResource certificateInfo = new CreateCertificateResource();
                string certificateStr = list[i];
                string dateCertificate = GetDate(certificateStr);
                Hashtable hashtable = FormatYear(dateCertificate);
                certificateInfo = new CreateCertificateResource
                {
                    StartDate = (DateTime)hashtable["StartDate"],
                    EndDate = (hashtable.Count > 1) ? (DateTime?)hashtable["EndDate"] : null,
                    Name = (GetNameAndProviderCertifiate(certificateStr)["Name"] != null) ? GetNameAndProviderCertifiate(certificateStr)["Name"].ToString() : null,
                    Provider = (GetNameAndProviderCertifiate(certificateStr)["Provider"] != null) ? GetNameAndProviderCertifiate(certificateStr)["Provider"].ToString() : null,
                    PersonId = id
                };
                listCertificate.Add(certificateInfo);

            }
            return listCertificate;

        }
        /// <summary>
        /// Get Name And Provider Certifiate
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Hashtable GetNameAndProviderCertifiate(string str)
        {
            Hashtable hashtable = new Hashtable();
            hashtable.Add("Name", null);
            hashtable.Add("Provider", null);
            if (!String.IsNullOrEmpty(str))
            {
                if (str.Contains("|"))
                {
                    if (str.Contains("-"))
                    {
                        string[] arrStr = str.Trim().Split("|");
                        if (arrStr.Any() && arrStr.Length > 1)
                        {
                            string[] arrStrNameAndProvider = arrStr[1].Trim().Split("-");
                            if (arrStrNameAndProvider.Any() && arrStrNameAndProvider.Length > 1)
                            {
                                hashtable["Name"] = arrStrNameAndProvider[0];
                                hashtable["Provider"] = arrStrNameAndProvider[1];
                            }
                        }
                    }
                    else
                    {
                        hashtable["Name"] = str;
                    }
                }
            }
            return hashtable;

        }

        #endregion
        #region Handling Project
        /// <summary>
        /// Get List Project
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<Project> GetListProject(List<string> list, int id)
        {
            int indexProject = list.IndexOf("PROJECTS");
            int indexEnd = list.Count();
            List<Project> listProject = new List<Project>();
            for (int i = indexProject + 4; i < indexEnd - 8; i = i + 8)
            {
                string str = list[i + 2];
                Hashtable hashDate = FormatYearAndMonth(list[i + 2]);
                if (hashDate is null || hashDate.Count <= 0)
                    return null;
                else
                {
                    Project project = new Project
                    {
                        PersonId = id,
                        OrderIndex = (String.IsNullOrEmpty(list[i + 1])) ? 0 : Convert.ToInt32(list[i + 1]),
                        StartDate = (DateTime)hashDate["StartDate"],
                        EndDate = (DateTime)hashDate["EndDate"],
                        Position = list[i + 3],
                        Name = list[i + 4],
                        Description = GetInformation(list[i + 5]),
                        Responsibilities = GetInformation(list[i + 6]),
                        TeamSize = (String.IsNullOrEmpty(GetInformation((list[i + 7])))) ? 1 : ConvertMonth(GetInformation(list[i + 7])),
                        Technologies = new List<Technology>()
                    };
                    List<string> listTechnology = SplitTechnology(GetInformation(list[i + 8]));
                    foreach (var item in listTechnology)
                    {
                        Technology technology = new Technology
                        {
                            Name = item
                        };
                        project.Technologies.Add(technology);
                    }
                    listProject.Add(project);
                }

            }
            return listProject;

        }
        #endregion
        #region Handling Date
        /// <summary>
        /// Get Date
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetDate(string str)
        {
            string result = null;
            if (!String.IsNullOrEmpty(str))
            {
                if (str.Contains("|"))
                {
                    string[] arrStr = str.Split("|");
                    if (arrStr != null && arrStr.Length > 1)
                        result = arrStr[0].Trim();
                }
            }
            return result;

        }
        /// <summary>
        /// Get Month
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetMonth(string str)
        {
            int result = -1;
            switch (str)
            {
                case "Jan":
                    result = 1;
                    break;
                case "Feb":
                    result = 2;
                    break;
                case "Mar":
                    result = 3;
                    break;
                case "Apr":
                    result = 4;
                    break;
                case "May":
                    result = 5;
                    break;
                case "Jun":
                    result = 6;
                    break;
                case "Jul":
                    result = 7;
                    break;
                case "Aug":
                    result = 8;
                    break;
                case "Sep":
                    result = 9;
                    break;
                case "Oct":
                    result = 10;
                    break;
                case "Nov":
                    result = 11;
                    break;
                case "Dec":
                    result = 12;
                    break;
                default:
                    break;
            }
            return result;
        }

        public static int ConvertMonth(string str)
        {
            var result = -1;
            try
            {
                result = Convert.ToInt32(str.Trim());
            }
            catch
            {
                result = -1;
            }
            return result;
        }

        /// <summary>
        /// FormatYearAndMonth
        /// </summary>
        /// <param name="dateStr"></param>
        /// <returns></returns>
        public static Hashtable FormatYearAndMonth(string dateStr)
        {
            Hashtable hashtable = new Hashtable();
            hashtable.Add("StartDate", DateTime.MinValue);
            hashtable.Add("EndDate", DateTime.MinValue);
            if (!String.IsNullOrEmpty(dateStr))
            {
                string[] arrListStr = dateStr.Trim().Split("-");
                if (arrListStr.Count() > 1)
                {
                    if (!String.IsNullOrEmpty(arrListStr[0]) && !String.IsNullOrEmpty(arrListStr[1]))
                    {
                        if (arrListStr[0].Contains("Now"))
                            hashtable["EndDate"] = DateTime.Now;
                        else
                        {
                            string[] arrListEndtDate = arrListStr[0].Trim().Split(" ");
                            if (arrListEndtDate.Count() > 1)
                            {
                                int month = (ConvertMonth(arrListEndtDate[0]) == -1) ? GetMonth(arrListEndtDate[0]) : ConvertMonth(arrListEndtDate[0]);
                                if (month == -1)
                                    hashtable["EndDate"] = DateTime.MinValue;
                                else
                                    hashtable["EndDate"] = (!Regex.IsMatch(arrListEndtDate[1], "[a-z]") || String.IsNullOrEmpty(arrListEndtDate[1])) ? new DateTime(Convert.ToInt32(arrListEndtDate[1]), month, 01) : DateTime.MinValue;
                            }
                        }
                        string[] arrListStarttDate = arrListStr[1].Trim().Split(" ");
                        if (arrListStarttDate.Count() > 1)
                        {
                            int month = (ConvertMonth(arrListStarttDate[0]) == -1) ? GetMonth(arrListStarttDate[0]) : ConvertMonth(arrListStarttDate[0]);
                            if (month == -1)
                                hashtable["StartDate"] = DateTime.MinValue;
                            else
                                hashtable["StartDate"] = (!Regex.IsMatch(arrListStarttDate[1], "[a-z]") || String.IsNullOrEmpty(arrListStarttDate[1])) ? new DateTime(Convert.ToInt32(arrListStarttDate[1]), month, 01) : DateTime.MinValue;
                        }
                    }

                }
                else
                {
                    string[] arrListStarttDate = dateStr.Trim().Split(" ");
                    if (arrListStarttDate.Count() > 1)
                    {
                        int month = (ConvertMonth(arrListStarttDate[0]) == -1) ? GetMonth(arrListStarttDate[0]) : ConvertMonth(arrListStarttDate[0]);
                        if (month == -1)
                            hashtable["StartDate"] = DateTime.MinValue;
                        else
                            hashtable["StartDate"] = (!Regex.IsMatch(arrListStarttDate[1], "[a-z]") || String.IsNullOrEmpty(arrListStarttDate[1])) ? new DateTime(Convert.ToInt32(arrListStarttDate[1]), month, 01) : DateTime.MinValue;
                    }
                    hashtable["EndDate"] = null;
                }
            }
            return hashtable;
        }

        /// <summary>
        /// Format Year
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Hashtable FormatYear(string str)
        {
            Hashtable hashtable = new Hashtable();
            if (!String.IsNullOrEmpty(str))
            {
                if (str.Contains("-"))
                {
                    string[] arrStr = str.Trim().Split("-");
                    if (arrStr.Count() > 1)
                    {
                        DateTime endDate = (!Regex.IsMatch(arrStr[0], "[a-z]") || String.IsNullOrEmpty(arrStr[0])) ? new DateTime(Convert.ToInt32(arrStr[0]), 01, 01) : DateTime.MinValue;
                        hashtable.Add("EndDate", endDate);
                        DateTime startDate = (!Regex.IsMatch(arrStr[1], "[a-z]") || String.IsNullOrEmpty(arrStr[1])) ? new DateTime(Convert.ToInt32(arrStr[1]), 01, 01) : DateTime.MinValue;
                        hashtable.Add("StartDate", startDate);
                    }
                    else
                    {
                        DateTime startDate = (!Regex.IsMatch(arrStr[0], "[a-z]") || String.IsNullOrEmpty(arrStr[0])) ? new DateTime(Convert.ToInt32(arrStr[0]), 01, 01) : DateTime.MinValue;
                        hashtable.Add("StartDate", startDate);
                        hashtable.Add("EndDate", null);
                    }
                }
                else
                {
                    DateTime dateTime = (!Regex.IsMatch(str, "[a-z]")) ? new DateTime(Convert.ToInt32(str), 01, 01) : DateTime.MinValue;
                    hashtable.Add("StartDate", dateTime);
                }
            }
            else
            {
                hashtable.Add("StartDate", DateTime.MinValue);
            }
            return hashtable;

        }
        #endregion
        #region Handling Information
        /// <summary>
        /// Get Infomation
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetInformation(string str)
        {
            string result = null;
            try
            {
                if (!String.IsNullOrEmpty(str))
                {
                    if (str.Contains(":"))
                    {
                        string[] arrStr = str.Trim().Split(":");
                        if (arrStr.Length > 1)
                            result = (String.IsNullOrEmpty(arrStr[1])) ? null : arrStr[1].Trim();
                    }
                }
                return result;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        public static AppResult CheckFile(List<string> list)
        {
            AppResult appResult = new AppResult();
            appResult.Message = "CV Import Failed, ";
            int indexSkill = list.IndexOf("SKILLS");
            int indexWorkHistory = list.IndexOf("WORKING HISTORY");
            int indexEducation = list.IndexOf("EDUCATION");
            int indexCertificate = list.IndexOf("CERTIFICATION");
            int indexProject = list.IndexOf("PROJECTS");
            int indexEnd = list.Count();
            if ((indexWorkHistory - indexSkill) > 1 && (indexWorkHistory - indexSkill - 1) % 2 != 0)
            {
                appResult.Result = false;
                appResult.Message += $"You must not let a blank cell in Skill Categories empty!";
                return appResult;
            }
            if ((indexEducation - indexWorkHistory) > 1 && (indexEducation - indexWorkHistory - 1) % 4 != 0)
            {
                appResult.Result = false;
                appResult.Message += $"You must not let a blank cell in WorkHistory empty!";
                return appResult;
            }
            if ((indexCertificate - indexEducation) > 1 && (indexCertificate - indexEducation - 1) % 2 != 0)
            {
                appResult.Result = false;
                appResult.Message += $"You must not let a blank cell in Certificate empty!";
                return appResult;
            }
            if ((indexEnd - indexProject) > 1 && (indexEnd - indexProject - 5) % 8 != 0)
            {
                appResult.Result = false;
                appResult.Message += $"You must not let a blank cell in Project empty!";
                return appResult;
            }
            var person = GetPerson(list);
            if (person != null)
            {
                if (String.IsNullOrEmpty(person.FullName))
                {
                    appResult.Result = false;
                    appResult.Message += Constants.Constant.FULLNAME_ERROR;
                    return appResult;
                }
                if (String.IsNullOrEmpty(person.LocationName))
                {
                    appResult.Result = false;
                    appResult.Message += "Invalid Office!";
                    return appResult;
                }
                if (!Functions.IsPhoneNumber(person.Phone))
                {
                    appResult.Result = false;
                    appResult.Message += Constants.Constant.PHONE_ERROR;
                    return appResult;
                }
                if (person.YearOfBirth.Year == 1)
                {
                    appResult.Result = false;
                    appResult.Message += "Invalid Year Of Birth!";
                    return appResult;
                }
                if (!Functions.HandlingEmail(person.Email))
                {
                    appResult.Result = false;
                    appResult.Message += Constants.Constant.EMAIL_ERROR;
                    return appResult;
                }
                if (String.IsNullOrEmpty(person.Description))
                {
                    appResult.Result = false;
                    appResult.Message += "Invalid Description!";
                    return appResult;
                }

            }
            List<CreateEducationResource> createEducationResources = ImportFileHandling.GetListEducation(list, 1);
            if (createEducationResources.Any())
            {
                int i = 0;
                foreach (var item in createEducationResources)
                {
                    i++;
                    if (String.IsNullOrEmpty(item.Major))
                    {
                        appResult.Result = false;
                        appResult.Message += $"Invalid Major Education ({i})!";
                        return appResult;
                    }
                    if (String.IsNullOrEmpty(item.CollegeName))
                    {
                        appResult.Result = false;
                        appResult.Message += $"Invalid CollegeName Education ({i})!";
                        return appResult;
                    }
                    if (Convert.ToDateTime(item.StartDate).Year == 1)
                    {
                        appResult.Result = false;
                        appResult.Message += $"Invalid StartDate Education ({i})!";
                        return appResult;
                    }
                    if (item.EndDate != null)
                        if (Convert.ToDateTime(item.EndDate).Year == 1)
                        {
                            appResult.Result = false;
                            appResult.Message += $"Invalid EndDate Education ({i})!";
                            return appResult;
                        }
                }
            }
            List<CreateCertificateResource> createCertificateResources = ImportFileHandling.GetListCertifiate(list, 1);
            if (createCertificateResources.Any())
            {
                int i = 0;
                foreach (var item in createCertificateResources)
                {
                    i++;
                    if (String.IsNullOrEmpty(item.Name))
                    {
                        appResult.Result = false;
                        appResult.Message += $"Invalid Name Certificate ({i})!";
                        return appResult;
                    }
                    if (String.IsNullOrEmpty(item.Provider))
                    {
                        appResult.Result = false;
                        appResult.Message += $"Invalid Provider Certificate ({i})!";
                        return appResult;
                    }
                    if (Convert.ToDateTime(item.StartDate).Year == 1)
                    {
                        appResult.Result = false;
                        appResult.Message += $"Invalid StartDate Certificate ({i})!";
                        return appResult;
                    }
                    if (item.EndDate != null)
                        if (Convert.ToDateTime(item.EndDate).Year == 1)
                        {
                            appResult.Result = false;
                            appResult.Message += $"Invalid EndDate Certificate ({i})!";
                            return appResult;
                        }
                }
            }
            List<CreateWorkHistoryResource> workHistoryResources = ImportFileHandling.GetListWorkHistory(list, 1);
            if (workHistoryResources.Any())
            {
                int i = 0;
                foreach (var item in workHistoryResources)
                {
                    i++;
                    if (String.IsNullOrEmpty(item.CompanyName))
                    {
                        appResult.Result = false;
                        appResult.Message += $"Invalid CompanyName WorkHistory ({i})!";
                        return appResult;
                    }
                    if (String.IsNullOrEmpty(item.Position))
                    {
                        appResult.Result = false;
                        appResult.Message += $"Invalid Position WorkHistory ({i})!";
                        return appResult;
                    }
                    if (Convert.ToDateTime(item.StartDate).Year == 1)
                    {
                        appResult.Result = false;
                        appResult.Message += $"Invalid StartDate WorkHistory ({i})!";
                        return appResult;
                    }
                    if (item.EndDate != null)
                        if (Convert.ToDateTime(item.EndDate).Year == 1)
                        {
                            appResult.Result = false;
                            appResult.Message += $"Invalid EndDate WorkHistory ({i})!";
                            return appResult;
                        }
                }
            }
            List<Project> projects = ImportFileHandling.GetListProject(list, 1);
            if (projects.Any())
            {
                int i = 0;
                foreach (var item in projects)
                {
                    i++;
                    if (String.IsNullOrEmpty(item.Position))
                    {
                        appResult.Result = false;
                        appResult.Message += $"Invalid Position Project ({i})!";
                        return appResult;
                    }
                    if (String.IsNullOrEmpty(item.Name))
                    {
                        appResult.Result = false;
                        appResult.Message += $"Invalid Name Project ({i})!";
                        return appResult;
                    }
                    if (String.IsNullOrEmpty(item.Description))
                    {
                        appResult.Result = false;
                        appResult.Message += $"Invalid Description Project ({i})!";
                        return appResult;
                    }
                    if (String.IsNullOrEmpty(item.Responsibilities))
                    {
                        appResult.Result = false;
                        appResult.Message += $"Invalid Responsibilities Project ({i})!";
                        return appResult;
                    }
                    if (item.TeamSize <= 0)
                    {
                        appResult.Result = false;
                        appResult.Message += $"Invalid TeamSize Project ({i})!";
                        return appResult;
                    }
                    if (item.Technologies.Count <= 0)
                    {
                        appResult.Result = false;
                        appResult.Message += $"Invalid Technologies Project ({i})!";
                        return appResult;
                    }
                    if (Convert.ToDateTime(item.StartDate).Year == 1)
                    {
                        appResult.Result = false;
                        appResult.Message += $"Invalid StartDate Project ({i})!";
                        return appResult;
                    }
                    if (item.EndDate != null)
                        if (Convert.ToDateTime(item.EndDate).Year == 1)
                        {
                            appResult.Result = false;
                            appResult.Message += $"Invalid EndDate Project ({i})!";
                            return appResult;
                        }
                }
            }
            List<Category> categories = ImportFileHandling.GetListCategory(list);
            if (categories.Any())
            {
                int i = 0;
                foreach (var item in categories)
                {
                    i++;
                    if (item.Name.Contains(",") || String.IsNullOrEmpty(item.Name))
                    {
                        appResult.Result = false;
                        appResult.Message += $"Invalid Name Category ({i})!";
                        return appResult;
                    }
                    if (item.Technologies.Count <= 0)
                    {
                        appResult.Result = false;
                        appResult.Message += $"Invalid Technology ({i})!";
                        return appResult;
                    }
                    foreach (var itemTech in item.Technologies)
                    {
                        if (item.Name.Contains(",") || String.IsNullOrEmpty(itemTech.Name))
                        {
                            appResult.Result = false;
                            appResult.Message += $"Invalid Name Technology ({i})!";
                            return appResult;
                        }
                    }
                }
            }
            return appResult;
        }
    }
}
