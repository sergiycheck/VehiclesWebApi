using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vehicles.Contracts.V1;


namespace vehicles.Helpers
{
    public class FileImgInfo
    {
        public string FileType { get; set; }
        public byte [] FileBytes { get; set; }
    }

    public interface IVehicleImageRetriever
    {
        public Task<FileImgInfo> GetImageByBrandAndUniqueNumber(
                string Brand,
                string UniqueNumber,
                string imgDirectory);

    }

    public class VehicleImageRetriever : IVehicleImageRetriever
    {

        public async Task<FileImgInfo> 
            GetImageByBrandAndUniqueNumber(string Brand, string UniqueNumber,string imgDirectory)
        {
            string[] fileEntries;

            var nameResult = ReplaceSpaceWithDash(Brand, UniqueNumber);
            if (Directory.Exists(imgDirectory))
            {
                fileEntries = Directory.GetFiles(imgDirectory);
                foreach (var fileName in fileEntries)
                {
                    if (fileName.Contains(nameResult))
                    {
                        var fileExtension = Path.GetExtension(fileName);
                        var file_type = "image";
                        if (!string.IsNullOrEmpty(fileExtension))
                        {
                            var bareExtension = fileExtension.Replace(".", string.Empty);
                            file_type = $"{file_type}/{bareExtension}";
                        }
                        var fileBytes = await File.ReadAllBytesAsync(fileName);

                        return new FileImgInfo()
                        {
                            FileBytes = fileBytes,
                            FileType = file_type
                        };
                    }
                }
            }

            throw new FileLoadException("Can not find file");
        }
        public string ReplaceSpaceWithDash(string Brand, string UniqueNumber)
        {
            var brandDash = Brand.Replace(" ", "_");
            var uniqueNumberDash = UniqueNumber.Replace(" ", "_");
            return $"{brandDash}_{uniqueNumberDash}";
        }
    }
}
