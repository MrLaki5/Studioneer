namespace WebRole1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order
    {
        [Key]
        public int IdO { get; set; }

        public int IdU { get; set; }

        public int? Number { get; set; }

        public int? Price { get; set; }

        [StringLength(20)]
        public string State { get; set; }

        public virtual User User { get; set; }
    }
}
