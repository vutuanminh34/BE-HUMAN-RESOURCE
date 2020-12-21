using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Common;
using WebAPI.Constants;
using WebAPI.Models;
using WebAPI.Models.Resource.Person;
using WebAPI.Repositories.Persons;
using WebAPI.Services.Persons;
using WebAPI.ViewModels;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : BaseApiController<PersonViewModel>
    {
        private IPersonService _personService;
        private IPersonRepository _personRepository;
        public PersonController(IPersonService personSevice, IPersonRepository personRepository)
        {
            this._personService = personSevice;
            this._personRepository = personRepository;
        }
        /// <summary>
        /// 
        /// GET PERSON BY ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod + "," + Role.user)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPersonById(int id)
        {
            var app = await _personService.GetPersonById(id);
            if (app == null)
            {
                //return bad request
                apiResult.AppResult.Message = Constant.PERSONID_ERROR;
                return BadRequest(apiResult.AppResult);
            };
            apiResult.AppResult.Message = Constant.INFO_PERSON_BY_ID;
            apiResult.AppResult.DataResult = app;
            return Ok(apiResult.AppResult);
        }

        /// <summary>
        /// SEARCH CV PERSON AND SKILL BY CONDITION (Pamameter:  Fullname, location, TechologyId)
        /// </summary>
        /// <param name="FullName"></param>
        /// <param name="Location"></param>
        /// <param name="TechnologyId"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod + "," + Role.user)]
        [HttpGet]
        public async Task<IActionResult> SearchPerson(int? page, int? limit, string FullName, int? Location, string technologyId)
        {
            List<int> myList = new List<int>();

            //Validate info person

            #region Validate person
            if (page is null && limit is null && FullName is null && Location is null)
            {
                apiResult.BaseModel.Message = Constant.ALL_PARAMETER_INVALID;
                return BadRequest(apiResult.BaseModel);
            }
            if (page < 1)
            {
                apiResult.BaseModel.Message = Constant.PAGE_INVALID;
                return BadRequest(apiResult.BaseModel);
            }
            if (limit < 1)
            {
                apiResult.BaseModel.Message = Constant.LIMIT_INVALID;
                return BadRequest(apiResult.BaseModel);
            }
            if (Location != null && Functions.CheckLocation((int)Location) == false)
            {
                apiResult.BaseModel.Message = Constant.LOCATION_INVALID;
                return BadRequest(apiResult.BaseModel);
            }
            if (page != null && limit is null)
            {
                apiResult.BaseModel.Message = Constant.LIMIT_INVALID;
                return BadRequest(apiResult.BaseModel);
            }
            if (limit != null && page is null)
            {
                apiResult.BaseModel.Message = Constant.PAGE_INVALID;
                return BadRequest(apiResult.BaseModel);
            }
            if ((FullName != null || Location != null) && (page is null && limit is null))
            {
                apiResult.BaseModel.Message = Constant.PAGINATION;
                return BadRequest(apiResult.BaseModel);
            }
            #endregion

            if (!String.IsNullOrEmpty(technologyId))
            {
                myList = technologyId.Split(',').Select(Int32.Parse).ToList();
            }

            /* technologId = TechnologyId.Split(',').Select(Int32.Parse).ToList();*/
            var app = await _personService.SearchPersonAndSkillAsync((int)page, (int)limit, FullName, Location, myList);
            if (Location == null)
            {
                Location = -1;
            }
            var count = await _personRepository.TotalPersonAndSkill(FullName, (int)Location, myList);
            apiResult.BaseModel.DataResult = app;
            apiResult.BaseModel.totalCount = count;
            apiResult.BaseModel.Message = Constant.INFO_PERSONS;
            apiResult.BaseModel.DataResult = app;
            return Ok(apiResult.BaseModel);
        }



        /// <summary>
        /// CREATE PERSON AND AVATAR BY ID PERSON
        /// NOTE:  this API create info person and after use this id person and insert avatar
        /// </summary>
        /// <param name="personInfo"></param>
        /// <param name="objectFile"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPost]
        public async Task<IActionResult> InsertPerson([FromForm] Person personInfo, [FromForm] FileUpload objectFile)
        {
            var app = await _personService.InsertPerson(personInfo, objectFile);
            if (app.AppResult.DataResult == null)
            {
                return BadRequest(app.AppResult);
            }
            return Ok(app.AppResult);
        }

        /// <summary>
        /// UPDATE PERSON BY ID PERSON
        /// </summary>
        /// <param name="personInfo"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerson([FromForm] string file, [FromForm] Person personInfo, [FromForm] FileUpload objectFile,  int id)
        {
            personInfo.Id = id;
            var app = await _personService.UpdatePerson(file, personInfo, objectFile);
            if (app.AppResult.DataResult == null)
            {
                return BadRequest(app.AppResult);
            }
            return Ok(app.AppResult);
        }

        /// <summary>
        /// UPDATE DESCRIPTION BY ID PERSON
        /// </summary>
        /// <param name="personObj"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPut("description/{id}")]
        public async Task<IActionResult> UpdateOverview([FromBody] SavePersonResource personObj, int id)
        {
            personObj.Id = id;
            var app = await _personService.UpdateOverview(personObj);
            if (app.AppResult.DataResult == null)
            {
                return BadRequest(app.AppResult);
            }
            return Ok(app.AppResult);
        }

        /// <summary>
        /// DELETE PERSON BY ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var app = await _personService.DeletePerson(id);
            if (!app.AppResult.Result)
            {
                return BadRequest(app.AppResult);
            }
            return Ok(app.AppResult);
        }
    }
}

