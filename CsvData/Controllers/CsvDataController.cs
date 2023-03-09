using CsvData.Data;
using CsvData.Models;
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
        private readonly CsvDataContext context;
        public CsvDataController(CsvDataContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllData()
        {
            return Ok(await context.CsvData.ToListAsync());
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetDataById([FromRoute] int id)
        {
            return Ok(await context.CsvData.FirstOrDefaultAsync(x => x.Id == id));
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateData([FromRoute] int id, [FromBody] CsvDataModel updatedData)
        {
            var existingData = await context.CsvData.FindAsync(id);

            if(existingData==null)
            {
                return NotFound();
            }

            existingData.Name = updatedData.Name;
            existingData.DateOfBirth = updatedData.DateOfBirth;
            existingData.Married = updatedData.Married;
            existingData.Phone = updatedData.Phone;
            existingData.Salary = updatedData.Salary;

            await context.SaveChangesAsync();

            return Ok(existingData);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteData([FromRoute] int id)
        {
            var existingData = await context.CsvData.FindAsync(id);

            if (existingData == null)
            {
                return NotFound();
            }

            context.CsvData.Remove(existingData);
            await context.SaveChangesAsync();

            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            foreach (var entity in context.CsvData)
                context.CsvData.Remove(entity);
            context.SaveChanges();

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", file.FileName); 
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            string[] dataValues = null;
            string[] datas = System.IO.File.ReadAllLines(path, Encoding.UTF8);
            for (int i = 1; i < datas.Length; i++)
            {
                if (!String.IsNullOrEmpty(datas[i]))
                {
                    dataValues = datas[i].Split(';');
                    CsvDataModel data = new CsvDataModel();
                    data.Name = dataValues[1];
                    data.DateOfBirth = Convert.ToDateTime(dataValues[2]);
                    data.Married = Convert.ToBoolean(Convert.ToInt64(dataValues[3]));
                    data.Phone = dataValues[4];
                    data.Salary = Convert.ToDecimal(dataValues[5]);
                    await context.CsvData.AddAsync(data);

                    await context.SaveChangesAsync();
                }
            }

            return Ok(await context.CsvData.ToListAsync());
        }

    }
}
