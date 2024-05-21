using ClosedXML.Excel;
using Helpers.ApiHelpers;
using Helpers.ConversionHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.CommonHelpers
{
    public class FilesHelpers : IFilesHelpers
    {



        private readonly IConstants _constants;

        public FilesHelpers(IConstants constants)
        {

            this._constants = constants;

        }


        public async Task<string> SaveFileToDirectory(IFormFile File, string? FileDirectory)
        {

            try
            {
                string FileGuidName = String.Empty;
                string FileCompletePath = String.Empty;
                string url = String.Empty;
                Guid Guid = Guid.NewGuid();
                FileGuidName = Guid.ToString().Substring(0, 3) + "_" + File.FileName;
                FileGuidName =StringConversionHelper.ReplaceSpacesInString(FileGuidName);
                FileGuidName =StringConversionHelper.MakeFileNameValid(FileGuidName);

                if (String.IsNullOrWhiteSpace(FileDirectory))
                {
                    FileDirectory = _constants.GetAppSettingKeyValue("AppSetting", "OtherImagesDirectory");
                    FileCompletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + FileDirectory, FileGuidName);

                }
                else
                {
                    FileCompletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + FileDirectory, FileGuidName);
                }


                var stream = new FileStream(FileCompletePath, FileMode.Create);
                await File.CopyToAsync(stream);
                stream.Close(); //-- stream.Close()  line were added because if you want to use the excel or other file directly after saving then it will throws error that the file is already in use by other process. So it is just for safe side

                url = FileDirectory + "/" + FileGuidName;


                await Task.FromResult(url);
                return (url);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<IActionResult> ExportToExcel(ControllerBase controller, string FileName, List<dynamic>? DataForExort)
        {
            #region DataTableColumns
            DataTable dt = new DataTable("Grid");
            if (DataForExort != null)
            {
                Dictionary<string, object>? dicCols = new Dictionary<string, object>();
                string JSONColResult = JsonConvert.SerializeObject(DataForExort.FirstOrDefault());
                dicCols = !String.IsNullOrWhiteSpace(JSONColResult) ? JsonConvert.DeserializeObject<Dictionary<string, object>>(JSONColResult) : new Dictionary<string, object>();
                if (dicCols != null && dicCols.Count > 0)
                {
                    foreach (var item in dicCols)
                    {
                        //--check if any column is not null then add in list for excel export
                        var matches = DataForExort.Where(e => DataForExort.Select(c => e.GetType()
                                                                      ?.GetProperty(item.Key)
                                                                      ?.GetValue(e)
                                                                      ?.ToString() != null)
                                                          .All(c => c == true));

                        if (matches != null && matches.Count() > 0 && item.Key != "PageSize" && item.Key != "PageNo" && item.Key != "CreatedBy" && item.Key != "ModifiedBy" && item.Key != "TotalRecords")
                        {
                            dt.Columns.Add(new DataColumn(item.Key));
                        }

                        // dt.Columns.Add(new DataColumn(item.Key));
                    }

                }
            }
            #endregion

            #region Data Table Columns Values from list
            Dictionary<string, object>? dicRow = new Dictionary<string, object>();
            if (DataForExort != null)
            {
                //--Get data table columns
                DataColumnCollection columns = dt.Columns;

                foreach (var row in DataForExort)
                {
                    //--empty the dictionary after each iteration
                    dicRow = new Dictionary<string, object>();

                    string JSONresult = JsonConvert.SerializeObject(row);
                    dicRow = !String.IsNullOrWhiteSpace(JSONresult) ? JsonConvert.DeserializeObject<Dictionary<string, object>>(JSONresult) : new Dictionary<string, object>();
                    List<string> rowOfData = new List<string>();
                    if (dicRow != null && dicRow.Count > 0)
                    {
                        foreach (var item in dicRow)
                        {

                            if (columns.Contains(item.Key))
                            {

                                bool IsDateTime = item.Value != null ? DateTimeConversionHelper.CheckValueIsDateTime(item.Value.ToString()) : false;


                                if (IsDateTime)
                                {
									try
									{
										rowOfData.Add(Convert.ToDateTime(item.Value).ToString("dddd, dd MMMM yyyy"));
									}
									catch
									{
										rowOfData.Add((item.Value == null ? "" : item.Value.ToString()));
									}
								}
                                else
                                {
                                    rowOfData.Add((item.Value == null ? "" : item.Value.ToString()));
                                }

                            }

                        }
                        dt.Rows.Add(rowOfData.ToArray());
                    }


                }

            }
            #endregion

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);

                    return controller.File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName + " " + DateTime.Now.ToString() + ".xlsx");

                }
            }




        }

        public async Task<string> DeleteAnyFileFromDirectory(string FilePath)
        {
            string result = string.Empty;

            try
            {
                //-- in start of FilePath, please append "/" symbol if it is not already append. Like "/commonImages/images/noor1.jpg"

                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + FilePath);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                result = "Deleted Successfully";

                await Task.FromResult(result);
                return (result);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<DataTable?> ConvertExcelFirstSheetToDataTable(string FilePath, bool? IwwwrootDirectoryRequired)
        {
        

            try
            {
                string pathFull = string.Empty;
                DataTable dt = new DataTable();

                if (IwwwrootDirectoryRequired != null && IwwwrootDirectoryRequired == true)
                {
                    pathFull = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + FilePath);
                }
                else
                {
                    pathFull = FilePath;
                }


                using (XLWorkbook workbook = new XLWorkbook(pathFull))
                {
                    IXLWorksheet worksheet = workbook.Worksheet(1);
                    bool FirstRow = true;
                    //Range for reading the cells based on the last cell used.
                    string readRange = "1:1";
                    foreach (IXLRow row in worksheet.RowsUsed())
                    {
                        //If Reading the First Row (used) then add them as column name
                        if (FirstRow)
                        {
                            //Checking the Last cellused for column generation in datatable
                            readRange = string.Format("{0}:{1}", 1, row.LastCellUsed().Address.ColumnNumber);
                            foreach (IXLCell cell in row.Cells(readRange))
                            {
                                dt.Columns.Add(cell.Value.ToString());
                            }
                            FirstRow = false;
                        }
                        else
                        {
                            //Adding a Row in datatable
                            dt.Rows.Add();
                            int cellIndex = 0;
                            //Updating the values of datatable
                            foreach (IXLCell cell in row.Cells(readRange))
                            {
                                dt.Rows[dt.Rows.Count - 1][cellIndex] = cell.Value.ToString();
                                cellIndex++;
                            }
                        }
                    }
                    //If no data in Excel file
                    if (FirstRow)
                    {
                        return dt; // --Empty Excel File!
                    }
                }


                await Task.FromResult(dt);
                return (dt);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<string> GetFileContentTypeForFileExtension(string fileExtension)
        {
            string contentType = "application/octet-stream";

            try
            {

                switch (fileExtension)
                {
                    case ".pdf":
                        contentType = "application/pdf";
                        break;
                    case ".doc":
                    case ".docx":
                        contentType = "application/msword";
                        break;
                    case ".xls":
                    case ".xlsx":
                        contentType = "application/vnd.ms-excel";
                        break;
                    case ".ppt":
                    case ".pptx":
                        contentType = "application/vnd.ms-powerpoint";
                        break;
                    case ".jpg":
                    case ".jpeg":
                        contentType = "image/jpeg";
                        break;
                    case ".png":
                        contentType = "image/png";
                        break;
                    case ".bmp":
                        contentType = "image/bmp";
                        break;
                    case ".gif":
                        contentType = "image/gif";
                        break;
                    case ".txt":
                        contentType = "text/plain";
                        break;
                    case ".csv":
                        contentType = "text/csv";
                        break;
                    case ".html":
                    case ".htm":
                        contentType = "text/html";
                        break;
                    case ".zip":
                        contentType = "application/zip";
                        break;
                    case ".rar":
                        contentType = "application/x-rar-compressed";
                        break;
                    case ".7z":
                        contentType = "application/x-7z-compressed";
                        break;
                    case ".tar":
                        contentType = "application/x-tar";
                        break;
                    case ".gz":
                        contentType = "application/gzip";
                        break;
                    case ".mp3":
                        contentType = "audio/mpeg";
                        break;
                    case ".wav":
                        contentType = "audio/wav";
                        break;
                    case ".ogg":
                        contentType = "audio/ogg";
                        break;
                    case ".mp4":
                    case ".m4v":
                        contentType = "video/mp4";
                        break;
                    case ".avi":
                        contentType = "video/x-msvideo";
                        break;
                    case ".wmv":
                        contentType = "video/x-ms-wmv";
                        break;
                    case ".flv":
                        contentType = "video/x-flv";
                        break;
                    case ".mov":
                        contentType = "video/quicktime";
                        break;
                    case ".mkv":
                        contentType = "video/x-matroska";
                        break;
                        contentType = "application/pdf";
                        break;
                    case ".ai":
                        contentType = "application/illustrator";
                        break;
                    case ".eps":
                        contentType = "application/postscript";
                        break;
                    case ".psd":
                        contentType = "image/vnd.adobe.photoshop";
                        break;
                    case ".indd":
                        contentType = "application/x-indesign";
                        break;
                    case ".svg":
                        contentType = "image/svg+xml";
                        break;
                    case ".js":
                        contentType = "text/javascript";
                        break;
                    case ".css":
                        contentType = "text/css";
                        break;
                    case ".json":
                        contentType = "application/json";
                        break;
                    case ".xml":
                        contentType = "application/xml";
                        break;
                    default:
                        contentType = "application/octet-stream";
                        break;

                }

                await Task.FromResult(contentType);
                return (contentType);
            }
            catch (Exception)
            {

                return contentType = "application/octet-stream";
            }
        }
    }

    public interface IFilesHelpers
    {


        Task<string> SaveFileToDirectory(IFormFile File, string? FileDirectory);
        Task<IActionResult> ExportToExcel(ControllerBase controller, string FileName, List<dynamic>? DataForExort);
        Task<string> DeleteAnyFileFromDirectory(string FilePath);
        Task<DataTable?> ConvertExcelFirstSheetToDataTable(string FilePath, bool? IwwwrootDirectoryRequired);
        Task<string> GetFileContentTypeForFileExtension(string fileExtension);


    }
}
