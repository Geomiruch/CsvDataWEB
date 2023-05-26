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
using System.Threading.Tasks;

namespace CsvData.Services
{
    public class CsvDataService : ICsvDataService
    {
        private readonly CsvDataContext context;
        public CsvDataService(CsvDataContext context)
        {
            this.context = context;
        }
        public async void DeleteDataAsync(int id)
        {
            var existingData = await context.CsvData.FindAsync(id);

            if (existingData == null)
            {
                throw new Exception();
            }

            context.CsvData.Remove(existingData);
            await context.SaveChangesAsync();

        }

        public async Task<List<CsvDataModel>> GetDataAsync()
        {
            return await context.CsvData.ToListAsync();
        }

        public async Task<CsvDataModel> GetDataByIdAsync(int id)
        {
            return await context.CsvData.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<CsvDataModel> UpdateDataAsync(int id, CsvDataModel updatedData)
        {
            var existingData = await context.CsvData.FindAsync(id);

            if (existingData == null)
            {
                throw new Exception();
            }

            existingData.Name = updatedData.Name;
            existingData.DateOfBirth = updatedData.DateOfBirth;
            existingData.Married = updatedData.Married;
            existingData.Phone = updatedData.Phone;
            existingData.Salary = updatedData.Salary;

            await context.SaveChangesAsync();

            return existingData;
        }

        public async Task<List<CsvDataModel>> UploadDataAsync(IFormFile file)
        {
            foreach (var entity in context.CsvData)
                context.CsvData.Remove(entity);
            context.SaveChanges();

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", file.FileName);
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }


            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";"
            };
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<CsvDataModel>();
                foreach (var record in records)
                {
                    if (string.IsNullOrWhiteSpace(record.Name))
                    {
                        break;
                    }
                    CsvDataModel data;
                    data = new CsvDataModel();
                    data.Name = record.Name;
                    data.DateOfBirth = record.DateOfBirth;
                    data.Married = record.Married;
                    data.Salary = record.Salary;
                    data.Phone = record.Phone;

                    await context.CsvData.AddAsync(data);
                }
                await context.SaveChangesAsync();
            }

            return await context.CsvData.ToListAsync();
        }
    }
}
