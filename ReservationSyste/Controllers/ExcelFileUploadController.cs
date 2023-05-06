//using Microsoft.AspNetCore.Mvc;
//using System.Reflection.PortableExecutable;
//using System.Runtime.Intrinsics;
//using ExcelDataReader;
//using System.Data;

//namespace ReservationSyste.Controllers
//{
//    public class ExcelFileUploadController : Controller
//    {
//        private readonly IWebHostEnvironment _hostEnvironment;
//        IExcelDataReader _reader;

//        public ExcelFileUploadController(IWebHostEnvironment hostEnvironment)
//        {
//            _hostEnvironment = hostEnvironment;
//        }
//        public IActionResult Index()
//        {
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> UploadExcel(ExcelModel model)
//        {
//            try
//            {
//                if (model.ExcelFile == null || model.FileType == null)
//                {
//                    model.FileTypeList = ExcelFor.GetExcelType();
//                    ViewBag.mesage = "";
//                    return View();
//                }
//                //Create the directory if it does not exist
//                string dirPath = Path.Combine(_hostEnvironment.WebRootPath, "ReceivedExcels");
//                if (!Directory.Exists(dirPath))
//                {
//                    Directory.CreateDirectory(dirPath);
//                }
//                //Make sure thsat only excel file is used
//                string dataFileName = Path.GetFileName(model.ExcelFile?.FileName ?? "");
//                string extension = Path.GetExtension(dataFileName);
//                string[] allowedExtensions = new string[] { ".xls", ".xlsx" };
//                if (!allowedExtensions.Contains(extension))
//                {
//                    ViewBag.message = "Invalid file type, only excel file is allowed";
//                    return View(model);
//                }
//                //Make a copy of the posted file from the receieved http request
//                string saveToPath = Path.Combine(dirPath, dataFileName);
//                using (FileStream stream = new FileStream(saveToPath, FileMode.Create))
//                {
//                    model.ExcelFile.CopyTo(stream);
//                }
//                //Use this to handle Encoding differeences in .NET Core
//                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
//                //Read the excel file
//                using (var stream = new FileStream(saveToPath, FileMode.Open))
//                {
//                    if (extension == ".xls")
//                    {
//                        _reader = ExcelReaderFactory.CreateBinaryReader(stream);
//                    }
//                    else
//                    {
//                        _reader = ExcelReaderFactory.CreateBinaryReader(stream);
//                    }
//                    DataSet ds = new DataSet();
//                    ds = _reader.AsDataSet();
//                    _reader.Close();
//                    if (ds != null && ds.Tables.Count > 0)
//                    {
//                        // Read the table
//                        int count = 0;
//                        DataTable dt = ds.Tables[0];
//                        for (int i = 1; i < dt.Rows.Count; i++)
//                        {
//                            if (model.FileType == "CashBook")
//                            {
//                                CashBook cb = new CashBook();
//                                cb.BP = dt.Rows[i][0].ToString();
//                                cb.TransDate = dt.Rows[i][1].ToString();
//                                cb.Credit = dt.Rows[i][2].ToString();
//                                cb.Debit = dt.Rows[i][3].ToString();
//                                var cbResult = await _excelServeic.CreateCashBookExcel(cb);
//                            }
//                            if (model.FileType == "Transaction Statement")
//                            {
//                                TransactionStatement ts = new TransactionStatement();
//                                ts.RemitaRef = dt.Rows[i][0].ToString();
//                                ts.TransDate = dt.Rows[i][1].ToString();
//                                ts.Creadit = dt.Rows[i][2].ToString();
//                                ts.Debit = dt.Rows[i][3].ToString();

//                                var result = await _excelService.CreateTransactionStatement(ts);
//                            }
//                            if (model.FileType != null)
//                                DeleteFile(model);

//                            ViewBag.message = "Total nuber of rows inserted = " + count;
//                            model.FileTypeList = ExcelFor.GetExcelType();
//                            return View(model);
//                        }
//                    }
//                }

//            }
//            catch (Exception)
//            {

//                ViewBag.message = "";
//            }
//            model.FileTypeList = ExcelFor.GetFileType();
//            return View(model);
//        }
//    }
//}
