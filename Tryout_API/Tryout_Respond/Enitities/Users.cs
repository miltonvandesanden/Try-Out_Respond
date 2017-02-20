namespace Tryout_Respond
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Users
    {
        [Key]
        [StringLength(50)]
        public string userID { get; set; }

        [Required]
        [StringLength(50)]
        public string username { get; set; }

        [Required]
        [StringLength(50)]
        public string passwordHash { get; set; }

        public string token { get; set; }

        public DateTime? token_expirationDate { get; set; }

        public bool? IsAdmin { get; set; }
    }
}
