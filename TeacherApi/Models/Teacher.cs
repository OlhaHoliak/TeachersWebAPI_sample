using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TeacherApi.Models
{
    public class Teacher
    {
        public long Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 2)]
        [RegularExpression(@"[^\d\.\!-)]*", ErrorMessage = "Name cannot contains digits and special symbols except whitespaces")]
        public string Name { get; set; }

        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        public bool IsWorking { get; set; }

        public string Secret { get; set; }
    }
}
