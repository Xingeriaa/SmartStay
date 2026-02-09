using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace do_an_tot_nghiep.Models
{
    [Table("Buildings")]
    public class NhaTro
    {
        private const string MetaVersion = "v1";
        private bool _suppressMetaUpdate;
        private string? _description;
        private int _loaiNha;
        private decimal _giaThue;
        private int _ngayThuTien;
        private string? _ghiChu;

        [Key]
        [Column("BuildingId")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên nhà")]
        [Column("Name")]
        public string TenNhaTro { get; set; } = string.Empty;

        [NotMapped]
        public int LoaiNha
        {
            get => _loaiNha;
            set
            {
                _loaiNha = value;
                UpdateDescription();
            }
        }

        [NotMapped]
        public decimal GiaThue
        {
            get => _giaThue;
            set
            {
                _giaThue = value;
                UpdateDescription();
            }
        }

        [NotMapped]
        public int NgayThuTien
        {
            get => _ngayThuTien;
            set
            {
                _ngayThuTien = value;
                UpdateDescription();
            }
        }

        [NotMapped]
        public string? TinhThanh { get; set; }

        [NotMapped]
        public string? QuanHuyen { get; set; }

        [NotMapped]
        public string? PhuongXa { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ chi tiết")]
        [Column("Address")]
        public string DiaChiChiTiet { get; set; } = string.Empty;

        [NotMapped]
        public string? GhiChu
        {
            get => _ghiChu;
            set
            {
                _ghiChu = value;
                UpdateDescription();
            }
        }

        [Column("Utilities")]
        public string? DanhSachDichVu { get; set; }

        public int? ManagerId { get; set; }
        public DateTime? OperationDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("Description")]
        public string? Description
        {
            get => _description;
            set
            {
                _description = value;
                ParseDescription(value);
            }
        }

        private void ParseDescription(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            try
            {
                var meta = JsonSerializer.Deserialize<BuildingMeta>(value);
                if (meta == null || meta.Version != MetaVersion)
                {
                    return;
                }

                _suppressMetaUpdate = true;
                _loaiNha = meta.LoaiNha;
                _giaThue = meta.GiaThue;
                _ngayThuTien = meta.NgayThuTien;
                _ghiChu = meta.GhiChu;
            }
            catch
            {
                // Ignore invalid metadata
            }
            finally
            {
                _suppressMetaUpdate = false;
            }
        }

        private void UpdateDescription()
        {
            if (_suppressMetaUpdate)
            {
                return;
            }

            var meta = new BuildingMeta
            {
                Version = MetaVersion,
                LoaiNha = _loaiNha,
                GiaThue = _giaThue,
                NgayThuTien = _ngayThuTien,
                GhiChu = _ghiChu
            };

            _description = JsonSerializer.Serialize(meta);
        }

        private class BuildingMeta
        {
            public string Version { get; set; } = MetaVersion;
            public int LoaiNha { get; set; }
            public decimal GiaThue { get; set; }
            public int NgayThuTien { get; set; }
            public string? GhiChu { get; set; }
        }
    }
}
