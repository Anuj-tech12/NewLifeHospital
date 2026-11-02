using System;
using System.ComponentModel.DataAnnotations;

namespace NewLifeHospital.Models
{
    public class Appointment
    {
        public int Id { get; set; }


    [Required]
        public string PatientName { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        public string? Email { get; set; }

        [Required]
        public string Doctor { get; set; } = string.Empty;

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public string AppointmentTime { get; set; } = string.Empty;

        public string? Message { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }


}
