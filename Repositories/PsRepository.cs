﻿using System;
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
    using NPOI.SS.Formula.Functions;
    using PictureSort.ViewModels;
    using System.Windows.Threading;
    using PictureSort.Utils;

    public class PsRepository
    {
        private object lockObj = new object();

        public delegate void Completed();

        public event Completed Sort_CompletedEvent;

        public ObservableCollection<PictureInfo> GetPInfoFromExcel(string path)
        {
            var mapper = new Mapper(path);
            var pInfos = mapper.Take<PictureInfo>().Select(x => x.Value).Select(x=> {
                //截取3段
                var parts = x.Id.Split('-');
                var destination = new string[3] ;
                if (parts.Length > 3)
                    Array.Copy(parts, destination, 3);
                x.Id =  string.Join("-",destination);
                return x;
            })
                .Distinct(new PInfoEqualityComparer());
            return new ObservableCollection<PictureInfo>(pInfos);
        }

        /// <summary>
        /// 匹配
        /// </summary>
        /// <param name="pInfo"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public Task Sort(PictureInfo pInfo, string[] files)
        {
            return Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Contains(pInfo.Id.Trim()))
                    {
                        pInfo.IsCatched = true;
                        pInfo.CopyFrom = files[i];
                        break;
                    }
                    pInfo.IsCatched = false;
                }
            });
        }

        public Task Clone(PictureInfo pInfo, PsViewModel vm)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    //根据qty 抓取
                    for (int i = 0; i < pInfo.Count; i++)
                    {
                        //后缀
                        var suffix = new StringBuilder("(1)".Length* i).Insert(0, "(1)", i).ToString();
                        using (var img = Image.Load(pInfo.CopyFrom))
                        {
                            var filePath = Path.Combine(pInfo.SaveFolder, pInfo.NameWithoutSuffix+suffix+".jpg");
                            img.Save( filePath);
                        }
                    }
                    pInfo.Remark = $"已抓取{pInfo.Count}次";
                }
                catch (System.IO.IOException ie)
                {
                    if (File.Exists(pInfo.SaveFolder))
                    {
                        pInfo.Remark = "重复抓取";
                    }
                }
                catch (Exception ex)
                {
                    pInfo.Remark = ex.Message;
                }
                finally
                {
                    lock (lockObj)
                    {
                        vm.ProgressValue+= pInfo.Count;
                    }
                }
            });
        }

        private void SetSaveAs(ObservableCollection<PictureInfo> pInfos, string targetPath)
        {
            pInfos.Select(x =>
           {
               var idParts = x.Id.Split('-');
               var lastFlag = "UnKnown";
               if (idParts.Length >= 1)
               {
                   lastFlag = idParts[0];
               }
               //savePath
               if (x.IsCatched)
               {
                   x.SaveFolder = Path.Combine(targetPath, lastFlag);
                   x.NameWithoutSuffix = x.Id;
               }
               return lastFlag;
           }).GroupBy(x => x).ForEach(x =>
           {
               if (!Directory.Exists(Path.Combine(targetPath, x.Key)))
               {
                   Directory.CreateDirectory(Path.Combine(targetPath, x.Key));
               }
           });
        }

        public void Export(PsViewModel vm,string path)
        {
            var mapper = new Mapper();
            mapper.Map<PictureInfo>("编号", x => x.Id)
                .Map<PictureInfo>("计数", x=>x.Count)
                .Map<PictureInfo>("是否已抓取",x=>x.IsCatched)
                .Map<PictureInfo>("备注", x => x.Remark);
            mapper.Save<PictureInfo>(path, vm.PictureInfos);
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

            Task.Factory.ContinueWhenAll(tasks, ancedents =>
            {
                SetSaveAs(infos, savePath);

                var pInfoCatched = infos.Where(x => x.IsCatched);
                //sum of qty
                vm.ProgressMax = pInfoCatched.Aggregate(0,(count,next)=> count+next.Count);
                vm.ProgressValue = 0;
                //sort
                var cloneTasks = pInfoCatched.Select(x => Clone(x, vm)).ToArray();

                Task.Factory.ContinueWhenAll(cloneTasks, subs =>
                {
                    //callback
                    Sort_CompletedEvent?.Invoke();
                });
            });
        }
    }
}
