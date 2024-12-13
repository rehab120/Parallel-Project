using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDowaloadergui
{
    internal class FileDowalod
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string FileName { get; set; }

        [Required]

        public string Url { get; set; }

        public DateTime DateDownaload { get; set; }

        //[MaxLength(50)]
        //public string Path { get; set; }

        public string? Statue { get; set; }

        public int ProgressPercentageF { get; set; }



    }
}
