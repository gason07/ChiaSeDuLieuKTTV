using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using DULIEUKTTV;

namespace CHIASEDULIEU.Utilities
{
    public class ClsDanhMuc
    {
        public static string khuVucHanhChinh (int maKVHC)
        {
            EntitiesQLKTTV eData = new EntitiesQLKTTV();
            string kq = "";
            var kvhc = eData.KhuVucHanhChinhs.FirstOrDefault(sp => sp.MaKVHC == maKVHC);
            if (kvhc != null)
            {
                kq = kvhc.TenKVHC;
            }
            return kq;
        }

        public static string khuVuc(int maKhuVuc)
        {
            EntitiesQLKTTV eData = new EntitiesQLKTTV();
            string kq = "";
            var kv = eData.KhuVucs.FirstOrDefault(sp => sp.ID == maKhuVuc);
            if (kv != null)
            {
                kq = kv.TenKhuVuc;
            }
            return kq;
        }

        public static string loaiBanTin (int maLoaiBantin)
        {
            EntitiesQLKTTV eData = new EntitiesQLKTTV();
            string kq = "";
            var lbt = eData.LoaiBanTins.FirstOrDefault(sp => sp.ID == maLoaiBantin);
            if (lbt != null)
            {
                kq = lbt.TenLoaiBanTin;
            }
            return kq;
        }

        public static string capDo(int loaiCapDo)
        {
            EntitiesQLKTTV eData = new EntitiesQLKTTV();
            string kq = "";
            var cd = eData.CapDoes.FirstOrDefault(sp => sp.ID == loaiCapDo);
            if (cd != null)
            {
                kq = cd.TenLoaiCapDo;
            }
            return kq;
        }

        public static string loaiThienTai(int maLoaiThienTai)
        {
            EntitiesQLKTTV eData = new EntitiesQLKTTV();
            string kq = "";
            var ltt = eData.LoaiThienTais.FirstOrDefault(sp => sp.ID== maLoaiThienTai);
            if (ltt != null)
            {
                kq = ltt.TenLoaiThienTai;
            }
            return kq;
        }

        public static string soHieu (int maSoHieu)
        {
            EntitiesQLKTTV eData = new EntitiesQLKTTV();
            string kq = "";
            var sh = eData.ThongTinGiayPheps.FirstOrDefault(sp => sp.ID == maSoHieu);
            if (sh != null)
            {
                kq = sh.SoHieu;
            }
            return kq;
        }
    }
}