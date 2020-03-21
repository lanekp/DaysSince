////#define DEBUG_AGENT

using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;                // ShellTile
using System.IO.IsolatedStorage;            // for isolated storage
using System.Windows.Media.Imaging;         // WriteableBitmap
using Microsoft.Phone.Scheduler;            // ScheduledAgent
using DaysSinceClassLibrary;
//using DaysSince.Resources;


namespace LiveTileScheduledTaskAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        //const int c_n173 = 173;// 173 pixels is the height and width of a WP7 tile image.
        const int c_n336 = 336;  // 336 pixels is the height and width of a WP8 medium tile image.

        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }


        protected override void OnInvoke(ScheduledTask task)
        {
            // NOTE: determine if this needs to run.
            // maybe save the "number of days" in isostorage and check it to see if it has changed?

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            DateTime dtDaysSinceDate = DateTime.Now;
            bool bWasSaveSelectedDateChecked = false;

            // only load a 'selected' date if gbSaveDate == true
            if (!settings.TryGetValue<bool>("saveSelectedDateIsChecked", out bWasSaveSelectedDateChecked))
            {
                NotifyComplete();
                return;
            }

            if (!settings.TryGetValue<DateTime>("selectdate", out dtDaysSinceDate))
            {
                NotifyComplete();
                return;
            }

            // We'll be using UIElements and WriteableBitmaps to build an image so we have to call BeginInvoke
            Deployment.Current.Dispatcher.BeginInvoke( () =>
            {
                // If the agent crashes twice WP7 will shut us down so let make sure if there's an error we catch it
                try
                {
                    ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault();
                    if (tile != null)
                    {
                        FormatResultsClass frc = new FormatResultsClass();

                        FlipTileData flipTile = new FlipTileData();
                        flipTile.Title = LiveTileScheduledTaskAgentResources.AppTitle;
                        flipTile.BackTitle = LiveTileScheduledTaskAgentResources.AppTitle;

                        flipTile.BackContent = " ";
                        flipTile.WideBackContent = " ";

                        String strBackContent = "";
                        frc.FormatLiveTileText( dtDaysSinceDate, DateTime.Now, out strBackContent ); 

                        //Medium size Tile 336x336 px
                        //Create image for BackBackgroundImage in IsoStore
                        frc.RenderText(strBackContent, c_n336, c_n336, 28, "BackBackgroundImage");

                        flipTile.BackBackgroundImage = new Uri(@"isostore:/Shared/ShellContent/BackBackgroundImage.jpg", UriKind.Absolute); //Generated image for Back Background 336x336
                        flipTile.BackgroundImage = new Uri("/Content/MediumBlankTileImage336x336.png", UriKind.Relative); //Default image for Background Image Medium Tile 336x336 px
                        //End Medium size Tile 336x336 px

                        //Wide size Tile 691x336 px
                        //flipTile.WideBackgroundImage = new Uri("/Content/WideBlankTileIcon691x336.png", UriKind.Relative); // Default image for Background Image Wide Tile 691x336 px
                        flipTile.WideBackgroundImage = new Uri("/Content/WideBackBackgroundTileIcon691x336WithGlow.png", UriKind.Relative); // Default image for Background Image Wide Tile 691x336 px

                        //Crete image for WideBackBackgroundImage in IsoStore
                        frc.RenderText(strBackContent, 691, c_n336, 40, "WideBackBackgroundImage");
                        flipTile.WideBackBackgroundImage = new Uri(@"isostore:/Shared/ShellContent/WideBackBackgroundImage.jpg", UriKind.Absolute);

                        //End Wide size Tile 691x336 px

                        //Update Live Tile
                        tile.Update(flipTile);
                    }
                } // try
            
                // catch
                catch (Exception e)
                {
                    //MessageBox.Show( e.ToString() );
                }
                finally
                {
                    NotifyComplete();
                } // finally 

            }); //Deployment.Current.Dispatcher.BeginInvoke(() =>
        } // OnInvoke(ScheduledTask task)        


    /******************** Original, pre-WP8 code
    protected override void OnInvoke(ScheduledTask task)
    {
        // NOTE: determine if this needs to run.
        // maybe save the "number of days" in isostorage and check it to see if it has changed?

        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        DateTime dtDaysSinceDate = DateTime.Now;
        bool bWasSaveSelectedDateChecked = false;

        // only load a 'selected' date if gbSaveDate == true
        if ( !settings.TryGetValue<bool>("saveSelectedDateIsChecked", out bWasSaveSelectedDateChecked) )
        {
            NotifyComplete();
            return;
        }

        if ( !settings.TryGetValue<DateTime>("selectdate", out dtDaysSinceDate) )
        {
            NotifyComplete();
            return;
        }

        // We'll be using UIElements and WhiteableBitmaps to build an image so we have to call BeginInvoke
        Deployment.Current.Dispatcher.BeginInvoke( () =>
        {
            // If the agent crashes twice WP7 will shut us down so let make sure if there's an error we catch it
            try
            {
                var bmp  = new WriteableBitmap(c_n336, c_n336);
                var logo = new BitmapImage(new Uri("/Content/MediumBlankBackTileImage336x336.png", UriKind.Relative));

                // Force the bitmapimage to load it's properties so the transform will work
                // Without this the neither the ImageOpened nor the ImageFailed would fire within our allotted time to run
                logo.CreateOptions = BitmapCreateOptions.None;

                // Important: DO NOT attempt any UIElement rendering until the ImageOpened event fires  
                logo.ImageOpened += delegate(object sender, RoutedEventArgs args)
                {
                    String strBackContent = "";

                    FormatResultsClass frc = new FormatResultsClass();
                    frc.FormatLiveTileText( dtDaysSinceDate, DateTime.Now, out strBackContent ); 

                    BitmapImage bm = (BitmapImage)sender;

                    var imgImage = new Image { Source = bm };

                    WriteableBitmap wbm = new WriteableBitmap( bm );
                    var tt = new TranslateTransform();
                    tt.X = c_n336; 
                    tt.Y = c_n336; 

                    //
                    //Next, we create the UI Elements that we want to push into our WriteableBitmap using the Render method.  
                    //We will need to format our Tile to our liking.  
                    //We will also need to add our image to our bitmap and position it properly.  
                    //In order to do that, we will need to use Transforms.  
                    //I’m lazy and uncreative, so I will use a TranslateTransform, but the idea is the same even if you use a MatrixTransform.             
                    //


                    var tbText = new TextBlock();
                    tbText.Height = c_n336;
                    tbText.Width =  c_n336;
                    tbText.Foreground = new SolidColorBrush(Colors.White);
                    tbText.FontFamily = new FontFamily("Arial Narrow");
                    tbText.FontSize = 30; 
                    tbText.TextWrapping = TextWrapping.Wrap;
                    tbText.Text = strBackContent;
                    tbText.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    tbText.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            
                    wbm.Render(imgImage, tt);
                    wbm.Render(tbText, null);
                    wbm.Invalidate();

                    //*
                    //Next is a little bit of complexity with IsolatedStorage.  
                    //In addition to providing local storage for our application, on the Phone, 
                    //IsolatedStorage has some shared areas.  
                    //Specifically, the Shared/ShellContent section.  
                    //We need to store our Bitmap in this area in order to be used by a Tile.             * 
                    //
                    var filename = "/Shared/ShellContent/testtile.jpg";
                    using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (var st = new IsolatedStorageFileStream(filename,
                                                                       System.IO.FileMode.Create,
                                                                       System.IO.FileAccess.Write,
                                                                       store))
                        {
                            wbm.SaveJpeg(st, c_n336, c_n336, 0, 100);
                        }
                    }

                    // Application Tile is always the first Tile, even if it is not pinned to Start.
                    ShellTile TileToFind = ShellTile.ActiveTiles.First();

                    // Application should always be found
                    if (null != TileToFind)
                    {
                        // set the properties to update for the Application Tile
                        // Empty strings for the text values and URIs will result in the property being cleared.
                        FlipTileData NewTileData = new FlipTileData
                        {
                            //BackContent = strBackContent,
                            WideBackContent = strBackContent,
                            BackBackgroundImage = new Uri("isostore:" + filename, UriKind.Absolute),
                        };

                        // Update the Application Tile
                        TileToFind.Update(NewTileData);
                    }
                }; // void OnImageOpened(object sender, RoutedEventArgs e)
            } // try
            catch
            {
            }
            finally
            {
                NotifyComplete();
            }
        } ); //Deployment.Current.Dispatcher.BeginInvoke(() =>
    } // OnInvoke(ScheduledTask task)        
     ******************************************/   
        
    } // class ScheduledAgent : ScheduledTaskAgent
} // namespace LiveTileScheduledTaskAgent
