using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Input.Inking;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI.Popups;
using System.Collections.Generic;
using Windows.Globalization;
using Windows.UI.Text.Core;

namespace NavPaneApp1.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page1 : Page
    {
        const int minPenSize = 2;
        const int penSizeIncrement = 2;
        int penSize;

        InkRecognizerContainer inkRecognizerContainer = null;
        private IReadOnlyList<InkRecognizer> recoView = null;
        private Language previousInputLanguage = null;
        private CoreTextServicesManager textServiceManager = null;
        private ToolTip recoTooltip;

        public Page1()
        {
            this.InitializeComponent();

            penSize = minPenSize;// + penSizeIncrement * PenThickness.SelectedIndex;

            InkDrawingAttributes drawingAttributes = new InkDrawingAttributes();
            drawingAttributes.Color = Windows.UI.Colors.Red;
            drawingAttributes.Size = new Size(penSize, penSize);
            drawingAttributes.IgnorePressure = false;
            drawingAttributes.FitToCurve = true;

            inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
            inkCanvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse | Windows.UI.Core.CoreInputDeviceTypes.Pen | Windows.UI.Core.CoreInputDeviceTypes.Touch;
            inkCanvas.InkPresenter.StrokesCollected += InkPresenter_StrokesCollected;
            inkCanvas.InkPresenter.StrokesErased += InkPresenter_StrokesErased;


            // Show the available recognizers
            inkRecognizerContainer = new InkRecognizerContainer();
            recoView = inkRecognizerContainer.GetRecognizers();
            if (recoView.Count > 0)
            {
                foreach (InkRecognizer recognizer in recoView)
                {
                    RecoName.Items.Add(recognizer.Name);
                }
            }
            else
            {
                RecoName.IsEnabled = false;
                RecoName.Items.Add("No Recognizer Available");
            }
            RecoName.SelectedIndex = 0;
        }

        private void InkPresenter_StrokesErased(InkPresenter sender, InkStrokesErasedEventArgs args)
        {
            //rootPage.NotifyUser(args.Strokes.Count + " stroke(s) erased!", NotifyType.StatusMessage);

        }

        private void InkPresenter_StrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args)
        {
            //rootPage.NotifyUser(args.Strokes.Count + " stroke(s) collected!", NotifyType.StatusMessage);
        }


        async void AppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            
            // We don't want to save an empty file
            if (inkCanvas.InkPresenter.StrokeContainer.GetStrokes().Count > 0)
            {
                var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
                savePicker.FileTypeChoices.Add("Gif with embedded ISF", new System.Collections.Generic.List<string> { ".gif" });

                Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
                if (null != file)
                {
                    try
                    {
                        using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            
                            await inkCanvas.InkPresenter.StrokeContainer.SaveAsync(stream);
                        }
                        ShowMessage(inkCanvas.InkPresenter.StrokeContainer.GetStrokes().Count + " stroke(s) saved!");
                    }
                    catch (Exception ex)
                    {
                        ShowMessage(ex.Message);
                    }
                }
            }
            else
            {
                ShowMessage("Não tem ink para salvar.");
            }

            
        }

        async void ShowMessage(string message)
        {
            MessageDialog msg = new MessageDialog(message); 
            await msg.ShowAsync();
        }
        private void OnRecognizerChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Clear_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            inkCanvas.InkPresenter.StrokeContainer.Clear();
        }

        async void ReconecerEscrita_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IReadOnlyList<InkStroke> currentStrokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();
            if (currentStrokes.Count > 0)
            {
 
                var recognitionResults = await inkRecognizerContainer.RecognizeAsync(inkCanvas.InkPresenter.StrokeContainer, InkRecognitionTarget.All);

                if (recognitionResults.Count > 0)
                {
                    // Display recognition result
                    string str = "Resultado do reconhecimento de escrita:";
                    foreach (var r in recognitionResults)
                    {
                        str += " " + r.GetTextCandidates()[0];
                    }
                    ShowMessage(str);
                }
                else
                {
                    ShowMessage("Nenhum texto reconhecido.");
                }

            }
            else
            {
                ShowMessage("Por favor, escreva alguma palavra no Ink.");
            }
        }
    }
}
