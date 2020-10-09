using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureSort.Models
{
    public class PictureInfo
    {
        public string Id { get; set; }
        public bool IsCatched { get; set; } = false;
        public string Remark { get; set; }
        public string SubFolder { get; set; }
        public string CopyFrom { get; set; }

        public string SaveAs { get; set; }
    }
}
