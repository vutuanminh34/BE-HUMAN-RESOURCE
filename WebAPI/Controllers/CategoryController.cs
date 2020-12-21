using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.RequestModel;
using WebAPI.Services.Categories;
using WebAPI.ViewModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : BaseApiController<CategoryViewModel>
    {
        private ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            this._categoryService = categoryService;
        }

        /// <summary>
        /// Get all Category
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod + "," + Role.user)]
        [HttpGet]
        public async Task<IActionResult> GetAllCategory(int? page, int? limit, string search)
        {
            var app = await _categoryService.GetAllCategory(page, limit, search);
            return Ok(app);
        }

        /// <summary>
        /// Create Category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPost]
        public async Task<IActionResult> InsertCategory([FromBody] Category category)
        {
            var app = await _categoryService.InsertCategory(category);
            if (!app.AppResult.Result)
                return BadRequest(app.AppResult);
            return Ok(app.AppResult);
        }

        /// <summary>
        /// Update Category
        /// </summary>
        /// <param name="category"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory([FromBody] Category category, int id)
        {
            var app = await _categoryService.UpdateCategory(category, id);
            if (!app.AppResult.Result)
                return BadRequest(app.AppResult);
            return Ok(app.AppResult);
        }

        /// <summary>
        /// Delete Category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin + "," + Role.Mod)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var app = await _categoryService.DeleteCategory(id);
            if (!app.AppResult.Result)
                return BadRequest(app.AppResult);
            return Ok(app.AppResult);
        }
    }
}
