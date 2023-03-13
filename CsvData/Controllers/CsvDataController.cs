using CsvData.Data;
using CsvData.Models;
using CsvData.Services;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvData.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CsvDataController : Controller
    {
        private readonly ICsvDataService csvDataService;

        public CsvDataController(ICsvDataService csvDataService)
        {
            this.csvDataService = csvDataService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllData()
        {
            return Ok(await csvDataService.GetDataAsync());
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetDataById([FromRoute] int id)
        {
            return Ok(await csvDataService.GetDataByIdAsync(id));
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateData([FromRoute] int id, [FromBody] CsvDataModel updatedData)
        {
            try
            {
                return Ok(await csvDataService.UpdateDataAsync(id, updatedData));
            }
            catch(Exception)
            {
                return NotFound();
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteData([FromRoute] int id)
        {
            try
            {
                csvDataService.DeleteDataAsync(id);
            }
            catch(Exception)
            {
                return NotFound();
            }
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            return Ok(await csvDataService.UploadDataAsync(file));
        }

    }
}
