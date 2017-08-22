namespace WebRole1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Response")]
    public partial class Response
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdU { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdP { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdC { get; set; }

        public int IdA { get; set; }

        public DateTime? SendTime { get; set; }

        public virtual Answer Answer { get; set; }

        public virtual Channel Channel { get; set; }

        public virtual Question Question { get; set; }

        public virtual User User { get; set; }
    }
}
