using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using CHIASEDULIEU.Models.BanTinDuBao;
using DULIEUKTTV;
using CHIASEDULIEU.Utilities;


namespace CHIASEDULIEU.Controllers
{
    public class LayDuLieuBanTinDuBaoController : ApiController
    {
        EntitiesQLKTTV dbContext = new EntitiesQLKTTV();

        //Lấy tất cả bản tin dự báo
        [HttpGet]
        public IHttpActionResult TatCaBanTinDuBao()
        {
            var tatCaBanTin = dbContext.BanTinDuBaos.Where(s => s.IsDeleted == false).AsEnumerable()
                .Select(sp => new 
                {
                    ID = sp.ID,
                    TenBanTin = sp.LoaiBanTin.TenLoaiBanTin,
                    NgayThucHien = sp.NgayThucHien?.ToString("dd/MM/yyyy HH:mm:ss")
                }).OrderBy(s => s.ID);
            if (tatCaBanTin.Count() > 0)
            {
                return Ok(new
                {
                    success_message = "Lấy danh sách tất cả bản tin thành công",
                    total = tatCaBanTin.Count(),
                    data = tatCaBanTin
                });
            }
            else
            {
                return Ok(new
                {
                    error_message = "Lấy danh sách tất cả bản tin không thành công",
                });
            }
        }

        //Lấy danh sách bản tin dựa theo tên bản tin
        [HttpGet]
        public IHttpActionResult TimKiemBanTinDuaTheoTenBanTin(string tenBanTin)
        {
            // Chuyển tên bản tin thành chữ thường và loại bỏ khoảng trống ở cả hai đầu
            tenBanTin = tenBanTin.Trim().ToLower();
            // Truy vấn
            var banTinTheoTen = dbContext.BanTinDuBaos.Where(s => s.LoaiBanTin.TenLoaiBanTin.ToLower().Trim().Contains(tenBanTin) && s.IsDeleted == false).AsEnumerable()
                .Select(sp => new 
                {
                    ID = sp.ID,
                    TenBanTin = sp.LoaiBanTin.TenLoaiBanTin,
                    NgayThucHien = sp.NgayThucHien?.ToString("dd/MM/yyyy HH:mm:ss")
                });
            if (banTinTheoTen.Count() > 0)
            {
                return Ok(new
                {
                    success_message = "Lấy danh sách tất cả bản tin thành công",
                    total = banTinTheoTen.Count(),
                    data = banTinTheoTen
                });
            }
            else
            {
                return Ok(new
                {
                    error_message = "Lấy danh sách tất cả bản tin không thành công",
                });
            }
        }

        //Lấy chi tiết bản tin dự báo theo ID
        [HttpGet]
        public IHttpActionResult LayChiTietBanTinDuBaoTheoID(int idbt)
        {
            var bantins = dbContext.BanTinDuBaos.Where(s => s.ID == idbt && s.IsDeleted == false).AsEnumerable().Select(sp => new
                 {
                    ID = sp.ID,
                    TieuDe = HttpUtility.HtmlDecode(sp.TieuDe),
                    TomTatNoiDung = HttpUtility.HtmlDecode(sp.TomTatNoiDung),
                    NoiDung = HttpUtility.HtmlDecode(sp.NoiDung),
                    NgayThucHien = sp.NgayThucHien?.ToString("dd/MM/yyyy HH:mm:ss"),
                    NguoiThucHien = HttpUtility.HtmlDecode(sp.NguoiDang),
                    TenBanTin = sp.LoaiBanTin.TenLoaiBanTin,
                    //ThongTinDiaDiem = sp.KhuVucHanhChinh.TenKVHC,
                    KhuVucDuBao = sp.KhuVuc.TenKhuVuc,
                    ThoiGianDuBao = sp.ThoiHanDuBao.TuThoiGian?.ToString("dd/MM/yyyy") + " - " + sp.ThoiHanDuBao.DenThoiGian?.ToString("dd/MM/yyyy"),
                    CapDoDuBao = sp.CapDo.TenLoaiCapDo,
                    LoaiThienTai = sp.LoaiThienTai.TenLoaiThienTai,
                    //TenToChucDuBao = sp.TenToChucDuBao,
                    TrangThaiDuyet = sp.TrangThaiDuyet,
                    TepDinhKem = sp.TapTinDinhKems.AsEnumerable().Where(p => p.IsDeleted == false).Select(fd => new 
                    {
                        ID_TT = fd.ID,
                        TenTapTin = fd.TenTapTin,
                        DuongDanURL = fd.DuongDan,
                        NgayDinhKem = fd.NgayDinhKem?.ToString("dd/MM/yyyy"),
                    }).ToList()
                });
            if (bantins.Count() > 0)
            {
                return Ok(new
                {
                    success_message = "Lấy chi tiết bản tin thành công",
                    data = bantins
                });
            }
            else
            {
                return Ok(new
                {
                    error_message = "Lấy chi tiết bản tin không thành công",
                });
            }
        }

        //Lấy tệp đính kèm (ảnh, phim, tài liệu) của bản tin
        [HttpGet]
        public HttpResponseMessage LayTepDinhKemTheoID(int idtbt)
        {
            EntitiesQLKTTV dbContext = new EntitiesQLKTTV();
            var tepDinhKem = dbContext.TapTinDinhKems.FirstOrDefault(s => s.ID == idtbt && s.IsDeleted == false);
            if (tepDinhKem == null)
            {
                //return new HttpResponseMessage(HttpStatusCode.NotFound);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NotFound);
                response.Content = new StringContent("Không tìm thấy tệp đính kèm.");
                return response;
            }
            string urlHost = ClsFtp.GetHostFTP();
            string UserName = ClsFtp.GetUserNameFTP();
            string Password = ClsFtp.GetPassWordFTP();
            string folderPath = tepDinhKem.DuongDan;
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
            var contentType = MimeMapping.GetMimeMapping(tepDinhKem.TenTapTin);
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(ftpStream)
            };
            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = tepDinhKem.TenTapTin
                };
            result.Content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            return result;
        }
    }
}
