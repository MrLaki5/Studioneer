namespace WebRole1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Winner")]
    public partial class Winner
    {
        [Key]
        [StringLength(18)]
        public string IdW { get; set; }

        public int IdA { get; set; }

        public int IdU { get; set; }

        public virtual Auction Auction { get; set; }

        public virtual User User { get; set; }
    }
}
