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
        [StringLength(18)]
        public string IdO { get; set; }

        public int IdU { get; set; }

        [StringLength(18)]
        public string Number { get; set; }

        [StringLength(18)]
        public string Price { get; set; }

        [StringLength(18)]
        public string State { get; set; }

        public virtual User User { get; set; }
    }
}
