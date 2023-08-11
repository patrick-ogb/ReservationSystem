using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ReservationSyste.Models;
using ReservationSyste.ViewModels;
using RestSharp;
using System.Data;
using OfficeOpenXml;
using System.IO;
using Microsoft.Data.SqlClient;

namespace ReservationSyste.Controllers
{
    public class VendorController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VendorController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Vendor()
        {

            return View(new VendorVM());
        }

        [HttpPost]
        public async Task<IActionResult> Vendor( VendorVM model)
        {
            if(ModelState.IsValid)
            {
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Upload");
                string uniqueFileName = string.Empty;
                uniqueFileName = model.ExcleFile?.FileName;
                string completepath = Path.Combine(uploadFolder, uniqueFileName);

                using (Stream stream = new FileStream(completepath, FileMode.Create))
                {
                    await model.ExcleFile!.CopyToAsync(stream);
                }

                DataTable dt =  ReadExcelToDataTable(completepath);


                using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:BulkData"]))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("InsertBulkData", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Data", dt);
                        command.ExecuteNonQuery();
                    }
                }



            }
            return View(new VendorVM());
        }


        public DataTable ReadExcelToDataTable(string filePath)
        {
            using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming the first worksheet is used

                DataTable dataTable = new DataTable();

                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                // Add columns to the DataTable
                for (int col = 1; col <= colCount; col++)
                {
                    DataColumn column = new DataColumn(worksheet.Cells[1, col].Value.ToString());
                    dataTable.Columns.Add(column);
                }

                // Add rows to the DataTable
                for (int row = 2; row <= rowCount; row++)
                {
                    DataRow dataRow = dataTable.NewRow();

                    for (int col = 1; col <= colCount; col++)
                    {
                        dataRow[col - 1] = worksheet.Cells[row, col].Value;
                    }

                    dataTable.Rows.Add(dataRow);
                }

                return dataTable;
            }
        }



    }

}
