using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureSort.Utils
{
    using PictureSort.Models;
    partial class PInfoEqualityComparer : IEqualityComparer<PictureInfo>
    {
        public bool Equals(PictureInfo x, PictureInfo y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;
            else if (x.Id == y.Id)
            {
                x.Count++;
                return true;
            }
            else
                return false;
        }

        public int GetHashCode(PictureInfo obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
