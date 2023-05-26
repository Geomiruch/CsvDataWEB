using CsvData.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CsvData.Services
{
    public interface ICsvDataService
    {
        Task<List<CsvDataModel>> GetDataAsync();
        Task<CsvDataModel> GetDataByIdAsync(int id);
        Task<List<CsvDataModel>> UploadDataAsync(IFormFile file);
        Task<CsvDataModel> UpdateDataAsync(int id, CsvDataModel updatedData);
        void DeleteDataAsync(int id);
    }
}
