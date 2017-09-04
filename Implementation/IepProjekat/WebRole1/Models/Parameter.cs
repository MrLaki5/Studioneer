namespace WebRole1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Parameter
    {
        [Key]
        public int IdP { get; set; }

        [Required]
        public int? AnswerNumber { get; set; }

        [Required]
        public int? SilverNumber { get; set; }

        [Required]
        public int? GoldNumber { get; set; }

        [Required]
        public int? PlatinumNumber { get; set; }

        [Required]
        public int? UnlockNumber { get; set; }

        [Required]
        public int? PremiumNumber { get; set; }
    }
}
