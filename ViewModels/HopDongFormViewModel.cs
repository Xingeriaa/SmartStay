using System.Collections.Generic;
using do_an_tot_nghiep.Models;

namespace do_an_tot_nghiep.ViewModels
{
    public class HopDongFormViewModel
    {
        public HopDong HopDong { get; set; } = new HopDong();
        public int? ChuHopDongId { get; set; }
        public List<int> CuDanIds { get; set; } = new List<int>();
        public List<PhongTro> PhongTros { get; set; } = new List<PhongTro>();
        public List<KhachThue> KhachThues { get; set; } = new List<KhachThue>();
    }
}
