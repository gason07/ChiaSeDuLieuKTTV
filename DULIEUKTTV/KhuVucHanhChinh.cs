//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DULIEUKTTV
{
    using System;
    using System.Collections.Generic;
    
    public partial class KhuVucHanhChinh
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KhuVucHanhChinh()
        {
            this.BanTinDuBaos = new HashSet<BanTinDuBao>();
            this.KhuVucHanhChinh1 = new HashSet<KhuVucHanhChinh>();
        }
    
        public int MaKVHC { get; set; }
        public string TenKVHC { get; set; }
        public Nullable<int> MaKVHC_CapTren { get; set; }
        public Nullable<int> MaKhuVuc { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BanTinDuBao> BanTinDuBaos { get; set; }
        public virtual KhuVuc KhuVuc { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KhuVucHanhChinh> KhuVucHanhChinh1 { get; set; }
        public virtual KhuVucHanhChinh KhuVucHanhChinh2 { get; set; }
    }
}