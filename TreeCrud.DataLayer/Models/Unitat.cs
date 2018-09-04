using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace TreeCrud.DataLayer.Models
{
    public class Unitat
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string Tipus { get; set; }
        public DateTime Data { get; set; }

    }
}