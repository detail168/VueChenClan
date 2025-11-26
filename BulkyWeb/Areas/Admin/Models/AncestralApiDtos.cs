using System.ComponentModel.DataAnnotations;

namespace BulkyBookWeb.Areas.Admin.Models
{
    public class AncestralPositionDto
    {
        public int AncestralPositionId { get; set; }
        [Required]
        public string PositionId { get; set; }
        public string Name { get; set; }
        public string Side { get; set; }
        public string Section { get; set; }
        public string Level { get; set; }
        public string Position { get; set; }
        public string Applicant { get; set; }
        public string Relation { get; set; }
        public string Mobile_Tel { get; set; }
        public string Note { get; set; }
    }

    public class SavePositionDto
    {
        [Required]
        public string DisplayText { get; set; }
        public int? SelectedAncestralPositionId { get; set; }
    }

    public class ImportRowDto
    {
        public string Name { get; set; }
        public string Side { get; set; }
        public string Section { get; set; }
        public string Level { get; set; }
        public string Position { get; set; }
        public string PositionId { get; set; }
        public string Applicant { get; set; }
        public string Relation { get; set; }
        public string Mobile_Tel { get; set; }
        public string Note { get; set; }
    }
}
