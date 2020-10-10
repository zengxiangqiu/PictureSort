using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace PictureSort.ViewModels
{
    using PictureSort.Models;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class PsViewModel : INotifyPropertyChanged
    {
        private string searchFolder = "";

        private string sourceFile = "";

        public string SourceFile
        {
            get { return sourceFile; }
            set
            {
                sourceFile = value;
                NotifyPropertyChanged();
            }
        }


        public string SearchFolder
        {
            get { return searchFolder; }
            set
            {
                searchFolder = value;
                NotifyPropertyChanged();
            }
        }

        private string saveFolder = "";

        public string SaveFolder
        {
            get { return saveFolder; }
            set
            {
                saveFolder = value;
                NotifyPropertyChanged();
            }
        }

        private double _progressValue;

        public double ProgressValue
        {
            get { return _progressValue; }
            set
            {
                _progressValue = value;
                NotifyPropertyChanged();
            }
        }

        private double _progressMax;

        public double ProgressMax
        {
            get { return _progressMax; }
            set
            {
                _progressMax = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<PictureInfo> PictureInfos { get; set; } = new ObservableCollection<PictureInfo>();

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
