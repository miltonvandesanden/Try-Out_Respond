namespace Tryout_Respond
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Taart
    {
        [Key]
        public int Id { get; set; }

        public string Type { get; set; }
        public string Naam { get; set; }
        public int Prijs { get; set; }
    }
}
