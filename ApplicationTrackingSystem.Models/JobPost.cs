using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTrackingSystem.Models
{
    public class JobPost
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Title { get; set; }
        [DataType(DataType.Date)]
        public DateTime DatePosted { get; set; }
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }
        [Required]
        [StringLength(30)]
        public string? CandidateSkill { get; set; }
        [Required]
        [StringLength(30)]
        public string? Experience {  get; set; }
        [Required]
        [StringLength(30)]
        public string? Qualification { get; set; }
        [Required]
        [StringLength(1000)] 
        public string? Description { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CreatedAt { get; set; }
        [StringLength(100)]
        public string? CreatedBy { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ModifiedAt { get; set; }
        [StringLength(100)]
        public string? ModifiedBy { get; set; }

    }
}
