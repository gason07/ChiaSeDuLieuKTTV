using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DULIEUKTTV;
using CHIASEDULIEU.Utilities;
using System.IO;

namespace CHIASEDULIEU.Controllers
{
    public class LayDuLieuBanDoPhimAnhController : ApiController
    {
        EntitiesQLKTTV dbContext = new EntitiesQLKTTV();

        //Lấy tất cả dữ liệu phim, ảnh
        [HttpGet]
        public IHttpActionResult LayThongTinPhimAnh()
        {
            var phimAnh = dbContext.PhimHinhAnhs.Where(pa => pa.IsDeleted == false).AsEnumerable()
                .Select(sp => new 
                {
                    ID = sp.ID,
                    TieuDe = sp.TieuDe
                });
            if (phimAnh.Count() > 0)
            {
                return Ok(new
                {
                    success_message = "Lấy tất cả thông tin phim ảnh thành công",
                    total = phimAnh.Count(),
                    data = phimAnh
                });
            }
            else
            {
                return Ok(new
                {
                    error_message = "Lấy tất cả thông tin phim ảnh không thành công",
                });
            }
        }

        //Lấy thông tin chi tiết phim, ảnh theo ID
        [HttpGet]
        public IHttpActionResult LayThongTinChiTietPhimAnhTheoID(int idpa)
        {
            var chiTietPhimAnh = dbContext.PhimHinhAnhs.Where(pa => pa.ID == idpa && pa.IsDeleted == false).AsEnumerable()
                  .Select(sp => new 
                  {
                      ID = sp.ID,
                      TieuDe = sp.TieuDe,
                      NgayThang = sp.NgayThang?.ToString("dd/MM/yyyy"),
                      TenTapTin = sp.TenTapTin,
                      MoTa = sp.MoTa,
                      GhiChu = sp.GhiChu,
                      DuongDanURL = sp.DuongDanURL,
                      ThoiGianCapNhat = sp.ThoiGianCapNhat?.ToString("dd/MM/yyyy HH:mm:ss")
                  });
            if (chiTietPhimAnh.Count() > 0)
            {
                return Ok(new
                {
                    success_message = "Lấy chi tiết thông tin phim, ảnh thành công",
                    data = chiTietPhimAnh
                });
            }
            else
            {
                return Ok(new
                {
                    error_message = "Lấy chi tiết thông tin phim, ảnh không thành công",
                });
            }
        }

        //Xem file đính kèm của phim, ảnh
        [HttpGet]
        public HttpResponseMessage LayTepDinhKemCuaPhimAnhTheoID(int idtpa)
        {
            EntitiesQLKTTV dbContext = new EntitiesQLKTTV();
            var phimAnhDinhKem = dbContext.PhimHinhAnhs.FirstOrDefault(s => s.ID == idtpa && s.IsDeleted == false);
            if (phimAnhDinhKem == null)
            {
                //return new HttpResponseMessage(HttpStatusCode.NotFound);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NotFound);
                response.Content = new StringContent("Không tìm thấy phim/ảnh đính kèm.");
                return response;
            }
            string urlHost = ClsFtp.GetHostFTP();
            string UserName = ClsFtp.GetUserNameFTP();
            string Password = ClsFtp.GetPassWordFTP();
            string folderPath = phimAnhDinhKem.DuongDanURL;
            var ftpRequest = (FtpWebRequest)WebRequest.Create(ClsFtp.GetHostFTP() + folderPath);
            ftpRequest.Credentials = new NetworkCredential(ClsFtp.GetUserNameFTP(), ClsFtp.GetPassWordFTP());
            ftpRequest.UseBinary = true;
            ftpRequest.UsePassive = true;
            ftpRequest.KeepAlive = true;
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            var ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
            if (ftpResponse.StatusCode == FtpStatusCode.CommandOK && ftpResponse.StatusCode != FtpStatusCode.ClosingData)
            {
                HttpResponseMessage errorResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                errorResponse.Content = new StringContent("Không kết nối được với máy chủ FTP.");
                return errorResponse;
            }
            var ftpStream = ftpResponse.GetResponseStream();
            var contentType = MimeMapping.GetMimeMapping(phimAnhDinhKem.TenTapTin);
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(ftpStream)
            };
            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = phimAnhDinhKem.TenTapTin
                };
            result.Content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            return result;
            // Đọc dữ liệu từ stream và chuyển thành mảng byte
            //using (MemoryStream memoryStream = new MemoryStream())
            //{
            //    ftpStream.CopyTo(memoryStream);
            //    byte[] imageBytes = memoryStream.ToArray();

