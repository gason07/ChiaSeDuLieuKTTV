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
    public class ChuyenDuLieuBanTinDuBaoController : ApiController
    {
        //Thêm bản tin dự báo
        [HttpPost]
        public IHttpActionResult ThemDuLieuBanTinDuBao123()
        {
            System.Collections.Specialized.NameValueCollection form = HttpContext.Current.Request.Form;
            string TieuDe = form["TieuDe"];
            string TomTatNoiDung = form["TomTatNoiDung"];
            string NoiDung = form["NoiDung"];
            string NguoiDang = form["NguoiDang"];
            string TenToChucDuBao = form["TenToChucDuBao"];
            string NguonTinDuBao = form["NguonTinDuBao"];
            string KEY_SEARCH = form["KEY_SEARCH"];
            int MaCapDoDuBao;
            int.TryParse(form["MaCapDoDuBao"], out MaCapDoDuBao);
            int MaKhuVuc;
            int.TryParse(form["MaKhuVuc"], out MaKhuVuc);
            int MaKVHC;
            int.TryParse(form["MaKVHC"], out MaKVHC);
            int MaLoaiThienTai;
            int.TryParse(form["MaLoaiThienTai"], out MaLoaiThienTai);
            int MaLoaiBanTin;
            int.TryParse(form["MaLoaiBanTin"], out MaLoaiBanTin);
            int ThoiGianDuBao;
            int.TryParse(form["ThoiGianDuBao"], out ThoiGianDuBao);
            bool TrangThaiDuyet;
            bool TrangThaiDuyetResult = bool.TryParse(form["TrangThaiDuyet"], out TrangThaiDuyet);
            string FilePath = null;
            string path = null;
            string fileName = null;
            string urlHost = ClsFtp.GetHostFTP();
            string UserName = ClsFtp.GetUserNameFTP();
            string Password = ClsFtp.GetPassWordFTP();
            EntitiesQLKTTV dbContext = new EntitiesQLKTTV();
            BanTinDuBao banTinDuBao = new BanTinDuBao();
            banTinDuBao.TieuDe = TieuDe;
            banTinDuBao.TomTatNoiDung = TomTatNoiDung;
            banTinDuBao.NoiDung = NoiDung;
            banTinDuBao.NgayThucHien = DateTime.Now;
            banTinDuBao.NguoiDang = NguoiDang;
            banTinDuBao.TenToChucDuBao = TenToChucDuBao;
            banTinDuBao.NguonTinDuBao = NguonTinDuBao;
            banTinDuBao.KEY_SEARCH = KEY_SEARCH;
            banTinDuBao.MaCapDoDuBao = MaCapDoDuBao;
            banTinDuBao.MaKhuVuc = MaKhuVuc;
            banTinDuBao.MaKVHC = MaKVHC;
            banTinDuBao.MaLoaiThienTai = MaLoaiThienTai;
            banTinDuBao.MaLoaiBanTin = MaLoaiBanTin;
            banTinDuBao.ThoiGianDuBao = ThoiGianDuBao;
            banTinDuBao.TrangThaiDuyet = true;
            dbContext.BanTinDuBaos.Add(banTinDuBao);
            dbContext.SaveChanges();
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

                    //Nếu là hình ảnh thì lưu vào thư mục hình ảnh, video thì lưu vào mục video, tài liệu thì lưu vào mục văn bản
                    string folderToSave = IsImage ? "HinhAnh/" : (IsVideo ? "Video/" : "VanBan/");

                    TapTinDinhKem FileDK = new TapTinDinhKem();
                    FileDK.TenTapTin = fileName;
                    FileDK.MaBanTinDuBao = banTinDuBao.ID;
                    FileDK.NgayDinhKem = DateTime.Now;
                    FileDK.IsHinhAnh = IsImage;
                    FileDK.IsVideo = IsVideo;
                    FileDK.IsDeleted = false;
                    int max_ID = dbContext.BanTinDuBaos.Max(p => p.ID);
                    FilePath = urlHost + "/FileUpload/" + folderToSave + max_ID.ToString() + @"/" + fileName;
                    FileDK.DuongDan = "/FileUpload/" + folderToSave + max_ID.ToString() + @"/" + fileName;
                    dbContext.TapTinDinhKems.Add(FileDK);
                    banTinDuBao.TapTinDinhKems.Add(FileDK);

                    //Lưu tệp đính kèm vào thư mục FTP
                    string ten_FolderSaveToFTP = "/FileUpload/" + folderToSave + FileDK.MaBanTinDuBao.ToString();
                    bool createFolder = ClsFtp.CreateFolder(urlHost, ten_FolderSaveToFTP, UserName, Password);
                    if (createFolder)
                    {
                        bool fileUploaded = ClsFtp.UploadFile(urlHost, ten_FolderSaveToFTP, fileName, path, UserName, Password);
                    }
                    //xóa app_Data
                    File.Delete(path);
                }
            }
            dbContext.SaveChanges();
            return Ok(new
            {
                FilePath,
                TieuDe,
                TomTatNoiDung,
                NoiDung,
                NguoiDang,
                TenToChucDuBao,
                NguonTinDuBao,
                KEY_SEARCH,
                MaCapDoDuBao,
                MaKVHC,
                MaLoaiThienTai,
                MaLoaiBanTin,
                ThoiGianDuBao,
                TrangThaiDuyet,
            });
        }
    }
}
