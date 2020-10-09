using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PictureSort.Models;
using Npoi.Mapper;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;
using System.Security.AccessControl;
using System.Security.Policy;
using System.IO.Packaging;
using System.Collections.ObjectModel;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace PictureSort.Repositories
{
    public class PsRepository
    {
        public ObservableCollection<PictureInfo> ImportSource(string path)
        {
            var mapper = new Mapper(path);
            var pInfos = mapper.Take<PictureInfo>().Select(x=>x.Value);
            return new ObservableCollection<PictureInfo>(pInfos);
        }

        public Task Sort(PictureInfo pInfos, string[] files)
        {
            return Task.Factory.StartNew(() => {
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Contains(pInfos.Id.Trim()))
                    {
                        pInfos.IsCatched = true;
                        pInfos.CopyFrom = files[i];
                    }
                }
            });
        }

        public Task Clone(PictureInfo pInfo)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var img = Image.Load(pInfo.CopyFrom))
                {
                    try
                    {
                        img.Save(pInfo.SaveAs);
                    }
                    catch (Exception ex)
                    {
                        pInfo.Remark = ex.Message;
                    }
                }
            });
        }

        private void CreateSubFolder(ObservableCollection<PictureInfo> pInfos, string targetPath)
        {
            var subFolders = pInfos.Select(x =>
           {
               var idParts = x.Id.Split('-');
               if (idParts.Length >= 4)
                   return idParts[3];
               else
                   return "UnKnown";
           }).GroupBy(x => x).Select(x => x.Key);

            subFolders.ForEach(x => {
                if (!Directory.Exists(Path.Combine(targetPath, x)))
                {
                    Directory.CreateDirectory(Path.Combine(targetPath, x));
                }
            });
        }

        public void Compare(string sourcePath, string searchPath, string targetPath)
        {
            var infos = ImportSource(sourcePath);

            var files =  Directory.GetFiles(searchPath);

            var tasks = infos.Select(x => Sort(x, files)).ToArray();

            CreateSubFolder(infos, targetPath);
            
            Task.Factory.ContinueWhenAll(tasks, ancedents =>
            {
                //sort
                var cloneTasks =  infos.Where(x=>x.IsCatched).Select(Clone).ToArray();

                Task.Factory.ContinueWhenAll(cloneTasks, subs => { 
                    //callback

                });
            });
        }
    }
}
