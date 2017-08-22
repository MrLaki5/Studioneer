namespace WebRole1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Answer")]
    public partial class Answer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Answer()
        {
            Responses = new HashSet<Response>();
        }

        [Key]
        public int IdA { get; set; }

        public int? IsCorrect { get; set; }

        public int IdP { get; set; }

        [StringLength(1)]
        public string Tag { get; set; }

        [StringLength(100)]
        public string Text { get; set; }

        public int? Number { get; set; }

        public virtual Question Question { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Response> Responses { get; set; }
    }
}
