using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace CHIASEDULIEU.Utilities
{
    public class ClsFtp
    {
        public static string GetUserNameFTP()
        {
            string username = ConfigurationManager.AppSettings["FTPUser"];
            return username;
        }
        public static string GetPassWordFTP()
        {
            string password = ConfigurationManager.AppSettings["FTPPassword"];
            return password;
        }
        public static string GetHostFTP()
        {
            string host = ConfigurationManager.AppSettings["FTPHost"];
            return host;
        }
        /// <summary>
        /// Upload file "sFilePath" từ webserver lên ftpserver với tên mới sFileName và được lưu trong thư mục mới sFolder
        /// </summary>
        /// <param name="urlHost">đường dẫn đến host => nó chỉ đến thư mục ftp trên fptserver</param>
        /// <param name="sFolder">thư mục lưu trong thư mục ftp</param>
        /// <param name="sFileName">tên file lưu trong ftp</param>
        /// <param name="sFilePath">đường dẫn+ tên file nguồn</param>
        /// <param name="UserName">username đăng nhập ftp</param>
        /// <param name="Password">password đăng nhập ftp</param>
        /// <returns></returns>
        public static bool UploadFile(string urlHost, string sFolder, string sFileName, string sFilePath, string UserName, string Password)
        {

            // Get the object used to communicate with the server.
            var request = (FtpWebRequest)WebRequest.Create(urlHost + @"/" + sFolder + @"/" + sFileName);
            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(UserName, Password);
            request.KeepAlive = true;

            request.Method = WebRequestMethods.Ftp.UploadFile;

            // Copy the contents of the file to the request stream.

            FileInfo fileInfo = new FileInfo(sFilePath);
            request.UseBinary = true;

            request.ContentLength = fileInfo.Length;

            int BufferLength = 204800000;
            byte[] buffer = new byte[BufferLength];
            int ContentLength;

            FileStream FS = fileInfo.OpenRead();

            try
            {
                Stream stream = request.GetRequestStream();
                ContentLength = FS.Read(buffer, 0, BufferLength);
                while (ContentLength != 0)
                {
                    stream.Write(buffer, 0, ContentLength);
                    ContentLength = FS.Read(buffer, 0, BufferLength);
                }
                stream.Close();
                stream.Dispose();
            }
            catch (Exception ex)
            {
                return false;
            }
            FS.Close();
            FS.Dispose();
            return true;
        }

        public static bool DownloadFileFromServer(string PathTemp, string strFile, string TenFile)
        {
            try
            {
                string NameFile = strFile.Split('/').Last();
                string[] s = NameFile.Split('.');
                string strUser = ClsFtp.GetUserNameFTP();
                string strPWD = ClsFtp.GetPassWordFTP();
                string hostFtpName = GetHostFTP();

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(hostFtpName + strFile);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(strUser, strPWD);
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();


                // note: since you are writing directly to client, I removed the `file` stream in your original code since we don't need to store the file locally... or so I am assuming
                //HttpResponse Response = HttpContext.Current.Response;
                //Response.ContentType = "application/octet-stream";
                //Response.Clear();
                //Response.ClearContent();
                //Response.ClearHeaders();
                //Response.Buffer = true;
                //Response.AddHeader("content-disposition", "attachment;filename=" + TenFile + "." + s[s.Count()-1]);

                //byte[] buffer = new byte[2 * 1024];
                //int read;
                //while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                //{
                //    Response.OutputStream.Write(buffer, 0, read);
                //}
                HttpResponse Response = HttpContext.Current.Response;
                Response.ContentType = "application/octet-stream";
                Response.ClearContent();
                Response.ClearHeaders();
                Response.BufferOutput = true;
                Response.AddHeader("content-disposition", "attachment;filename=" + TenFile + "." + s[s.Count() - 1]);
                byte[] buffer = new byte[2 * 1024];
                int read;
                while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    Response.OutputStream.Write(buffer, 0, read);
                }
                Response.Flush();
                Response.Close();
                Response.End();

                responseStream.Close();
                response.Close();
                return true;
            }
            catch (Exception ex)
            {
                if (ex.Message == "The remote server " +
                               "returned an error: (404) Not Found.")
                    throw new Exception("File not found");
                else if (ex.Message == "The remote server" +
                        " returned an error: (401) Unauthorized.")
                    throw new Exception("Unauthorized access");

                return false;
            }
        }

        public static bool Download(string sPathSaveFile, string sFileName, string sFilePath, string UserName, string Password)
        {
            bool bReturn = true;

            FtpWebRequest request;
            FileStream fileStream = null;
            FtpWebResponse response;
            Stream readStream;

            try
            {
                fileStream = new FileStream(sPathSaveFile, FileMode.Create);

                request = (FtpWebRequest)FtpWebRequest.Create(new Uri(sFilePath));
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.UseBinary = true;
                request.Credentials = new NetworkCredential(UserName, Password);
                response = (FtpWebResponse)request.GetResponse();

                readStream = response.GetResponseStream();
                try
                {
                    int bufferSize = 2048;
                    int readCount;
                    byte[] buffer = new byte[bufferSize];

                    readCount = readStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        fileStream.Write(buffer, 0, readCount);
                        readCount = readStream.Read(buffer, 0, bufferSize);
                    }
                }
                catch { }
                readStream.Close();
                fileStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                if (fileStream != null)
                    fileStream.Close();
                bReturn = false;

            }

            return bReturn;

        }

        public static bool CreateFolder(string urlHost, string FolderName, string UserName, string Password)
        {
            bool result = true;
            try
            {
                // Step 1 - Open a request using the full URI               
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(urlHost + @"/" + FolderName);
                // Step 2 - Configure the connection request
                request.Credentials = new NetworkCredential(UserName, Password);
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;

                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                // Step 3 - Call GetResponse() method to actually attempt to create the directory
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();

                stream.Close();
                response.Close();
            }
            catch (WebException e)
            {
                result = false;
                String status = ((FtpWebResponse)e.Response).StatusDescription;
            }

            return result;
        }
        /// <summary>
        /// this function always return true
        /// </summary>
        public static bool CheckExistsFolder(string urlHost, string FolderName, string UserName, string Password)
        {
            bool IsExists = false;

            try
            {
                //string DirectoryPath = urlHost + @"/" + FolderName;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(urlHost + @"/" + FolderName + "/");
                request.Credentials = new NetworkCredential(UserName, Password);
                request.Method = WebRequestMethods.Ftp.PrintWorkingDirectory;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();

                stream.Close();
                response.Close();
            }
            catch
            {
                IsExists = true;
                /*
                if (ex.Response != null)  
                {  
                    FtpWebResponse response = (FtpWebResponse)ex.Response;  
                    if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)  
                    {
                        IsExists = false; 
                    }  
                } */
            }
            return IsExists;

        }

        public static bool DeleteFileFTP(string FilePath)
        {
            bool IsDelete = true;
            try
            {
                FtpWebRequest requestFileDelete = (FtpWebRequest)WebRequest.Create(GetHostFTP() + FilePath);
                requestFileDelete.Credentials = new NetworkCredential(GetUserNameFTP(), GetPassWordFTP());
                requestFileDelete.Method = WebRequestMethods.Ftp.DeleteFile;

                FtpWebResponse responseFileDelete = (FtpWebResponse)requestFileDelete.GetResponse();
            }
            catch (Exception)
            {
                IsDelete = false;
            }
            return IsDelete;
        }
        public static bool DeleteFile(string FilePath, string UserName, string Password)
        {
            bool IsDelete = true;
            try
            {
                FtpWebRequest requestFileDelete = (FtpWebRequest)WebRequest.Create(FilePath);
                requestFileDelete.Credentials = new NetworkCredential(UserName, Password);
                requestFileDelete.Method = WebRequestMethods.Ftp.DeleteFile;

                FtpWebResponse responseFileDelete = (FtpWebResponse)requestFileDelete.GetResponse();
            }
            catch (Exception)
            {
                IsDelete = false;
            }
            return IsDelete;
        }

        public static void OpenFile(string sFilePath)
        {
            System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo();
            processStartInfo.FileName = sFilePath;
            processStartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            System.Diagnostics.Process.Start(processStartInfo);
        }
        /// <summary>
        /// Đổi tên file
        /// </summary>
        /// <param name="OldName">Tên Cũ</param>
        /// <param name="NewName">Tên Mới</param>
        /// <returns>True nếu đổi tên thành công</returns>
        public bool Rename(string OldName, string NewName, string FilePath, string UserName, string Password)
        {
            bool bReturn = true;
            FtpWebRequest request;
            try
            {
                request = (FtpWebRequest)FtpWebRequest.Create(new Uri(FilePath + OldName));
                request.Method = WebRequestMethods.Ftp.Rename;
                request.RenameTo = NewName;
                request.UseBinary = true;
                request.Credentials = new NetworkCredential(UserName, Password);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();

                stream.Close();
                response.Close();
            }
            catch
            {
                bReturn = false;
            }
            return bReturn;
        }
        /// <summary>
        /// Tạo tên file từ Username và ngày giờ phút hiện tại
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns>Tên file mới tạo</returns>
        public static string SetName(string UserName)
        {
            string NameVBD = string.Empty;
            NameVBD = UserName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            return NameVBD;
        }
        /// <summary>
        /// Thư mục lưu file theo lĩnh vực Và tháng lưu file eg: LinhVuc/201209
        /// </summary>
        /// <param name="linhvuc">Tên lĩnh vực</param>
        /// <returns></returns>
        public static string getFtpFolderPath(string linhvuc)
        {

            return linhvuc + @"/" + DateTime.Now.ToString("yyyyMM");

        }

        /// <summary>
        /// Hàm đưa File từ client lên ftpserver
        /// Tạo thư mục theo IDKho/IDKe/IDNgan/IDHop/IDHoSo/TenHS.pdf
        /// Đưa File lên FTP
        /// </summary>
        /// <param name="sourceFolderPath">Thư mục chứa fileUpload</param>
        /// <param name="filFileUploadName">Tên fileUploas</param>
        /// <param name="list_TenDuongDan">List IDKho/IDKe/IDNgan/IDHop/IDHoSo </param>
        /// <param name="tenfile">Tên file mới</param>
        /// <returns>trả về đường dẫn download file từ ftp nếu up thành công, errorString có "Error:" nếu lỗi</returns>

        public static string uploadToFtp(string sourceFolderPath, string filFileUploadName, List<string> list_TenDuongDan, string tenfile)
        {

            string urlHost = "";
            string strError = "";
            string UserName = "";
            string Password = "";
            string newFileName;
            string desFolderPath;
            string duongdantam = "";
            string duongserver = "";
            try
            {

                try
                {

                    urlHost = GetHostFTP();
                    UserName = GetUserNameFTP();
                    Password = GetPassWordFTP();

                }
                catch (Exception ex)
                {
                    return "Error: " + ex.Message;
                }

                //----upload lên web server
                if (filFileUploadName != null)// nếu không có lỗi
                {
                    foreach (var i in list_TenDuongDan)
                    {
                        duongdantam = duongdantam + "\\" + i;
                    }

                    foreach (var i in list_TenDuongDan)
                    {
                        duongserver = duongserver + @"/" + i;
                    }
                    desFolderPath = duongserver;
                    newFileName = tenfile + Path.GetExtension(filFileUploadName);


                    string folderExist = "";
                    string folderPath = "";

                    //-----tạo thư mục chứa file
                    foreach (var i in list_TenDuongDan)
                    {
                        folderPath = folderExist + "/" + i;
                        if (Utilities.ClsFtp.CheckExistsFolder(urlHost, folderPath, UserName, Password) == false)
                        {
                            Utilities.ClsFtp.CreateFolder(urlHost, folderPath, UserName, Password);
                            folderExist = folderExist + @"/" + i;
                        }
                        else
                        {
                            //Trường hợp đã có folder rồi thì phải lấy địa chỉ folder đó
                            folderExist = folderExist + @"/" + i;
                        }
                    }

                    //-----upload to ftpserver
                    if (Utilities.ClsFtp.UploadFile(urlHost, desFolderPath, newFileName, sourceFolderPath + @"File" + duongdantam + "\\" + filFileUploadName, UserName, Password))
                        //foreach (string file in System.IO.Directory.GetFiles(sourceFolderPath + @"File\Upload\" + nguoidung + "\\"))
                        System.IO.File.Delete(sourceFolderPath + @"File" + duongdantam + "\\" + filFileUploadName);

                }
                else
                    return strError;
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
            return /*"" + urlHost + @"/" + */desFolderPath + @"/" + newFileName;
        }
        /// <summary>
        /// Hàm tạo thư mục
        /// IDTenKho/IDKe/IDNgan/IDHop/IDHoSo
        /// Cody by: N.T.H.Thanh 
        /// Date :20130709
        /// </summary>
        /// <param name="list_TenDuongDanMoi"></param>
        /// <returns></returns>
        public static string CreatePathSaveFile(List<int> list_TenDuongDanMoi)
        {
            string DuongDanMoi = "";
            string urlHost = "";
            string UserName = "";
            string Password = "";
            string desFolderPath;
            string duongserver = "";

            //---B1: kết nối ftpserver
            try
            {

                urlHost = GetHostFTP();
                UserName = GetUserNameFTP();
                Password = GetPassWordFTP();

            }
            catch (Exception ex)
            {
                string loi = ex.Message;
            }
            //---B2: kiểm tra coi đường dẫn cũ có file hay không sau đó mới tạo thư mục theo đường dẫn mới
            //----upload lên web server

            foreach (int i in list_TenDuongDanMoi)
            {
                duongserver = duongserver + @"/" + i;
            }
            desFolderPath = @"ftp_giayphephoatdong" + duongserver;

            string folderExist = "";
            string folderPath = "";

            //-----tạo các thư mục chứa file
            foreach (int i in list_TenDuongDanMoi)
            {
                folderPath = @"/ftp_giayphephoatdong" + folderExist + "/" + i;
                if (Utilities.ClsFtp.CheckExistsFolder(urlHost, folderPath, UserName, Password) == false)
                {
                    Utilities.ClsFtp.CreateFolder(urlHost, folderPath, UserName, Password);
                    folderExist = folderExist + @"/" + i;
                }
                else
                {
                    //Trường hợp đã có folder rồi thì phải lấy địa chỉ folder đó
                    folderExist = folderExist + @"/" + i;
                }
            }
            //-----B3 Copy file từ thư mục cũ sang thư mục mới                 
            DuongDanMoi = urlHost + @"/" + desFolderPath;
            return DuongDanMoi;
        }

        /// <summary>
        /// Hàm chuyển đổi file đính kèm khi chuyển kho ngăn kệ
        /// Code by: N.T.H.Thanh
        /// Date:20130705
        /// </summary>
        /// <param name="PathOld">đường dẫn thư mục cũ</param>
        /// <param name="PathNew">đường dẫn thư mục mới</param>
        /// <param name="eQLKho"></param>
        public static bool ChangeFileFTP(string DuongDanCu, List<int> list_TenDuongDanMoi, out string DuongDanMoiOK)
        {
            //truy cập vào đường dẫn thư mục cũ sau đó copy vào đường dẫn thư mục mới,
            //nếu copy thành công thì xóa file đó (kiểm tra thư mục chứa mà ko có dữ liệu thì xóa luôn thư mục) thông báo 
            bool Result_Copy = false;
            string urlHost = "";
            string UserName = "";
            string Password = "";
            string desFolderPath;
            string duongserver = "";
            DuongDanMoiOK = "";
            try
            {
                //---B1: kết nối ftpserver
                try
                {

                    urlHost = GetHostFTP();
                    UserName = GetUserNameFTP();
                    Password = GetPassWordFTP();

                }
                catch (Exception ex)
                {
                    Result_Copy = false; //lỗi kết nối đến ftp
                }
                //---B2: kiểm tra coi đường dẫn cũ có file hay không sau đó mới tạo thư mục theo đường dẫn mới
                //----upload lên web server             

                if (FTPFileExists(DuongDanCu, UserName, Password) == true)// viết hàm kiểm tra file cũ coi có hay không
                {
                    foreach (int i in list_TenDuongDanMoi)
                    {
                        duongserver = duongserver + @"/" + i;
                    }
                    desFolderPath = @"ftp_giayphephoatdong" + duongserver;
                    // newFileName = tenfile;


                    string folderExist = "";
                    string folderPath = "";

                    //-----tạo các thư mục chứa file
                    foreach (int i in list_TenDuongDanMoi)
                    {
                        folderPath = @"/ftp_giayphephoatdong" + folderExist + "/" + i;
                        if (Utilities.ClsFtp.CheckExistsFolder(urlHost, folderPath, UserName, Password) == false)
                        {
                            Utilities.ClsFtp.CreateFolder(urlHost, folderPath, UserName, Password);
                            folderExist = folderExist + @"/" + i;
                        }
                        else
                        {
                            //Trường hợp đã có folder rồi thì phải lấy địa chỉ folder đó
                            folderExist = folderExist + @"/" + i;
                        }
                    }

                    //-----B3 Copy file từ thư mục cũ sang thư mục mới
                    string TenFile = DuongDanCu.Split('/').Last();
                    string DuongDanMoi = urlHost + @"/" + desFolderPath + @"/" + TenFile;

                    if (Utilities.ClsFtp.CopyFile(DuongDanCu, DuongDanMoi, TenFile, UserName, Password) == true)
                    {
                        Result_Copy = true;
                        //---B4 Copy thành công thì thực hiện xóa file 
                        Utilities.ClsFtp.DeleteFile(DuongDanCu, UserName, UserName);
                        DuongDanMoiOK = DuongDanMoi;

                    }

                }
                else
                    Result_Copy = false;

            }
            catch (Exception ex)
            {
                Result_Copy = false;
            }

            return Result_Copy;



        }
        /// <summary>
        /// Hàm kiểm tra file tồn tại trên FTP
        /// Code by: N.T.H.Thanh
        /// date 20130709
        /// </summary>
        /// <param name="sourceFilename">đường dẫn ftp</param>
        /// <param name="ftpUsername"></param>
        /// <param name="ftpPassword"></param>
        /// <returns></returns>

        public static bool FTPFileExists(string sourceFilename, string ftpUsername, string ftpPassword)
        {

            bool _fileExists = false;

            // System.Uri _targetURI = new System.Uri("ftp://" + address + @"/" + sourceFilename);
            System.Uri _targetURI = new System.Uri(sourceFilename);
            FtpWebRequest req = (FtpWebRequest)FtpWebRequest.Create(_targetURI);
            req.Credentials = new NetworkCredential(ftpUsername,
                                                 ftpPassword);


            req.UsePassive = false;
            req.KeepAlive = false;

            req.Method = WebRequestMethods.Ftp.GetFileSize;

            try
            {
                FtpWebResponse wr = (FtpWebResponse)req.GetResponse();
                if (wr.StatusCode == FtpStatusCode.CommandOK
                     || wr.StatusCode == FtpStatusCode.ClosingData
                     || wr.StatusCode == FtpStatusCode.ClosingControl
                     || wr.StatusCode == FtpStatusCode.ConnectionClosed
                     || wr.StatusCode == FtpStatusCode.FileActionOK
                     || wr.StatusCode == FtpStatusCode.FileStatus)
                {
                    _fileExists = true;
                }
                else
                {
                    System.Diagnostics.Trace.TraceError(string.Concat(sourceFilename, " ftp status  code: ", wr.StatusCode.ToString()));
                    _fileExists = false;
                }
            }
            catch (Exception ex)
            {
                // If any error occured then the file effectively doesn't exist
                System.Diagnostics.Trace.TraceError(ex.ToString());
                _fileExists = false;
            }

            return _fileExists;
        }
        /// <summary>
        /// Hàm Copy File với đường dẫn FTP
        /// </summary>
        /// <param name="DuongDanCu">đường dẫn file cũ + tên file cũ</param>
        /// <param name="DuongDanMoi">đường dẫn file mới + tên file mới</param>
        /// <param name="fileName">Tên file</param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool CopyFile(string DuongDanCu, string DuongDanMoi, string fileName, string userName, string password)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(DuongDanCu);
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                request.Credentials = new NetworkCredential(userName, password);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                Upload(DuongDanMoi, ToByteArray(responseStream), userName, password);
                responseStream.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Byte[] ToByteArray(Stream stream)
        {
            MemoryStream ms = new MemoryStream();
            byte[] chunk = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(chunk, 0, chunk.Length)) > 0)
            {
                ms.Write(chunk, 0, bytesRead);
            }

            return ms.ToArray();
        }

        public static bool Upload(string FileName, byte[] Image, string FtpUsername, string FtpPassword)
        {
            try
            {
                System.Net.FtpWebRequest clsRequest = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(FileName);
                clsRequest.Credentials = new System.Net.NetworkCredential(FtpUsername, FtpPassword);
                clsRequest.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
                System.IO.Stream clsStream = clsRequest.GetRequestStream();
                clsStream.Write(Image, 0, Image.Length);

                clsStream.Close();
                clsStream.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Download trực tiếp file từ ftp vể browser.
        /// Chú ý không đặt nút download trong Update panel
        /// </summary>
        /// <param name="ftpServer"></param>
        /// <param name="ftpUsername"></param>
        /// <param name="ftpPassword"></param>
        /// <param name="strHostFilePath"></param>
        /// <param name="strDownloadFile"></param>
        /// <returns></returns>
        public static bool DownloadFromFtpServer(string ftpServer, string ftpUsername, string ftpPassword, string strHostFilePath, string strDownloadFile)
        {
            try
            {
                //string strDownloadURL = strFile;
                string strFile = strHostFilePath;
                string strUser = ftpUsername;
                string strPWD = ftpPassword;

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpServer + strFile);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(strUser, strPWD);
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream(); // -dùng để đọc file stream from ftp server
                HttpResponse httpResponse = HttpContext.Current.Response;
                httpResponse.ContentType = "application/octet-stream";
                httpResponse.Clear();
                httpResponse.ClearContent();
                httpResponse.ClearHeaders();
                httpResponse.Buffer = true;
                httpResponse.AddHeader("content-disposition", "attachment;filename=" + strDownloadFile);

                byte[] buffer = new byte[2 * 1024];
                int read;
                while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    httpResponse.OutputStream.Write(buffer, 0, read); // Không ghi file vào web server và dùng HttpResponse ghi vào trigger the download dialog in the browser 
                }
                responseStream.Close();
                response.Close();
                //httpResponse.Flush();
                //httpResponse.End();
                return true;

            }
            catch (Exception ex)
            {
                if (ex.Message == "The remote server " +
                               "returned an error: (404) Not Found.")
                    throw new Exception("File not found");
                else if (ex.Message == "The remote server" +
                        " returned an error: (401) Unauthorized.")
                    throw new Exception("Unauthorized access");

                return false;
            }
        }

        public static bool DownloadFtpToWebserver(string sPathSaveFile, string sFileName, string sFilePath)
        {
            bool bReturn = true;

            string UserName = GetUserNameFTP();
            string Password = GetPassWordFTP();
            string hostName = GetHostFTP();
            using (WebClient request = new WebClient())
            {
                request.Credentials = new NetworkCredential(UserName, Password);
                byte[] fileData = request.DownloadData(hostName + sFileName);

                using (FileStream file = File.Create(sPathSaveFile))
                {
                    file.Write(fileData, 0, fileData.Length);
                    file.Close();
                }
                //MessageBox.Show("Download Complete");
            }

            return bReturn;

        }
        public static void TransmitFileToBrowser(string sFileName)
        {
            string spathfolder = "";
            try
            {
                var file = new FileInfo(sFileName);
                spathfolder = file.DirectoryName;
                System.Web.HttpContext context = System.Web.HttpContext.Current;
                context.Response.ClearContent();
                //context.Response.AppendHeader("Content-Disposition", "attachment; filename=" + file.Name.Trim());
                context.Response.AppendHeader("Content-Disposition", "inline; filename=" + file.Name.Trim());
                context.Response.AppendHeader("Content-Length", file.Length.ToString());
                string fExtn = GetMimeTypeByFileName(sFileName);
                context.Response.ContentType = fExtn;
                //context.Response.TransmitFile(sFileName);
                context.Response.WriteFile(sFileName);
                context.Response.Flush();
                context.Response.End();
            }
            finally
            {
                File.Delete(sFileName);
                //if (Directory.Exists(spathfolder)) { Directory.Delete(spathfolder); }
            }
        }
        public static string GetMimeTypeByFileName(string sFileName)
        {
            string sMime = "application/octet-stream";

            string sExtension = System.IO.Path.GetExtension(sFileName);
            if (!string.IsNullOrEmpty(sExtension))
            {
                sExtension = sExtension.Replace(".", "");
                sExtension = sExtension.ToLower();
                switch (sExtension)
                {
                    case "htm":
                    case "html":
                    case "log":
                        return "text/HTML";

                    case "txt":
                        return "text/plain";


                    case "doc":
                        return "application/ms-word";

                    case "tiff":
                    case "tif":
                        return "image/tiff";

                    case "asf":
                        return "video/x-ms-asf";

                    case "avi":
                        return "video/avi";

                    case "zip":
                        return "application/zip";

                    case "xls":
                    case "csv":
                        return "application/vnd.ms-excel";

                    case "gif":
                        return "image/gif";

                    case "jpg":
                    case "jpeg":
                        return "image/jpeg";

                    case "bmp":
                        return "image/bmp";

                    case "wav":
                        return "audio/wav";

                    case "mp3":
                        return "audio/mpeg3";

                    case "mpg":
                    case "mpeg":
                        return "video/mpeg";

                    case "rtf":
                        return "application/rtf";

                    case "asp":
                        return "text/asp";

                    case "pdf":
                        return "application/pdf";

                    case "fdf":
                        return "application/vnd.fdf";

                    case "ppt":
                        return "application/mspowerpoint";

                    case "dwg":
                        return "image/vnd.dwg";

                    case "msg":
                        return "application/msoutlook";

                    case "xml":
                    case "sdxl":
                        return "application/xml";

                    case ".xdp":
                        return "application/vnd.adobe.xdp+xml";

                    default:
                        return "application/octet-stream";
                }
            }
            return sMime;
        }
    }
}