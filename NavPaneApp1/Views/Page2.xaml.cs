﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace NavPaneApp1.Views
{
    public class Palestrante
    {
        public string nome { get; set; }
        public string imageUri { get; set; }
    }

    public sealed partial class Page2 : Page
    {
        ObservableCollection<Palestrante> ListSource;
        ObservableCollection<Palestrante> ListDestination;
        public ObservableCollection<MyItem> MyItems { get; private set; } = new ObservableCollection<MyItem>();
        string _deletedItem;

        public Page2()
        {
            this.InitializeComponent();
            ListSource = new ObservableCollection<Palestrante>();
            ListDestination = new ObservableCollection<Palestrante>();
            ListSource.Add(new Palestrante() { nome = "Sample Text 1", imageUri = "/Assets/mri.jpg" });
            ListSource.Add(new Palestrante() { nome = "Sample Text 2", imageUri = "/Assets/blueback.jpg" });
            ListSource.Add(new Palestrante() { nome = "Sample Text 3", imageUri = "/Assets/redWide.png" });

            SourceListView.ItemsSource = ListSource;
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
                var listViewItemsSource = destinationListView?.ItemsSource as ObservableCollection<Palestrante>;

                if (listViewItemsSource != null)
                {
                    foreach (var itemId in itemIdsToMove)
                    {
                        var itemToMove = ListSource.First(i => i.nome.ToString() == itemId);
                        listViewItemsSource.Add(itemToMove);
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
                var items = string.Join(",", e.Items.Cast<Palestrante>().Select(i => i.nome));
                e.Data.SetText(items);
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
                var def = e.GetDeferral();
                var id = await e.DataView.GetTextAsync();
                var itemIdsToMove = id.Split(',');

                var listViewItemsSource = TargetListView.ItemsSource as ObservableCollection<Palestrante>;

                if (listViewItemsSource != null)
                {
                    foreach (var itemId in itemIdsToMove)
                    {
                        var itemToMove = ListDestination.First(i => i.nome.ToString() == itemId);
                        listViewItemsSource.Remove(itemToMove);
                        ListDestination.Remove(itemToMove);
                    }
                }
                e.AcceptedOperation = DataPackageOperation.Move;
                def.Complete();
            }
        }

        private async void imageContainer_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var storageItems = await e.DataView.GetStorageItemsAsync();

                foreach (StorageFile storageItem in storageItems)
                {
                    var bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(await storageItem.OpenReadAsync());

                    var myItem = new MyItem
                    {
                        Id = Guid.NewGuid(),
                        Image = bitmapImage
                    };

                    this.MyItems.Add(myItem);

                }
            }
        }

        private void imageContainer_DragEnter(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "Coloque o item aqui para copiá-lo para sua aplicação";
        }
    }
}
