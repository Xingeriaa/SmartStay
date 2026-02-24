namespace do_an_tot_nghiep.Filters
{
    /// <summary>
    /// Tên Role chuẩn — khớp CHÍNH XÁC với giá trị RoleName trong bảng Roles (DB).
    /// DB schema: Admin / Staff / Tenant
    /// </summary>
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Staff = "Staff";
        public const string Tenant = "Tenant";   // DB value = "Tenant"

        // Composite shortcuts (cách nhau dấu phẩy)
        public const string AdminOrStaff = "Admin,Staff";
        public const string AdminStaffOrTenant = "Admin,Staff,Tenant";
    }
}
