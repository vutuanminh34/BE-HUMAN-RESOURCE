using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using Spire.Xls;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Common;
using WebAPI.Models;
using WebAPI.Services.Certificates;
using WebAPI.Services.Educations;
using WebAPI.Services.Persons;
using WebAPI.Services.Projects;
using WebAPI.Services.Skills;
using WebAPI.Services.WorkHistories;

namespace WebAPI.Services.ExportFiles
{
    public class ExportFileService : IExportFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppSettings _appSettings;
        private readonly IPersonService _personService;
        private readonly ISkillService _skillService;
        private readonly IWorkHistoryService _workHistoryService;
        private readonly IEducationService _educationService;
        private readonly ICertificateService _certificateService;
        private readonly IProjectService _projectService;

        public ExportFileService(IWebHostEnvironment webHostEnvironment,
            IOptions<AppSettings> appSettings,
            IPersonService personService,
            ISkillService skillService,
            IWorkHistoryService workHistoryService,
            IEducationService educationService,
            ICertificateService certificateService,
            IProjectService projectService)
        {
            this._webHostEnvironment = webHostEnvironment;
            this._appSettings = appSettings.Value;
            this._personService = personService;
            this._skillService = skillService;
            this._workHistoryService = workHistoryService;
            this._educationService = educationService;
            this._certificateService = certificateService;
            this._projectService = projectService;
        }
        public async Task<string> ExportFile(int id, string type)
        {
            await Task.Yield();
            var profilePerson = await _personService.GetPersonById(id);
            var listSkill = await _skillService.GetSkillByPerson(id);
            var listWorkHistory = await _workHistoryService.GetWorkHistoryByPersonId(id);
            var listEducation = await _educationService.GetEducationByPersonId(id);
            var listCertificate = await _certificateService.GetCertificateByPersonId(id);
            var listProject = await _projectService.GetProjectByPersonId(id);
            if (profilePerson == null)
            {
                return null;
            }
            ExportFileUtils exportFileUtils = new ExportFileUtils();
            string fullPathTemplate = $@"{_webHostEnvironment.WebRootPath}/Form_CV.xlsx";
            string sheetName = "Sheet1";
            using (var source = System.IO.File.OpenRead(fullPathTemplate))
            using (ExcelPackage excelPkg = new ExcelPackage(source))
            {
                ExcelWorksheet worksheet = exportFileUtils.SheetTemPlate(excelPkg, sheetName);
                int irecordIndex = 3;
                StringBuilder strBuilder = new StringBuilder(string.Empty);
                string defaultSpace = "   ";

                #region Export profile Person
                Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#bdd6ee");
                worksheet.Cells[$"B{irecordIndex}:M{irecordIndex}"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                worksheet.Cells[$"B{irecordIndex}"].Value = profilePerson.FullName;
                worksheet.Cells[$"B{irecordIndex + 1}"].Value = defaultSpace + "Office";
                worksheet.Cells[$"D{irecordIndex + 1}"].Value = defaultSpace + profilePerson.Location;
                worksheet.Cells[$"B{irecordIndex + 2}"].Value = defaultSpace + "Gender";
                worksheet.Cells[$"D{irecordIndex + 2}"].Value = defaultSpace + profilePerson.Gender;
                worksheet.Cells[$"B{irecordIndex + 3}"].Value = defaultSpace + "Year of Birth";
                worksheet.Cells[$"D{irecordIndex + 3}"].Value = defaultSpace + profilePerson.YearOfBirth.Year;
                worksheet.Cells[$"B{irecordIndex + 4}:N{irecordIndex + 4}"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                worksheet.Cells[$"B{irecordIndex + 5}"].Value = profilePerson.Description;
                irecordIndex += 5;
                #endregion

                #region Export list Skill
                int irecordSkill = irecordIndex + 4;
                int itemSkill = listSkill.ToNonNullList().Count;
                if (irecordSkill + itemSkill <= irecordIndex + 14)
                    worksheet.DeleteRow(irecordSkill, 10 - itemSkill, false);
                else if (irecordSkill + itemSkill - 10 >= irecordIndex + 4)
                {
                    worksheet.InsertRow(irecordSkill + 10, itemSkill - 10, irecordSkill);
                }
                worksheet.Cells[$"B{irecordSkill - 1}:N{irecordSkill - 1}"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                foreach (var item in listSkill)
                {
                    worksheet.Cells[$"B{irecordSkill}"].Value = defaultSpace + item.Name;
                    int lengthTemp = item.Technologies.Count;
                    for (int i = 0; i < lengthTemp; i++)
                    {
                        if (i == (lengthTemp - 1))
                        {
                            strBuilder.Append($"{item.Technologies[i].Name}");
                            break;
                        }
                        strBuilder.Append($"{item.Technologies[i].Name}, ");
                    }
                    worksheet.Cells[$"D{irecordSkill}"].Value = defaultSpace + strBuilder.ToString();
                    strBuilder.Clear();
                    irecordSkill++;
                }
                #endregion

                #region Export list WorkHistory
                int irecordWorkHistory = irecordSkill + 2;
                int irecordNumber = 1;
                int itemWorkHistory = listWorkHistory.ToNonNullList().Count;
                if (irecordWorkHistory + itemWorkHistory <= irecordSkill + 12)
                    worksheet.DeleteRow(irecordWorkHistory, 10 - itemWorkHistory, true);
                else if (irecordWorkHistory + itemWorkHistory - 10 >= irecordSkill + 2)
                    worksheet.InsertRow(irecordWorkHistory + 10, itemWorkHistory - 10, irecordWorkHistory);
                worksheet.Cells[$"B{irecordWorkHistory - 2}:N{irecordWorkHistory - 2}"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                foreach (var item in listWorkHistory)
                {
                    worksheet.Cells[$"B{irecordWorkHistory}"].Value = irecordNumber.ToString();
                    worksheet.Cells[$"C{irecordWorkHistory}"].Value = item.EndDate + "-" + item.StartDate;
                    worksheet.Cells[$"F{irecordWorkHistory}"].Value = item.CompanyName;
                    worksheet.Cells[$"M{irecordWorkHistory}"].Value = item.Position;
                    irecordNumber++;
                    irecordWorkHistory++;

                }
                #endregion

                #region Export list Education
                int irecordEducation = irecordWorkHistory + 1;
                int itemEducation = listEducation.ToNonNullList().Count;
                if (irecordEducation + 2 * itemEducation <= irecordWorkHistory + 13)
                    worksheet.DeleteRow(irecordEducation, 12 - 2 * itemEducation, true);
                else if (irecordEducation + 2 * itemEducation - 12 >= irecordWorkHistory + 1)
                    worksheet.InsertRow(irecordEducation, 2 * itemEducation - 12, irecordEducation);
                worksheet.Cells[$"B{irecordEducation - 1}:N{irecordEducation - 1}"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                foreach (var item in listEducation)
                {

                    worksheet.Cells[$"B{irecordEducation}"].Value = defaultSpace + item.EndDate + " - " + item.StartDate + " | " + item.CollegeName;
                    worksheet.Cells[$"B{irecordEducation + 1}"].Value = defaultSpace + "Major: " + item.Major;
                    irecordEducation += 2;
                }
                #endregion

                #region Export list Certificate
                int irecordCertificate = irecordEducation + 1;
                int itemCertificate = listCertificate.ToNonNullList().Count;
                if (irecordCertificate + itemCertificate <= irecordEducation + 11)
                    worksheet.DeleteRow(irecordCertificate, 10 - itemCertificate, true);
                else if (irecordCertificate + itemCertificate - 10 >= irecordEducation + 1)
                    worksheet.InsertRow(irecordCertificate + 10, itemCertificate - 10, irecordEducation);
                worksheet.Cells[$"B{irecordCertificate - 1}:N{irecordCertificate - 1}"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                foreach (var item in listCertificate)
                {
                    worksheet.Cells[$"B{irecordCertificate}"].Value = defaultSpace + item.StartDate + " | " + item.Name + " - " + item.Provider;
                    irecordCertificate++;
                }
                #endregion

                #region Export list Project
                int irecordProject = irecordCertificate + 2;
                irecordNumber = 1;
                int itemProject = listProject.ToNonNullList().Count;
                if (irecordProject + 5 * itemProject <= irecordEducation + 42)
                    worksheet.DeleteRow(irecordProject, 40 - 5 * itemProject, true);
                else if (irecordProject + 5 * itemProject - 40 >= irecordCertificate + 2)
                    worksheet.InsertRow(irecordProject, 5 * itemProject - 40, irecordEducation);
                worksheet.Cells[$"B{irecordProject - 2}:N{irecordProject - 2}"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                foreach (var item in listProject)
                {

                    worksheet.Cells[$"B{irecordProject}"].Value = irecordNumber.ToString();
                    worksheet.Cells[$"C{irecordProject }"].Value = item.EndDate.ToString("MM/yyyy") + " - " + item.StartDate.ToString("MM/yyyy");
                    worksheet.Cells[$"E{irecordProject }"].Value = item.Position;
                    worksheet.Cells[$"I{irecordProject }"].Value = item.Name;
                    worksheet.Cells[$"I{irecordProject + 1}"].Value = "Description: " + item.Description;
                    worksheet.Cells[$"I{irecordProject + 2}"].Value = "Responsibilities: " + item.Responsibilities;
                    worksheet.Cells[$"I{irecordProject + 3}"].Value = "TeamSize: " + item.TeamSize;
                    int lengthTemp = item.Technologies.Count;
                    for (int i = 0; i < lengthTemp; i++)
                    {
                        if (i == (lengthTemp - 1))
                        {
                            strBuilder.Append($"{item.Technologies[i].Name}");
                            break;
                        }
                        strBuilder.Append($"{item.Technologies[i].Name}, ");
                    }
                    worksheet.Cells[$"I{irecordProject + 4}"].Value = "Technologies used:  " + strBuilder.ToString();
                    strBuilder.Clear();
                    irecordProject += 5;
                    irecordNumber++;
                }
                #endregion

                #region Write Image to excel
                int rowIndex = 2;
                int colIndex = 13;
                int Width = 149;
                int Height = 225;
                string pathDefaultImage = _webHostEnvironment.WebRootPath + _appSettings.ImagePath + "avatar-default.png";
                string pathImage = _webHostEnvironment.WebRootPath + _appSettings.ImagePath + $"{profilePerson.Avatar}";
                Image avatar;
                if (!System.IO.File.Exists(pathImage)) avatar = Image.FromFile(pathDefaultImage);
                else avatar = Image.FromFile(pathImage);
                Bitmap img = new Bitmap(avatar);
                if (img.HorizontalResolution == 0 || img.VerticalResolution == 0)
                    img.SetResolution(96, 96);
                OfficeOpenXml.Drawing.ExcelPicture pic = worksheet.Drawings.AddPicture("Sample", img);
                pic.SetPosition(rowIndex, 0, colIndex, 0);
                pic.SetSize(Width, Height);
                #endregion

                excelPkg.Save();

                string p_strPath = _webHostEnvironment.WebRootPath + _appSettings.CVPath + $"CV_{profilePerson.FullName}_{profilePerson.StaffId}.xlsx";

                if (System.IO.File.Exists(p_strPath))
                    System.IO.File.Delete(p_strPath);

                // Create excel file on physical disk  
                FileStream objFileStrm = System.IO.File.Create(p_strPath);
                objFileStrm.Close();

                // Write content to excel file  
                System.IO.File.WriteAllBytes(p_strPath, excelPkg.GetAsByteArray());
            }
            #region Convert Excel to Pdf + download
            string p_strPathExcel = _webHostEnvironment.WebRootPath + _appSettings.CVPath + $"CV_{profilePerson.FullName}_{profilePerson.StaffId}.xlsx";
            string p_strPathPdf = _webHostEnvironment.WebRootPath + _appSettings.CVPath + $"CV_{profilePerson.FullName}_{profilePerson.StaffId}.pdf";
            var pdflName = $"CV/CV_{profilePerson.FullName}_{profilePerson.StaffId}.pdf";
            var excelName = $"CV/CV_{profilePerson.FullName}_{profilePerson.StaffId}.xlsx";
            string downloadUrl = string.Format($"{WebAPI.Helpers.HttpContext.Current.Request.Scheme}://{WebAPI.Helpers.HttpContext.Current.Request.Host.Value}/{excelName}");
            if (type == "excel")
            {
                return downloadUrl;
            }
            Workbook workBook = new Workbook();
            workBook.LoadFromFile(p_strPathExcel, ExcelVersion.Version2010);
            workBook.SaveToFile(p_strPathPdf, FileFormat.PDF);

            downloadUrl = string.Format($"{WebAPI.Helpers.HttpContext.Current.Request.Scheme}://{WebAPI.Helpers.HttpContext.Current.Request.Host.Value}/{pdflName}");
            return downloadUrl;
            #endregion

        }
        public async Task<string> DownloadTemplateCV()
        {
            string templateCV = "CV/Template_CV.xlsx";
            string downloadUrl = string.Format($"{WebAPI.Helpers.HttpContext.Current.Request.Scheme}://{WebAPI.Helpers.HttpContext.Current.Request.Host.Value}/{templateCV}");
            return downloadUrl;
        }

    }
}
