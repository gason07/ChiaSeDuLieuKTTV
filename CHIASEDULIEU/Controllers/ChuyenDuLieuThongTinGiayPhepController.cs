using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DULIEUKTTV;
using CHIASEDULIEU.Utilities;

namespace CHIASEDULIEU.Controllers
{
    public class ChuyenDuLieuThongTinGiayPhepController : ApiController
    {
        EntitiesQLKTTV dbContext = new EntitiesQLKTTV();

        //Lấy tất cả thông tin giấy phép
        [HttpGet]
        public IHttpActionResult TatCaGiayPhep()
        {
            var giayPhep = dbContext.ThongTinGiayPheps.Where(gp => gp.IsDeleted == false).AsEnumerable()
            .Select(sp => new 
            {
                ID = sp.ID,
                SoHieu = sp.SoHieu,
                Nam = sp.Nam,
                NgayThang = sp.NgayThang?.ToString("dd/MM/yyyy")
            }).OrderBy(s => s.ID);
            if (giayPhep.Count() > 0)
            {
                return Ok(new
                {
                    success_message = "Lấy tất cả thông tin giấy phép thành công",
                    total = giayPhep.Count(),
                    data = giayPhep
                });
            }
            else
            {
                return Ok(new
                {
                    error_message = "Lấy tất cả thông tin giấy phép không thành công",
                });
            }
        }
    
        //Lấy chi tiết thông tin giấy phép theo ID
        [HttpGet]
        public IHttpActionResult LayThongTinGiayPhepTheoID(int idgp)
        {
            var chiTietGiayPhep = dbContext.ThongTinGiayPheps.Where(s => s.ID == idgp && s.IsDeleted == false).AsEnumerable()
                .Select(sp => new 
                {
                    ID = sp.ID,
                    SoHieu = sp.SoHieu,
                    Nam = sp.Nam,
                    NgayThang = sp.NgayThang?.ToString("dd/MM/yyyy"),
                    TenDonViDuocCapPhep = sp.TenDonVi,
                    PhamViHoatDongDuBao = sp.PhamViHoatDong,
                    DoiTuongCungCap = sp.DonViToChuc.TenDonVi,
                    GiaHan = sp.GiaHan,
                    DonViKyXacNhan = sp.DonViToChuc1.TenDonVi,
                    LoaiCapDo = sp.CapDo.TenLoaiCapDo,
                    TrangThaiGiayPhep = sp.TrangThaiGiayPhep,
                    LyDoThuHoi = sp.LyDoThuHoi,
                    GiayPhepDinhKem = sp.File_DinhKem.AsEnumerable().Select(gpdk => new
                    {
                        ID_GiayPhep = gpdk.ID_GiayPhep,
                        Ten_FileDinhKem = gpdk.Ten_FileDinhKem,
                        DuongDanURL = gpdk.DuongDanURL,
                        NgayTao = gpdk.NgayTao?.ToString("dd/MM/yyyy")
                    }).ToList()    
                }).OrderBy(s => s.ID).ToList();
            if (chiTietGiayPhep.Count() > 0)
            {
                return Ok(new
                {
                    success_message = "Lấy chi tiết thông tin giấy phép thành công",
                    data = chiTietGiayPhep
                });
            }
            else
            {
                return Ok(new
                {
                    error_message = "Lấy chi tiết thông tin giấy phép không thành công",
                });
            }
        }

        //Lấy file giấy phép đi kèm theo ID thông tin giấy phép
        [HttpGet]
        public HttpResponseMessage LayTepDinhKemCuaGiayPhepTheoID(int idtgp)
        {
            EntitiesQLKTTV dbContext = new EntitiesQLKTTV();
            var giayPhepDinhKem = dbContext.File_DinhKem.FirstOrDefault(gp => gp.ID_GiayPhep == idtgp && gp.ISDELETE == false);
            if (giayPhepDinhKem == null)
            {
                //return new HttpResponseMessage(HttpStatusCode.NotFound);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NotFound);
                response.Content = new StringContent("Không tìm thấy giấy phép đính kèm.");
                return response;
            }
            string urlHost = ClsFtp.GetHostFTP();
            string UserName = ClsFtp.GetUserNameFTP();
            string Password = ClsFtp.GetPassWordFTP();
            string folderPath = giayPhepDinhKem.DuongDanURL;
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
            var contentType = MimeMapping.GetMimeMapping(giayPhepDinhKem.Ten_FileDinhKem);
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(ftpStream)
            };
            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = giayPhepDinhKem.Ten_FileDinhKem
                };
            result.Content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            return result;
        }
    }
}
