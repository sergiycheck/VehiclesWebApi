using Microsoft.AspNetCore.Http;
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

        public Task<string> CreateImgByBrandAndUniqueNumber(
                        IFormFile file,
                        string Brand,
                        string UniqueNumber,
                        string imgDirectory);

        public FileImgInfo GetFileImgInfoByImgPath(string imgPath);

        public Task<FileImgInfo>
            GetImageByBrandAndUniqueNumber(
                string Brand,
                string UniqueNumber,
                string imgDirectory);
        public string GetFilePathByVehicleBrandAndUniqueNumber
                (string Brand, string UniqueNumber, string imgDirectory);

        public bool DeleteFile(string imgPath);
    }

    public class VehicleImageRetriever : IVehicleImageRetriever
    {

        public async Task<FileImgInfo> 
            GetImageByBrandAndUniqueNumber(
                string Brand, 
                string UniqueNumber,
                string imgDirectory)
        {
            string[] fileEntries;

            var nameResult = ReplaceSpaceWithDash(Brand, UniqueNumber);
            if (Directory.Exists(imgDirectory))
            {
                fileEntries = Directory.GetFiles(imgDirectory);
                
                foreach (var fileNamePath in fileEntries)
                {
                    var fileExtension = Path.GetExtension(fileNamePath);

                    var fileNameWithExt = Path.GetFileName(fileNamePath);

                    fileNameWithExt = fileNameWithExt.Replace(fileExtension, "");

                    if (fileNameWithExt.ToUpper() == nameResult.ToUpper())
                    {
                        
                        var file_type = "image";
                        if (!string.IsNullOrEmpty(fileExtension))
                        {
                            var bareExtension = fileExtension.Replace(".", string.Empty);
                            file_type = $"{file_type}/{bareExtension}";
                        }
                        var fileBytes = await File.ReadAllBytesAsync(fileNamePath);

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

        public FileImgInfo GetFileImgInfoByImgPath(string imgPath)
        {
            var fileExtension = Path.GetExtension(imgPath);

            var file_type = "image";
            if (!string.IsNullOrEmpty(fileExtension))
            {
                var bareExtension = fileExtension.Replace(".", string.Empty);
                file_type = $"{file_type}/{bareExtension}";
            }
            var fileBytes = File.ReadAllBytes(imgPath);

            return new FileImgInfo()
            {
                FileBytes = fileBytes,
                FileType = file_type
            };
        }

        public bool DeleteFile(string imgPath)
        {
            if (imgPath != string.Empty && File.Exists(imgPath))
            {
                File.Delete(imgPath);
                return true;
            }
            return false;
            
        }

        public string GetFilePathByVehicleBrandAndUniqueNumber
                (string Brand, string UniqueNumber, string imgDirectory)
        {
            string[] fileEntries;

            var nameResult = ReplaceSpaceWithDash(Brand, UniqueNumber);
            if (Directory.Exists(imgDirectory))
            {
                fileEntries = Directory.GetFiles(imgDirectory);

                foreach (var fileNamePath in fileEntries)
                {
                    var fileExtension = Path.GetExtension(fileNamePath);

                    var fileNameWithExt = Path.GetFileName(fileNamePath);

                    fileNameWithExt = fileNameWithExt.Replace(fileExtension, "");

                    if (fileNameWithExt.ToUpper() == nameResult.ToUpper())
                    {
                        return fileNamePath;
                    }
                }
            }
            return "";
        }
        


        public async Task<string> 
            CreateImgByBrandAndUniqueNumber(
                IFormFile file,
                string Brand,
                string UniqueNumber,
                string imgDirectory)
        {
            if (file == null)
            {
                return "file is null";
            }
            var fileExtension = Path.GetExtension(file.FileName);
            var nameResult = ReplaceSpaceWithDash(Brand, UniqueNumber);
            if (Directory.Exists(imgDirectory))
            {
                var imgPath = $@"{imgDirectory}\{nameResult}{fileExtension}";
                using (var fileStream = new FileStream(imgPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                return imgPath;
            }
            return string.Empty;
        }
        
        public string ReplaceSpaceWithDash(string Brand, string UniqueNumber)
        {
            var brandDash = Brand.Replace(" ", "_");
            var uniqueNumberDash = UniqueNumber.Replace(" ", "_");
            return $"{brandDash}_{uniqueNumberDash}";
        }
    }
}
