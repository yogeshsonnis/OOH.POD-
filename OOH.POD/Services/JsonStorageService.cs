using OOH.POD.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OOH.POD.Services
{
    public class JsonStorageService
    {
        private readonly string _folderPath;
        private readonly string _filePath;

        public JsonStorageService()
        {
            _folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");


            _filePath = Path.Combine(_folderPath, "compartments.json");


            EnsureStorage();
        }

        private void EnsureStorage()
        {
            if (!Directory.Exists(_folderPath))
                Directory.CreateDirectory(_folderPath);

            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "[]"); // create empty JSON list
        }

        public async Task<List<Compartment>> LoadAsync()
        {
            try
            {
                string json = await File.ReadAllTextAsync(_filePath);

                if (string.IsNullOrWhiteSpace(json))
                    return new List<Compartment>();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true
                };

                var result = JsonSerializer.Deserialize<List<Compartment>>(json, options);
                return result ?? new List<Compartment>();
            }
            catch (Exception ex)
            {
                await File.WriteAllTextAsync("json_error.log", ex.ToString());
                return new List<Compartment>();
            }
        }

        public async Task SaveAsync(List<Compartment> compartments)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(compartments, options);

                string tempFile = _filePath + ".tmp";

                // Write atomically: write to temp, then replace the original
                await File.WriteAllTextAsync(tempFile, json);
                File.Copy(tempFile, _filePath, overwrite: true);
                File.Delete(tempFile);
            }
            catch (Exception ex)
            {
                await File.WriteAllTextAsync("json_error.log", ex.ToString());
            }
        }
    }
}
