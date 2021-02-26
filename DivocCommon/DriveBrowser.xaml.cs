using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public partial class DriveBrowser : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<DriveItem> Items { get; set; }
        public DriveItem SelectedItem { get; private set; }

        private DriveItem _parentItem;
        public DriveItem ParentItem 
        {
            get { return _parentItem; }

            private set
            {
                _parentItem = value;
                OnPropertyChanged();
            }
        }

        private Visibility _upVisibility;
        public Visibility UpVisibility
        {
            get { return _upVisibility; }

            set
            {
                _upVisibility = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public DriveItem PreviousParentItem { get; private set; }
        private ContentManager _contentMgr = null;
        private List<string> _fileTypes = null;
        private bool _saving = false; // are we looking for a location to put something in?

        public string OpenLabel
        {
            get
            {
                return ResourceBroker.GetString(_saving ? ResourceBroker.ResourceID.SAVE_LABEL : ResourceBroker.ResourceID.OPEN_LABEL);
            }
        }

        public Stack<DriveItem> ParentChain = new Stack<DriveItem>();

        public DriveBrowser(ContentManager contentManager, List<string> fileTypes = null, bool saving = false, IntPtr wnd = default)
        {
            InitializeComponent();
            DataContext = this;
            Items = new ObservableCollection<DriveItem>();
            listItems.ItemsSource = Items;

            if(wnd != null)
            {
                var interopHelper = new System.Windows.Interop.WindowInteropHelper(this)
                {
                    Owner = wnd
                };
            }

            _contentMgr = contentManager;
            _fileTypes = fileTypes;
            _saving = saving;

            ParentItem = _contentMgr.Root;

            ParentChain.Push(_contentMgr.Root);

            UpVisibility = Visibility.Collapsed;
        }

        public async Task<bool> Init()
        {
            (await _contentMgr.GetDocumentsREST(fileTypes: _fileTypes)).ForEach(Items.Add);

            return true;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await Init();

            CheckOpenButtonEnablement();
        }

        private void CheckOpenButtonEnablement()
        {
            SelectedItem = listItems.SelectedItem as DriveItem;

            if(_saving)
            {
                if (SelectedItem == null || SelectedItem.Folder != null)
                    openBtn.IsEnabled = true;
                else
                    openBtn.IsEnabled = false;
            }
            else
            {
                if (SelectedItem != null)
                    openBtn.IsEnabled = true;
                else
                    openBtn.IsEnabled = false;
            }
        }

        private void ListItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckOpenButtonEnablement();
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            DriveItem item = listItems.SelectedItem as DriveItem;

            if(_saving)
            {
                if(item == null && ParentChain.Count == 1)
                {
                    // Should only ever get here in the case of the user being at the root,
                    // but we need to get that returned
                    DoSelectItem(ParentChain.Pop());
                }
                else if(item.Folder != null)
                {
                    DoSelectItem(item);
                } // NO else, open button will be disabled for documents when in save mode
            }
            else
            {
                if (item.Folder == null)
                    DoSelectItem(item);
                else
                    DoDrillDown(item);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private async void DoDrillDown(DriveItem parent)
        {
            if(parent != null && parent.Folder != null)
            {
                PreviousParentItem = ParentItem;
                ParentItem = parent;

                ParentChain.Push(parent);

                UpVisibility = Visibility.Visible;

                string parentId = parent.Id;
                Items.Clear();
                (await _contentMgr.GetDocumentsREST(parentId, _fileTypes)).ForEach(Items.Add);
            }
        }

        private void DoSelectItem(DriveItem item)
        {
            if(item != null)
            {
                SelectedItem = item;
                this.DialogResult = true;
            }
        }

        private void ListItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataCtx = ((FrameworkElement)e.OriginalSource).DataContext;

            if(dataCtx is DriveItem)
            {
                DriveItem item = dataCtx as DriveItem;

                if(_saving)
                {
                    // if folder, drill down
                    if (item.Folder != null)
                        DoDrillDown(item);
                    else
                        DoSelectItem(item);
                }
                else
                {
                    // if document, open it
                    if (item.Folder == null)
                        DoSelectItem(item);
                    else
                        DoDrillDown(item);
                }
            }
        }

        private async void UpBtn_Click(object sender, RoutedEventArgs e)
        {
            ParentChain.Pop();

            UpVisibility = (ParentChain.Count > 1) ? Visibility.Visible : Visibility.Collapsed;

            Items.Clear();


            (await _contentMgr.GetDocumentsREST(ParentChain.Peek().Id, _fileTypes)).ForEach(Items.Add);
            ParentItem = ParentChain.Peek();
        }
    }
}
