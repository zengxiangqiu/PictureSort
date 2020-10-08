using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PictureSort.Models;
using Npoi.Mapper;
using System.Runtime.InteropServices.WindowsRuntime;

namespace PictureSort.Repositories
{
    public class PsRepository
    {
        public List<PictureInfo> ImportSource(string path)
        {
            var mapper = new Mapper(path);
            var pInfos = mapper.Take<PictureInfo>().Select(x=>x.Value).ToList();
            return pInfos;
        }

        public Task Sort(List<PictureInfo> pInfos, string target)
    }
}
