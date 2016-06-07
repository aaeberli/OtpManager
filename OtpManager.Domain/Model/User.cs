namespace OtpManager.Domain.Model
{
    using Common.Abstract;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("User")]
    public partial class User : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string UserId { get; set; }

        public virtual Otp Otp { get; set; }
    }
}
