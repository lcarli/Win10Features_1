using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;
using Windows.Storage;

namespace NavPaneApp1.Views
{
    public class Palestrante
    {
        public string nome { get; set; }
        public string imageUri { get; set; }
    }

    public sealed partial class Page2 : Page
    {
        List<Palestrante> ListSource;
        List<Palestrante> ListDestination;
        string _deletedItem;

        public Page2()
        {
            this.InitializeComponent();
            ListSource = new List<Palestrante>();
            ListDestination = new List<Palestrante>();
            ListSource.Add(new Palestrante() { nome = "CURURU 1", imageUri = "/Assets/mri.jpg" });
            ListSource.Add(new Palestrante() { nome = "CURURU 2", imageUri = "/Assets/mri.jpg" });
            ListSource.Add(new Palestrante() { nome = "CURURU 3", imageUri = "/Assets/mri.jpg" });
            ListDestination.Add(new Palestrante() { nome = "CURURU 3", imageUri = "/Assets/mri.jpg" });

            //_reference = GetSampleData();
            //_selection = new ObservableCollection<string>();
            SourceListView.ItemsSource = ListSource;
            //TargetListView.ItemsSource = ListDestination;
        }

        private ObservableCollection<string> GetSampleData()
        {
            return new ObservableCollection<string>
            {
                "Item 1",
                "Item 2",
                "Item 3",
                "Item 4",
                "Item 5",
                "Item 6",
                "Item 7",
                "Item 8",
            };
        }
        /// <summary>
        /// DragItemsStarting is called when the Drag and Drop operation starts
        /// We take advantage of it to set the content of the DataPackage
        /// as well as indicate which operations are supported
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SourceListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            var items = string.Join(",", e.Items.Cast<Palestrante>().Select(i => i.nome));
            e.Data.SetText(items);
            //e.Data.SetData("StandardDataFormats.Storage", )
            //// Prepare a string with one dragged item per line
            //var items = new StringBuilder();
            //foreach (var item in e.Items)
            //{
            //    if (items.Length > 0) items.AppendLine();
            //    items.Append(item as string);
            //}
            //// Set the content of the DataPackage
            //e.Data.SetText(items.ToString());
            // As we want our Reference list to say intact, we only allow Copy
            e.Data.RequestedOperation = DataPackageOperation.Copy;
        }

        /// <summary>
        /// DragOver is called when the dragged pointer moves over a UIElement with AllowDrop=True
        /// We need to return an AcceptedOperation != None in either DragOver or DragEnter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TargetListView_DragOver(object sender, DragEventArgs e)
        {
            // Our list only accepts text
            e.AcceptedOperation = (e.DataView.Contains(StandardDataFormats.Text)) ? DataPackageOperation.Copy : DataPackageOperation.None;
        }

        /// <summary>
        /// We need to return the effective operation from Drop
        /// This is not important for our source ListView, but it might be if the user
        /// drags text from another source
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TargetListView_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.Text))
            {
                var def = e.GetDeferral();
                var id = await e.DataView.GetTextAsync();
                var itemIdsToMove = id.Split(',');

                var destinationListView = sender as ListView;
                var listViewItemsSource = destinationListView?.ItemsSource as List<Palestrante>;

                if (listViewItemsSource != null)
                {
                    foreach (var itemId in itemIdsToMove)
                    {
                        var itemToMove = ListSource.First(i => i.nome.ToString() == itemId);

                        ListDestination.Add(itemToMove);
                        //ListSource.Remove(itemToMove);
                    }
                }
                e.AcceptedOperation = DataPackageOperation.Copy;
                def.Complete();
            }
        }

        /// <summary>
        /// DragtemsStarting is called for D&D and reorder as the framework does not
        /// know wherer the user will drop the items. Reorder means that the target
        /// and the source ListView are the same.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TargetListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            // The ListView is declared with selection mode set to Single.
            // But we want the code to be robust
            if (e.Items.Count == 1)
            {
                e.Data.SetText(e.Items[0] as string);
                // Reorder or move to trash are always a move
                e.Data.RequestedOperation = DataPackageOperation.Move;
                _deletedItem = null;
            }
        }

        /// <summary>
        /// Called at the end of the operation, whether it was a reorder or move to trash
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void TargetListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            // args.DropResult is always Move and therefore we have to rely on _deletedItem to distinguish
            // between reorder and move to trash
            // Another solution would be to listen for events in the ObservableCollection
            if (_deletedItem != null)
            {
                //ListDestination.Remove(_deletedItem);
                _deletedItem = null;
            }
        }

        /// <summary>
        /// Entering the Trash icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TargetTextBlock_DragEnter(object sender, DragEventArgs e)
        {
            // Trash only accepts text
            e.AcceptedOperation = (e.DataView.Contains(StandardDataFormats.Text) ? DataPackageOperation.Move : DataPackageOperation.None);
            // We don't want to show the Move icon
            e.DragUIOverride.IsGlyphVisible = false;
            e.DragUIOverride.Caption = "Coloque o item aqui para remover da lista de itens selecionados";
        }


        /// <summary>
        /// Drop on the Trash
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TargetTextBlock_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.Text))
            {
                // We need to take the deferral as the source will read _deletedItem which
                // we cannot set synchronously
                var def = e.GetDeferral();
                _deletedItem = await e.DataView.GetTextAsync();
                e.AcceptedOperation = DataPackageOperation.Move;
                def.Complete();
            }
        }
    }
}
