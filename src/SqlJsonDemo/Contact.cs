using System;
using System.ComponentModel.DataAnnotations;

namespace SqlJsonDemo
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Notes { get; set; }
    }
}
