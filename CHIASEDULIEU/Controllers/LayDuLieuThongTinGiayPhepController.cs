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
    public class LayDuLieuThongTinGiayPhepController : ApiController
    {
        //Thêm thông tin giấy phép
        [HttpPost]
        public IHttpActionResult ThemDuLieuGiayPhep()
        {
            System.Collections.Specialized.NameValueCollection form = HttpContext.Current.Request.Form;
            string SoHieu = form["SoHieu"];
            string Nam = form["Nam"];
            string ngayThangStr = form["NgayThang"];
            DateTime? NgayThang = null;
            if (!string.IsNullOrEmpty(ngayThangStr)) //chuyển đổi chuỗi form["NgayThang"] thành kiểu DateTime
            {
                NgayThang = ClsFunctionDate.ConvertDate(ngayThangStr);
            }
            string TenDonVi = form["TenDonVi"];
            string GiaHan = form["GiaHan"];
            string LyDoThuHoi = form["LyDoThuHoi"];
            string PhamViHoatDong = form["PhamViHoatDong"];
            string TrangThaiGiayPhep = form["TrangThaiGiayPhep"];
            int DoiTuongCungCap;
            int.TryParse(form["DoiTuongCungCap"], out DoiTuongCungCap);
            int DonViKyXacNhan;
            int.TryParse(form["DonViKyXacNhan"], out DonViKyXacNhan);
            int MaCapDoDuBao;
            int.TryParse(form["MaCapDoDuBao"], out MaCapDoDuBao);
            string FilePath = null;
            string path = null;
            string fileName = null;
            string urlHost = ClsFtp.GetHostFTP();
            string UserName = ClsFtp.GetUserNameFTP();
            string Password = ClsFtp.GetPassWordFTP();
            EntitiesQLKTTV dbContext = new EntitiesQLKTTV();
            ThongTinGiayPhep thongTinGiayPhep = new ThongTinGiayPhep();
            thongTinGiayPhep.SoHieu = SoHieu;
            thongTinGiayPhep.Nam = Nam;
            thongTinGiayPhep.NgayThang = NgayThang;
            thongTinGiayPhep.TenDonVi = TenDonVi;
            thongTinGiayPhep.GiaHan = GiaHan;
            thongTinGiayPhep.LyDoThuHoi = LyDoThuHoi;
            thongTinGiayPhep.PhamViHoatDong = PhamViHoatDong;
            thongTinGiayPhep.TrangThaiGiayPhep = TrangThaiGiayPhep;
            thongTinGiayPhep.DoiTuongCungCap = DoiTuongCungCap;
            thongTinGiayPhep.DonViKyXacNhan = DonViKyXacNhan;
            thongTinGiayPhep.MaCapDoDuBao = MaCapDoDuBao;
            dbContext.ThongTinGiayPheps.Add(thongTinGiayPhep);
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
                    string FileName = System.IO.Path.GetFileName(filepath.FileName);
                    string FileTest = System.IO.Path.GetFullPath(filepath.FileName);
                    File_DinhKem FileDK = new File_DinhKem();
                    FileDK.ID_GiayPhep = thongTinGiayPhep.ID;
                    FileDK.Ten_FileDinhKem = fileName;
                    FileDK.NgayTao = DateTime.Now;
                    FileDK.IS_IMAGE = false;
                    FileDK.ISDELETE = false;
                    int max_ID = dbContext.ThongTinGiayPheps.Max(p => p.ID);
                    FilePath = urlHost + "/ThongTinGiayPhep/" + max_ID.ToString() + @"/" + fileName;
                    FileDK.DuongDanURL = "/ThongTinGiayPhep/" + max_ID.ToString() + @"/" + fileName;
                    dbContext.File_DinhKem.Add(FileDK);
                    thongTinGiayPhep.File_DinhKem.Add(FileDK);


                    //Lưu tệp đính kèm vào thư mục FTP
                    string ten_FolderSaveToFTP = "/ThongTinGiayPhep/" + FileDK.ID_GiayPhep.ToString();
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
                SoHieu,
                Nam,
                NgayThang,
                TenDonVi,
                GiaHan,
                LyDoThuHoi,
                PhamViHoatDong,
                TrangThaiGiayPhep,
                DoiTuongCungCap,
                DonViKyXacNhan,
                MaCapDoDuBao,
            });
        }
    }
}
