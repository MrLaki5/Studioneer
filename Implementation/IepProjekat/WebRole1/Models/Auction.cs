namespace WebRole1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Auction")]
    public partial class Auction
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Auction()
        {
            Bid = new HashSet<Bid>();
            Winner = new HashSet<Winner>();
        }

        [Key]
        public int IdA { get; set; }

        public int? Time { get; set; }

        [StringLength(20)]
        public string Name { get; set; }

        public int? StPrice { get; set; }

        public DateTime? CrDate { get; set; }

        public DateTime? OpDate { get; set; }

        public DateTime? EndDate { get; set; }

        [StringLength(20)]
        public string State { get; set; }

        public int? IncPrice { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bid> Bid { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Winner> Winner { get; set; }
    }
}
