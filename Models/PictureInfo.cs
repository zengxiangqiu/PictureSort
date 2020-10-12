using Npoi.Mapper.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PictureSort.Models
{
    public class PictureInfo : INotifyPropertyChanged
    {
        public string Id { get; set; }

        public int Count { get; set; } = 1;
        private bool _isCatched = false;
        public bool IsCatched
        {
            get
            {
                return _isCatched;
            }
            set
            {
                _isCatched = value;
                NotifyPropertyChanged();
            }
        }
        public string Remark { get; set; }
        [Ignore]
        public string SubFolder { get; set; }
        [Ignore]
        public string CopyFrom { get; set; }

        [Ignore]
        public string SaveAs { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
