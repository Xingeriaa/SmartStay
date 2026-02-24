namespace do_an_tot_nghiep.ViewModels
{
    public class DashboardViewModel
    {
        public decimal Revenue { get; set; }
        public decimal Debt { get; set; }
        public int TotalRooms { get; set; }
        public int EmptyRooms { get; set; }
        public decimal VacancyRate { get; set; }
        public long ElectricityConsumption { get; set; }
        public long WaterConsumption { get; set; }
        public string MonthYear { get; set; } = string.Empty;
    }
}
