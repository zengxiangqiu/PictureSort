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
    using PictureSort.ViewModels;
    using System.Windows.Threading;

    public class PsRepository
    {
        public delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);
        public UpdateProgressBarDelegate updatePbDelegate;

        public ObservableCollection<PictureInfo> ImportSource(string path)
        {
            var mapper = new Mapper(path);
            var pInfos = mapper.Take<PictureInfo>().Select(x => x.Value);
            return new ObservableCollection<PictureInfo>(pInfos);
        }

        public Task Sort(PictureInfo pInfos, string[] files)
        {
            return Task.Factory.StartNew(() =>
            {
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
                        UpdatePb();
                    }
                    catch (Exception ex)
                    {
                        pInfo.Remark = ex.Message;
                    }
                }
            });
        }

        private void UpdatePb()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(updatePbDelegate,System.Windows.Threading.DispatcherPriority.Background,new object[] { System.Windows.Controls.ProgressBar.ValueProperty, 2 });
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

            subFolders.ForEach(x =>
            {
                if (!Directory.Exists(Path.Combine(targetPath, x)))
                {
                    Directory.CreateDirectory(Path.Combine(targetPath, x));
                }
            });

            pInfos.ForEach(x => CombineSaveAs(x, targetPath));
        }

        private void CombineSaveAs(PictureInfo pInfo, string targetPath)
        {
            var idParts = pInfo.Id.Split('-');
            var subFolder = "";
            if (idParts.Length >= 4)
                subFolder = idParts[3];
            else
                subFolder = "UnKnown";
            pInfo.SaveAs = Path.Combine(targetPath, subFolder, pInfo.Id + ".jpg");
        }

        public void Compare(PsViewModel vm)
        {
            var sourcePath = vm.SourceFile;
            var searchPath = vm.SearchFolder;
            var savePath = vm.SaveFolder;
            var infos = vm.PictureInfos;

            //infos.Clear();
            //ImportSource(sourcePath).ForEach(infos.Add);

            var files = Directory.GetFiles(searchPath, "*.jpg", SearchOption.AllDirectories);

            var tasks = infos.Select(x => Sort(x, files)).ToArray();

            CreateSubFolder(infos, savePath);

            Task.Factory.ContinueWhenAll(tasks, ancedents =>
            {
                //sort
                var cloneTasks = infos.Where(x => x.IsCatched).Select(Clone).ToArray();

                Task.Factory.ContinueWhenAll(cloneTasks, subs =>
                {
                    //callback

                });
            });
        }
    }
}
