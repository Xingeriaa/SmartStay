namespace do_an_tot_nghiep.Models.Enums
{
    public enum ServiceChargeType
    {
        FixedPerRoom,      // Flat fee applied to the whole room
        FixedPerTenant,    // Fee multiplied by the number of active tenants
        Metered,           // Calculated via MeterReadings (Electricity, Water)
        UsageBased,        // Variable count per month
        Membership         // Opt-in subscription
    }
}
