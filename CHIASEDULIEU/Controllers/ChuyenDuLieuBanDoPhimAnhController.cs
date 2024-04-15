using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.IO;
using DULIEUKTTV;
using CHIASEDULIEU.Utilities;

namespace CHIASEDULIEU.Controllers
{
    public class ChuyenDuLieuBanDoPhimAnhController : ApiController
    {
        //Thêm dữ liệu phim, hình ảnh
        [HttpPost]
        public IHttpActionResult ThemDuLieuPhimHinhAnh()
        {
            try
            {
                System.Collections.Specialized.NameValueCollection form = HttpContext.Current.Request.Form;
                string TieuDe = form["TieuDe"];
                string MoTa = form["MoTa"];
                string GhiChu = form["GhiChu"];
                string FilePath = null;
                string path = null;
                string fileName = null;
                string ngayThangStr = form["NgayThang"];
                DateTime? NgayThang = null;
                if (!string.IsNullOrEmpty(ngayThangStr))
                {
                    NgayThang = ClsFunctionDate.ConvertDate(ngayThangStr);
                }
                string urlHost = ClsFtp.GetHostFTP();
                string UserName = ClsFtp.GetUserNameFTP();
                string Password = ClsFtp.GetPassWordFTP();
                for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    var filepath = HttpContext.Current.Request.Files.Count > 0 ?
                    HttpContext.Current.Request.Files[i] : null;
                    if (filepath != null && filepath.ContentLength > 0)
                    {
                        fileName = Path.GetFileName(filepath.FileName);
                        path = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), fileName);
                        filepath.SaveAs(path);

                        // Xác định kiểu của tệp đính kèm bằng cách sử dụng ClsFtp
                        string mimeType = ClsFtp.GetMimeTypeByFileName(fileName);

                        // Kiểm tra kiểu MIME để xác định xem tệp là hình ảnh hay video
                        bool IsImage = mimeType.StartsWith("image/");
                        bool IsVideo = mimeType.StartsWith("video/");

                        using (EntitiesQLKTTV dbContext = new EntitiesQLKTTV())
                        {
                            int max_ID = dbContext.PhimHinhAnhs.Max(p => p.ID) + 1;
                            var phimAnh = new PhimHinhAnh()
                            {
                                TieuDe = TieuDe,
                                NgayThang = NgayThang,
                                GhiChu = GhiChu,
                                TenTapTin = fileName,
                                DuongDanURL = "/PhimAnh/" + max_ID.ToString() + @"/" + fileName,
                                MoTa = MoTa,
                                ThoiGianCapNhat = DateTime.Now,
                                IsDeleted = false,
                                IsImage = IsImage,
                                IsVideo = IsVideo
                            };
                            FilePath = urlHost + "/PhimAnh/" + max_ID.ToString() + @"/" + fileName;
                            dbContext.PhimHinhAnhs.Add(phimAnh);
                            dbContext.SaveChanges();

                            string ten_FolderSaveToFTP = "/PhimAnh/" + phimAnh.ID.ToString();
                            bool createFolder = ClsFtp.CreateFolder(urlHost, ten_FolderSaveToFTP, UserName, Password);
                            if (createFolder)
                            {
                                bool fileUploaded = ClsFtp.UploadFile(urlHost, ten_FolderSaveToFTP, fileName, path, UserName, Password);
                            }
                            File.Delete(path); //xoá App_Data
                        }
                    }
                }
                return Ok(new
                {
                    FilePath,
                    TieuDe,
                    MoTa,
                    GhiChu,
                    NgayThang,
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //Thêm dữ liệu bản đồ
        [HttpPost]
        public IHttpActionResult ThemDuLieuBanDo()
        {
            try
            {
                System.Collections.Specialized.NameValueCollection form = HttpContext.Current.Request.Form;
                string TenBanDo = form["TenBanDo"];
                string TenTapTin = form["TenTapTin"];
                string MoTa = form["MoTa"];
                string FilePath = null;
                string path = null;
                string fileName = null;
                string urlHost = ClsFtp.GetHostFTP();
                string UserName = ClsFtp.GetUserNameFTP();
                string Password = ClsFtp.GetPassWordFTP();
                for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    var filepath = HttpContext.Current.Request.Files.Count > 0 ?
                    HttpContext.Current.Request.Files[i] : null;
                    if (filepath != null && filepath.ContentLength > 0)
                    {
                        fileName = Path.GetFileName(filepath.FileName);
                        path = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), fileName);
                        filepath.SaveAs(path);

                        using (EntitiesQLKTTV dbContext = new EntitiesQLKTTV())
                        {
                            int max_ID = dbContext.BanDoes.Max(p => p.ID) + 1;
                            var banDo = new BanDo()
                            {
                                TenBanDo = TenBanDo,
                                TenTapTin = fileName,
                                DuongDanURL = "/BanDo/" + max_ID.ToString() + @"/" + fileName,
                                MoTa = MoTa,
                                ThoiGianCapNhat = DateTime.Now,
                                IsDeleted = false,
                            };
                            FilePath = urlHost + "/BanDo/" + max_ID.ToString() + @"/" + fileName;
                            dbContext.BanDoes.Add(banDo);
                            dbContext.SaveChanges();
                            string ten_FolderSaveToFTP = "/BanDo/" + banDo.ID.ToString();
                            bool createFolder = ClsFtp.CreateFolder(urlHost, ten_FolderSaveToFTP, UserName, Password);
                            if (createFolder)
                            {
                                bool fileUploaded = ClsFtp.UploadFile(urlHost, ten_FolderSaveToFTP, fileName, path, UserName, Password);
                            }
                            File.Delete(path);
                        }
                    }
                }
                return Ok(new
                {
                    FilePath,
                    TenBanDo,
                    MoTa,
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
