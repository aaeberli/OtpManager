namespace OtpManager.Domain.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Otp")]
    public partial class Otp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }

        [Required]
        [StringLength(40)]
        public string Password { get; set; }

        public DateTime StartDate { get; set; }

        public virtual User User { get; set; }
    }
}
