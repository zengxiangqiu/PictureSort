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
    using NPOI.SS.Formula.Functions;
    using PictureSort.Repositories;
    using PictureSort.ViewModels;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly PsRepository repository = new PsRepository();
        CourseDef ColumnsDef;
        public MainWindow()
        {
            InitializeComponent();
            LoadConfig();
            this.DataContext = new PsViewModel();
            repository.Sort_CompletedEvent += Repository_Sort_CompletedEvent;
        }

        private void Repository_Sort_CompletedEvent()
        {
            Dispatcher.Invoke(() =>
            {
                var vm = this.DataContext as PsViewModel;
                MessageBox.Show($"已抓取{vm.ProgressValue}张图片", "Tips");
                btnSort.IsEnabled = true;
            });
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
                var infos = repository.GetPInfoFromExcel(vm.SourceFile);
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
            btnSort.IsEnabled = false;
        }

        private void LoadConfig()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CourseDef));
            ColumnsDef = (CourseDef)serializer.Deserialize(new XmlTextReader("config.xml"));
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var col = ColumnsDef.Items[0].Column.Where(x => x.Prop == e.PropertyName).FirstOrDefault();
            if (col != null)
            {
                e.Column.DisplayIndex = Convert.ToInt32(col.Index);
                e.Column.Header = col.DisplayName;
                e.Column.IsReadOnly = true;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as PsViewModel;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel 文件| *.xlsx";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    repository.Export(vm, saveFileDialog.FileName);
                    MessageBox.Show("成功导出");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("导出失败，原因：" + ex.Message);
                }
            }
        }
    }
}
