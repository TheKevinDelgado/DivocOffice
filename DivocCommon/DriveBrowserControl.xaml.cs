using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using DivocCommon.DataModel;

namespace DivocCommon
{
    /// <summary>
    /// Interaction logic for DriveBrowserControl.xaml
    /// </summary>
    public partial class DriveBrowserControl : UserControl
    {
        public event EventHandler<DriveItem> DriveItemSelected;
        public event EventHandler BrowseCanceled;

        public ObservableCollection<DriveItem> Items { get; set; }
        public DriveItem SelectedItem { get; private set; }
        private ContentManager _contentMgr = null;
        private List<string> _fileTypes = null;
        private bool _location = false;

        public DriveBrowserControl()
        {
            InitializeComponent();
            DataContext = this;
            Items = new ObservableCollection<DriveItem>();
            listItems.ItemsSource = Items;
        }

        public async void Init(ContentManager contentManager, List<string> fileTypes, bool location = false)
        {
            _contentMgr = contentManager;
            _fileTypes = fileTypes;
            _location = location;

            (await _contentMgr.GetDocumentsREST(fileTypes: _fileTypes)).ForEach(Items.Add);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CheckOpenButtonEnablement();
        }

        private void CheckOpenButtonEnablement()
        {
            SelectedItem = listItems.SelectedItem as DriveItem;

            openBtn.IsEnabled = (SelectedItem == null) || (_location ? (SelectedItem.folder != null) : true);
        }

        private void listItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckOpenButtonEnablement();
        }

        private void openBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectedItem = listItems.SelectedItem as DriveItem;

            DriveItemSelected?.Invoke(this, SelectedItem);
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            BrowseCanceled?.Invoke(this, e);
        }

        private async void listItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataCtx = ((FrameworkElement)e.OriginalSource).DataContext;

            if(dataCtx is DriveItem)
            {
                SelectedItem = listItems.SelectedItem as DriveItem;
                string parentId = SelectedItem.id;
                Items.Clear();
                (await _contentMgr.GetDocumentsREST(parentId, _fileTypes)).ForEach(Items.Add);

            }
        }
    }
}