            //    // Chuyển đổi mảng byte thành chuỗi Base64
            //    string base64Image = Convert.ToBase64String(imageBytes);

            //    HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            //    result.Content = new StringContent(base64Image);

            //    // Thiết lập loại nội dung cho phản hồi HTTP
            //    result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream"); // Thay đổi thành loại ảnh thích hợp

            //    return result;
            //}
        }

        //Lấy tất cả bản đồ
        [HttpGet]
        public IHttpActionResult LayThongTinBanDo()
        {
            var banDo = dbContext.BanDoes.Where(map => map.IsDeleted == false).AsEnumerable()
            .Select(sp => new 
            {
                ID = sp.ID,
                TenBanDo = sp.TenBanDo,
                ThoiGianCapNhat = sp.ThoiGianCapNhat?.ToString("dd/MM/yyyy HH:mm:ss")
            });
            if (banDo.Count() > 0)
            {
                return Ok(new
                {
                    success_message = "Lấy tất cả thông tin bản đồ thành công",
                    total = banDo.Count(),
                    data = banDo
                });
            }
            else
            {
                return Ok(new
                {
                    error_message = "Lấy tất cả thông tin bản đồ không thành công",
                });
            }
        }
    

        //Lấy thông tin chi tiết bản đồ theo ID
        [HttpGet]
        public IHttpActionResult LayThongTinBanDoTheoID(int idbd)
        {
            var chiTietBanDo = dbContext.BanDoes.Where(map => map.ID == idbd && map.IsDeleted == false).AsEnumerable()
            .Select(sp => new 
            {
                ID = sp.ID,
                TenBanDo = sp.TenBanDo,
                LoaiBanDo = sp.LoaiBanDo,
                TenTapTin = sp.TenTapTin,
                MoTa = sp.MoTa,
                DuongDan = sp.DuongDanURL,
                ThoiGianCapNhat = sp.ThoiGianCapNhat?.ToString("dd/MM/yyyy HH:mm:ss")
            });
            if (chiTietBanDo.Count() > 0)
            {
                return Ok(new
                {
                    success_message = "Lấy chi tiết thông tin bản đồ thành công",
                    data = chiTietBanDo
                });
            }
            else
            {
                return Ok(new
                {
                    error_message = "Lấy chi tiết thông tin bản đồ không thành công",
                });
            }
        }

        //Xem file đính kèm của bản đồ
        [HttpGet]
        public HttpResponseMessage LayTepDinhKemCuaBanDoTheoID(int idtbd)
        {
            EntitiesQLKTTV dbContext = new EntitiesQLKTTV();
            var banDoDinhKem = dbContext.BanDoes.FirstOrDefault(bd => bd.ID == idtbd && bd.IsDeleted == false);
            if (banDoDinhKem == null)
            {
                //return new HttpResponseMessage(HttpStatusCode.NotFound);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NotFound);
                response.Content = new StringContent("Không tìm thấy bản đồ đính kèm.");
                return response;
            }
            string urlHost = ClsFtp.GetHostFTP();
            string UserName = ClsFtp.GetUserNameFTP();
            string Password = ClsFtp.GetPassWordFTP();
            string folderPath = banDoDinhKem.DuongDanURL;
            var ftpRequest = (FtpWebRequest)WebRequest.Create(ClsFtp.GetHostFTP() + folderPath);
            ftpRequest.Credentials = new NetworkCredential(ClsFtp.GetUserNameFTP(), ClsFtp.GetPassWordFTP());
            ftpRequest.UseBinary = true;
            ftpRequest.UsePassive = true;
            ftpRequest.KeepAlive = true;
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            var ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
            if (ftpResponse.StatusCode == FtpStatusCode.CommandOK && ftpResponse.StatusCode != FtpStatusCode.ClosingData)
            {
                HttpResponseMessage errorResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                errorResponse.Content = new StringContent("Không kết nối được với máy chủ FTP.");
                return errorResponse;
            }
            var ftpStream = ftpResponse.GetResponseStream();
            var contentType = MimeMapping.GetMimeMapping(banDoDinhKem.TenTapTin);
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(ftpStream)
            };
            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = banDoDinhKem.TenTapTin
                };
            result.Content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            return result;
        }

    }
}
