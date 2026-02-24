using System.Collections.Generic;

namespace do_an_tot_nghiep.ViewModels
{
    public class TicketDetailViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
        public string CreatedDate { get; set; } = string.Empty;
        public string AssignedToStaff { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
