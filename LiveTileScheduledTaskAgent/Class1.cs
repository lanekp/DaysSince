using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
 
namespace LiveTileScheduledTaskAgent
{
 
 /// <summary>Represents a scheduled task agent.</summary>
 public class ScheduledAgent : ScheduledTaskAgent
 {
    /// <summary>Indicates if the class has been initialized.</summary>
    private static volatile bool _classInitialized;
 
    /// <summary>Creates a new instance of <see cref="ScheduledAgent"/>.</summary>
    /// <remarks>ScheduledAgent constructor, initializes the UnhandledException handler</remarks>
     public ScheduledAgent()
     {
         if (!_classInitialized)
         {
            _classInitialized = true;
             // Subscribe to the managed exception handler
             Deployment.Current.Dispatcher.BeginInvoke(delegate { Application.Current.UnhandledException += ScheduledAgent_UnhandledException; } );
         }
     } //public ScheduledAgent()
 
     /// <summary>Raised when the task throws an exception that is unhandled.</summary>
     /// <param name="sender">The sender of the event.</param>
     /// <param name="e">The arguments of the event.</param>
     private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
     {
         if (System.Diagnostics.Debugger.IsAttached)
         {
            // An unhandled exception has occurred; break into the debugger
            System.Diagnostics.Debugger.Break();
         }
     } // private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
 
     /// <summary>Agent that runs a scheduled task.</summary>
     /// <param name="task">The invoked task.</param>
     /// <remarks>This method is called when a periodic or resource intensive task is invoked.</remarks>
     protected override void OnInvoke(ScheduledTask task)
     {
        // Get our application tile
        ShellTile tile = ShellTile.ActiveTiles.First();
 
        // Some data we'll use on the tile
        XElement xml = new XElement("Values", new XAttribute("value1", "123"), new XAttribute("value2", "456"), new XAttribute("value3", "789"));
 
        // We'll be using UIElements and WhiteableBitmaps to build an image so we have to call BeginInvoke
        Deployment.Current.Dispatcher.BeginInvoke(() =>
        {
            // If the agent crashes twice WP7 will shut us down so let make sure if there's an error we catch it
            try
            {
                // Find out if the user is in the light or dark theme
                bool isDarkTheme = Colors.White.Equals((Application.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush).Color);
 
                // Get the current accent color the user has selected
                SolidColorBrush accent = new SolidColorBrush((Color)Application.Current.Resources["PhoneAccentColor"]);
 
                /* Get the image that we'll use as the background of our tiles content included in the application as content
                * Image would not load if it was an embedded resource regardless of containing library */
                BitmapImage image = new BitmapImage(new Uri(isDarkTheme ? "DarkBackground.png" : "LightBackground.png", UriKind.RelativeOrAbsolute));
 
                // Without this the neither the ImageOpened nor the ImageFailed would fire within our allotted time to run
                image.CreateOptions = BitmapCreateOptions.None;
 
                // Important: DO NOT attempt any UIElement rendering until the ImageOpened event fires  
                image.ImageOpened += delegate(object sender, RoutedEventArgs args)
                {
                    // Size used for UIElement.Measure
                    Size size = new Size(173, 173);
 
                    // Rectangle used for UIElement.Arrange
                    Rect rect = new Rect(0, 0, 173, 173);
 
                    // Transform used to indicate the top left position of the UIElement in the image
                    TranslateTransform tileTran = new TranslateTransform() { X = 0, Y = 0 };
 
                    // The UIElement to build
                    MyTile tile = new MyTile();
 
                    /* Set the UIElement background to the users accent color
                    * We do this because the SaveJPEG extension kills transparency */
                    tile.LayoutRoot.Background = accent;
 
                    // Assign the loaded image to the Image control 
                    tile.BackgroundImage.Source = image;
 
                    // Add content to the MyTile instance
                    tile.Part1.Text = xml.Attribute("value1");
                    tile.Part2.Text = xml.Attribute("value2");
                    tile.Part3.Text = xml.Attribute("value3");
 
                    // We've changed the layout so let get it updated
                    tile.UpdateLayout();
 
                    // Measure the layout
                    tile.Measure(size);
 
                    // Layout may have been altered, let's get it updated again
                    tile.UpdateLayout();
 
                    // Arrange the layout
                    tile.Arrange(rect);
 
                    // Layout may have been altered, let's get it updated again
                    tile.UpdateLayout();
 
                    // Create a WriteableBitmap that is 173 pixels wide and 173 pixels tall
                    WriteableBitmap bitMap = new WriteableBitmap(173, 173);
 
                    // Render the UIElement to the WriteableBitmap
                    bitMap.Render(tile, tileTran);
 
                    // Calling invalidate actually causes the WriteableBitmap to draw the UIElement we passed to Render
                    bitMap.Invalidate();
 
                    // Now that we have an image all rendered with our content, we need to save it
                    using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        // We use the path "/Shared/ShellContent" because it is accessible to us here and the tile
                        var filename = "/Shared/ShellContent/BackTileBackground.jpg";
 
                        // If the image already exists, delete it
                        if (store.FileExists(filename)) { store.DeleteFile(filename); }
 
                        // Either the file wasn't there or we deleted it, now save the updated image there
                        using (var fs = store.CreateFile(filename))
                        {
                            // Both calls will work but I have seen posts with people saying that the extension wasn't saving the image
                            // bitMap.SaveJpeg(fs, 173, 173, 0, 100);
                            System.Windows.Media.Imaging.Extensions.SaveJpeg(bitMap, fs, 173, 173, 0, 100);
                        }
                    }
 
                    // Any values on the StandardTileData that are not set will not be effected
                    tile.Update(new StandardTileData
                    {
                        // "isostore:/" is handy little way to give a uri directly to our IsolatedStorage location
                        BackBackgroundImage = new Uri("isostore:/Shared/ShellContent/BackTileBackground.jpg", UriKind.Absolute)
                    });
                };  // image.ImageOpened += delegate(object sender, RoutedEventArgs args)
            } // try
            catch 
            { 
                /* Might load the original default but for this example not calling update will suffice */ 
            }
            finally
            { 
                NotifyComplete(); 
            }
        }); //Deployment.Current.Dispatcher.BeginInvoke(() =>
     } // OnInvoke(ScheduledTask task)
 } //public class ScheduledAgent : ScheduledTaskAgent
 } // namespace LiveTileScheduledTaskAgent




