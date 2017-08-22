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

        public int? AnswerNumber { get; set; }

        public int? SilverNumber { get; set; }

        public int? GoldNumber { get; set; }

        public int? PlatinumNumber { get; set; }

        public int? UnlockNumber { get; set; }

        public int? PremiumNumber { get; set; }
    }
}
