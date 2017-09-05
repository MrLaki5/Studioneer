namespace WebRole1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Question")]
    public partial class Question
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Question()
        {
            Answers = new HashSet<Answer>();
            Publisheds = new HashSet<Published>();
            Responses = new HashSet<Response>();
        }

        [Key]
        public int IdP { get; set; }

        [StringLength(20)]
        public string Title { get; set; }

        [StringLength(200)]
        public string Text { get; set; }

        public byte[] Image { get; set; }

        public DateTime? CreationTime { get; set; }

        public DateTime? LastLock { get; set; }

        public int IdU { get; set; }

        public int? IsLocked { get; set; }

        public int? IsClone { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Answer> Answers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Published> Publisheds { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Response> Responses { get; set; }
    }
}
