using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace NavPaneApp1.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page3 : Page
    {
        public Page3()
        {
            this.InitializeComponent();
        }

        private void button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150ImageAndText01);

            XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");
            tileTextAttributes[0].InnerText = "Hello World! My very own tile notification";

            XmlNodeList tileImageAttributes = tileXml.GetElementsByTagName("image");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("src", "ms-appx:///assets/redWide.png");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("alt", "red graphic");

            XmlDocument squareTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text04);
            XmlNodeList squareTileTextAttributes = squareTileXml.GetElementsByTagName("text");
            squareTileTextAttributes[0].AppendChild(squareTileXml.CreateTextNode("Hello World! My very own tile notification"));
            IXmlNode node = tileXml.ImportNode(squareTileXml.GetElementsByTagName("binding").Item(0), true);
            tileXml.GetElementsByTagName("visual").Item(0).AppendChild(node);

            TileNotification tileNotification = new TileNotification(tileXml);

            tileNotification.ExpirationTime = DateTimeOffset.UtcNow.AddSeconds(10);

            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);

        }

        private void btnLocatToast_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText01;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode("Hello World!"));
            XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");
            ((XmlElement)toastImageAttributes[0]).SetAttribute("src", "ms-appx:///assets/redWide.png");
            ((XmlElement)toastImageAttributes[0]).SetAttribute("alt", "red graphic");

            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private void teste()
        {
            string title = "Você tem uma reserva";
            string content = "Check this out, Happy Canyon in Utah!";
            string image = "http://blogs.msdn.com/cfs-filesystemfile.ashx/__key/communityserver-blogs-components-weblogfiles/00-00-01-71-81-permanent/2727.happycanyon1_5B00_1_5D00_.jpg";

            string toastActions2 =
            $@"<toast>
                <visual>
                    <binding template='ToastGeneric'>
                        <text>{title}</text>
                        <text>{content}</text>
                        <image src='{image}' placement='appLogoOverride' hint-crop='circle'/>
                    </binding>
                </visual>
                <actions>
                    <input id = 'time' type = 'selection' defaultInput='2'  />
                    <selection id = '1' content='Café da manhã' />
                    <selection id = '2' content = 'Almoço' />
                    <selection id = '3' content = 'Jantar' />
                    <action activationType = 'background' content = 'Reserve' arguments = 'reservar' />
                    <action activationType = 'foreground' content = 'Ligar para o restaurante' arguments = 'ligar' />
                </actions>
            </toast>";

            XmlDocument x2 = new XmlDocument();
            x2.LoadXml(toastActions2);
            ToastNotification t2 = new ToastNotification(x2);
            ToastNotificationManager.CreateToastNotifier().Show(t2);
        }

        private void btnLocatToastWithAction_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            string title = "Você recebeu uma imagem";
            string content = "Check this out, Happy Canyon in Utah!";
            string image = "http://blogs.msdn.com/cfs-filesystemfile.ashx/__key/communityserver-blogs-components-weblogfiles/00-00-01-71-81-permanent/2727.happycanyon1_5B00_1_5D00_.jpg";

            string toastActions =
            $@"<toast>
                <visual>
                    <binding template='ToastGeneric'>
                        <text>{title}</text>
                        <text>{content}</text>
                        <image src='{image}' placement='appLogoOverride' hint-crop='circle'/>
                    </binding>
                </visual>
                <actions>
                    <input id = 'message' type = 'text' placeholderContent = 'reply here' />
                    <action activationType = 'background' content = 'reply' arguments = 'reply' />
                    <action activationType = 'foreground' content = 'video call' arguments = 'video' />
                </actions>
            </toast>";

            XmlDocument x = new XmlDocument();
            x.LoadXml(toastActions);
            ToastNotification t = new ToastNotification(x);
            ToastNotificationManager.CreateToastNotifier().Show(t);
        }

        private void btnLocatToastWithAction_Click_1(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            teste();
        }
    }
}
