using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("SystemConfigs")]
    public class SystemConfig
    {
        [Key]
        [Column("ConfigKey")]
        public string Key { get; set; } = string.Empty;

        [Column("ConfigValue")]
        public string? Value { get; set; }

        public string? Description { get; set; }
    }
}
