using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrustucture
{
    public static class IformfileToUrl
    {
        public static async Task<string> UploadFile(IFormFile file, Guid id)
        {
            if (file == null || file.Length == 0)
                throw new BadRequestException("File is empty");

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

            string uniqueFileName = id.ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            string fileUrl = $"/uploads/{uniqueFileName}"; // Relative URL
            return fileUrl;
        }
    }
}
