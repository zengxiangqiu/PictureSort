using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureSort.Models
{
    public class PictureInfo
    {
        public int Id { get; set; }
        public bool IsCatched { get; set; } = false;
        public string Remark { get; set; }
        public string SubFolder { get; set; }
    }
}
