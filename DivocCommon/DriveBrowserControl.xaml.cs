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
    /// Event args for the DriveItemSelected event.
    /// </summary>
    public class DriveItemSelectedArgs : EventArgs
    {
        public DriveItem Item { get; private set; }

        public DriveItemSelectedArgs(DriveItem item)
        {
            Item = item;
        }
    }

    /// <summary>
    /// Interaction logic for DriveBrowserControl.xaml
    /// </summary>
    public partial class DriveBrowserControl : UserControl
    {
        public event EventHandler<DriveItemSelectedArgs> DriveItemSelected;
        public event EventHandler BrowseCanceled;

        public ObservableCollection<DriveItem> Items { get; set; }
        public DriveItem SelectedItem { get; private set; }
        public DriveItem ParentItem { get; private set; }
        public DriveItem PreviousParentItem { get; private set; }
        private ContentManager _contentMgr = null;
        private List<string> _fileTypes = null;
        private bool _location = false; // are we looking for a location to put something in?

        public Stack<DriveItem> ParentChain = new Stack<DriveItem>();

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

            ParentItem = _contentMgr.Root;

            ParentChain.Push(_contentMgr.Root);

            (await _contentMgr.GetDocumentsREST(fileTypes: _fileTypes)).ForEach(Items.Add);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CheckOpenButtonEnablement();
        }

        private void CheckOpenButtonEnablement()
        {
            SelectedItem = listItems.SelectedItem as DriveItem;

            openBtn.IsEnabled = (SelectedItem == null) || !_location || (SelectedItem.Folder != null);
        }

        private void ListItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckOpenButtonEnablement();
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectedItem = listItems.SelectedItem as DriveItem;

            if (_location)
            {
                DriveItemSelected?.Invoke(this, new DriveItemSelectedArgs(SelectedItem ?? ParentChain.Pop()));
            }
            else
            {
                DriveItemSelected?.Invoke(this, new DriveItemSelectedArgs(SelectedItem));
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            BrowseCanceled?.Invoke(this, e);
        }

        private async void ListItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataCtx = ((FrameworkElement)e.OriginalSource).DataContext;

            if(dataCtx is DriveItem)
            {
                DriveItem selItem = listItems.SelectedItem as DriveItem;

                if(selItem.Folder != null)
                {
                    // Do drill-down.
                    PreviousParentItem = ParentItem;
                    ParentItem = selItem;

                    ParentChain.Push(selItem);
                    upBtn.Visibility = Visibility.Visible;  // need to do proper binding for this

                    string parentId = selItem.Id;
                    Items.Clear();
                    (await _contentMgr.GetDocumentsREST(parentId, _fileTypes)).ForEach(Items.Add);
                }
                else
                {
                    SelectedItem = selItem;

                    if(!_location)
                    {
                        // Not a folder item, and we are not in location selection mode,
                        // so we'll assume the user wants to select it if they're double-clicking on it
                        DriveItemSelected?.Invoke(this, new DriveItemSelectedArgs(selItem));
                    }
                }
            }
        }

        private async void UpBtn_Click(object sender, RoutedEventArgs e)
        {
            ParentChain.Pop();

            if (ParentChain.Count <= 1) upBtn.Visibility = Visibility.Hidden;  // need to do proper binding for this

            Items.Clear();
            (await _contentMgr.GetDocumentsREST(ParentChain.Peek().Id, _fileTypes)).ForEach(Items.Add);
        }
    }
}
