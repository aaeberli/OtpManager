namespace OtpManager.Domain.Model
{
    using Common.Abstract;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Otp")]
    public partial class Otp : BaseEntity
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
