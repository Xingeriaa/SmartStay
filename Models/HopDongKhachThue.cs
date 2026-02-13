using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("ContractTenants")]
    public class HopDongKhachThue
    {
        [Key]
        public long Id { get; set; }

        [Column("ContractId")]
        public int HopDongId { get; set; }

        [Column("TenantId")]
        public int KhachThueId { get; set; }

        public bool IsRepresentative { get; set; } = false;

        [NotMapped]
        public string VaiTro
        {
            get => IsRepresentative ? "CHU_HOP_DONG" : "CU_DAN";
            set => IsRepresentative = value == "CHU_HOP_DONG";
        }

        [ForeignKey(nameof(HopDongId))]
        public HopDong HopDong { get; set; } = null!;

        [ForeignKey(nameof(KhachThueId))]
        public KhachThue KhachThue { get; set; } = null!;
    }
}
