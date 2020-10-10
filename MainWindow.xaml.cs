using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace PictureSort
{
    using PictureSort.Repositories;
    using PictureSort.ViewModels;
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly PsRepository repository = new PsRepository();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new PsViewModel();
            repository.updatePbDelegate = new PsRepository.UpdateProgressBarDelegate(ProgressBar1.SetValue);
        }

        private void btnImport_click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as PsViewModel;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx;*.xls";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                //Get the path of specified file
                var filePath = openFileDialog.FileName;
                vm.SourceFile = filePath;
                var infos = repository.ImportSource(vm.SourceFile);
                vm.PictureInfos.Clear();
                infos.ToList().ForEach(vm.PictureInfos.Add);
                ProgressBar1.Maximum = infos.Count();
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            var vm = this.DataContext as PsViewModel;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                vm.SearchFolder =dialog.FileName;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            var vm = this.DataContext as PsViewModel;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                vm.SaveFolder = dialog.FileName;
            }
        }

        private void btnSort_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as PsViewModel;
            repository.Compare(vm);
        }
    }
}
