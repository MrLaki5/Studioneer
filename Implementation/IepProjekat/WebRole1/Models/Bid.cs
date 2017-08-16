namespace WebRole1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Bid")]
    public partial class Bid
    {
        [Key]
        [StringLength(18)]
        public string IdB { get; set; }

        [StringLength(18)]
        public string Time { get; set; }

        public int IdU { get; set; }

        public int IdA { get; set; }

        public virtual Auction Auction { get; set; }

        public virtual User User { get; set; }
    }
}
