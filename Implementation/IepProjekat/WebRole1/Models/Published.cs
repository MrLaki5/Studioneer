namespace WebRole1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Published")]
    public partial class Published
    {
        public int IdP { get; set; }

        public int IdC { get; set; }

        [Key]
        public int IdPub { get; set; }

        public DateTime? PubTime { get; set; }

        public virtual Channel Channel { get; set; }

        public virtual Question Question { get; set; }
    }
}
