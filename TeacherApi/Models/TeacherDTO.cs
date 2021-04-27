using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TeacherApi.Models
{
    [ModelMetadataType(typeof(Teacher))]
    public class TeacherDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool IsWorking { get; set; }
    }
}
