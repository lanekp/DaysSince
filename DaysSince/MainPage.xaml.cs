#define DEBUG_AGENT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;                  // Binding class
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
using DaysSince.Resources;

 
 // TO DO: 
 // 04/13/13: figure out how to solve the problem of ScheduledAgents need to be renewed every 14 days.
 //              ie, scheduled tasks expire every 14 days. Need to solve this problem.
 // 03/24/13: figure out how to elegantly format output for 3 scenarios: days since, date diff and back content
 // 03/24/13: format back tile content to fit on tile: create bitmap? reduce font? reduce amount of text?
 // 02/19/13: improve fonts, colors of output text
 // 04/13/13: convert to WP8 project.
 
 // DONE
 // 02/10/13: Change project name to 'Days Since'
 // 02/09/13: Change the 'x' ok/cancel icons on datepicker
 // 02/10/13: Set default date to today's date
 // 02/10/13: Finished DateDiff functionality
 // 02/19/13: Finished testing all logical paths
 // 03/11/13: Add functionality to checkbox "Keep selected date as default"
 // 02/09/13: live tiles
 // 02/18/13: fix formatting of text (on main page) for Date Diff and Days Since. 
 // 02/23: select a tile style (flip?, should it be 
 // 03/24/13: show 'days since on live tiles when 'keep selected date as default' checked.
 // 04/13/13: Create a logo to put on the tile
 // 04/06/13: Put a triangle in the tile .png file
 // 04/13/13: figure out how to schedule notifications
 // 05/20/13: Add an 'about' app bar item: should include instructions on .5 hr updates, contact info, etc.
 // 04/06/13: get a new splashscreen
 // 03/24/13: add a prayer page
 // 04/23/13: add option to use European date format
 // 04/13/13: if user takes off 'live tile' must delete contents of back tile.


namespace DaysSince
{
    public partial class MainPage : PhoneApplicationPage
    {
        const int     c_n336 = 336; // the height and width of a tile, in pixels
        //enum eFormatType { MAINPAGE = 1, BACKTILE = 2, DATEDIFF = 3 };
        // Scheduled Task variables
        string periodicTaskName = "PeriodicAgent";
        PeriodicTask periodicTask;
        bool agentsAreEnabled = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Code to localize the ApplicationBar
            BuildLocalizedApplicationBar();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            if ((Application.Current as App).gbHaveLoadedSavedDate)
                datePicker.Value = (Application.Current as App).gdtSelectedDate;

            string strDisplayResults = "";
            FormatResultsClass frc = new FormatResultsClass();
            if (true == (Application.Current as App).gbSaveDate)
            {
                chkUseSelectedDate.IsChecked = true;
                
                chkShowOnLiveTile.Visibility = Visibility.Visible;
                if (true == (Application.Current as App).gbShowOnTile)
                {
                    chkShowOnLiveTile.IsChecked = true;
                    frc.FormatDaysSinceResults((Application.Current as App).gdtSelectedDate, DateTime.Now, out strDisplayResults);
                    textBlock1.Text = strDisplayResults;
                    SetDynamicTile();
                    StartPeriodicAgent();  //called to refresh the agent on each startup so it doesn't expire (agents expire every 14 days).
                }
                else
                {
                    frc.FormatDaysSinceResults((Application.Current as App).gdtSelectedDate, DateTime.Now, out strDisplayResults);
                    textBlock1.Text = strDisplayResults;
                    SetApplicationTile("");
                }
            }
            else
            {
                frc.FormatDaysSinceResults(DateTime.Now, DateTime.Now, out strDisplayResults);
                textBlock1.Text = strDisplayResults;
                SetApplicationTile(""); // Set up default flip tile (blank)
            }
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }


        //  Called in Date Diff, when 'FromDatePicker' is selected'
        void FromValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            // do a sanity check to make sure dates are valid here
            DateTime dtFromDate;
            DateTime dtToDate;
            string strDisplayResults = "";

            dtFromDate = (DateTime)FromDatePicker.Value;
            dtToDate = (DateTime)ToDatePicker.Value;

            FormatResultsClass frc = new FormatResultsClass();
            frc.FormatDateDiffResults(dtFromDate, dtToDate, out strDisplayResults);// 3 == DATEDIFF

            textBlock2.Text = strDisplayResults;
        }

        //  Called in Date Diff, when 'ToDatePicker' is selected'
        void ToValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            // do a sanity check to make sure dates are valid here
            DateTime dtFromDate;
            DateTime dtToDate;
            string strDisplayResults = "";
            
            dtFromDate = (DateTime)FromDatePicker.Value;
            dtToDate = (DateTime)ToDatePicker.Value;

            FormatResultsClass frc = new FormatResultsClass();
            frc.FormatDateDiffResults(dtFromDate, dtToDate, out strDisplayResults);// 3 == DATEDIFF
            
            textBlock2.Text = strDisplayResults;
        }


        // Called for 'DaysSince', when DatePicker is selected.
        void SelectedDateValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            if (null == textBlock1)
                return;
            /*
            if (date is changed) && (chkbox is not already selected)
	            do nothing

             * if (date is unchanged and (chkbox is already selected)
	            // do nothing

            if (date is unchanged and (chkbox gets selected)
	            // save date  */

            // get today's date. This is the 'To' date.
            DateTime dtToday = DateTime.Now;

            // get 'from' date
            DateTime dtFromDate = new DateTime();
            dtFromDate = (DateTime)e.NewDateTime;

            string strDisplayResults = "";
            
            FormatResultsClass frc = new FormatResultsClass();
            frc.FormatDaysSinceResults(dtFromDate, dtToday, out strDisplayResults);
            
            textBlock1.Text = strDisplayResults;
            
            (Application.Current as App).gdtSelectedDate = dtFromDate;
            
            // if (date is changed) && (chkbox is already selected)
	        // assume user wants this to be new default selected date
            if ((bool)chkUseSelectedDate.IsChecked)
            {
                SaveSelectedDate();
            }

            // if (date is changed) && (chkbox is already selected)
            // assume user wants this to show on live tile
            if ((bool)chkShowOnLiveTile.IsChecked)
            {
                ShowOnLiveTile();
            }
        } // void SelectedDateValueChanged(object sender, DateTimeValueChangedEventArgs e)


        private void SaveSelectedDate()
        {
            (Application.Current as App).gbSaveDate = true;

            // if there is no 'saved' selected date, 
            //      save 'just' selected date
            if ((false == (Application.Current as App).gbHaveLoadedSavedDate))
            {
                (Application.Current as App).gdtSelectedDate = (DateTime)datePicker.Value;
                (Application.Current as App).SaveSettings();
            }

            // if 'saved' selected date != to 'just' selected date, 
            //save 'just selected date
            if (!(Application.Current as App).gdtSelectedDate.Equals(datePicker.Value))
            {
                (Application.Current as App).SaveSettings();
            }
        }

        
        // Called when "Keep Selected Date as Default" is checked on 'Days Since' page
        // Use this as the default date, to show on main page for 'Days Since' when app is started.
        private void SaveDate_Clicked(object sender, RoutedEventArgs e)
        {
            if ((bool)chkUseSelectedDate.IsChecked)
            {
                (Application.Current as App).gbSaveDate = true;
                chkShowOnLiveTile.Visibility = Visibility.Visible;
                SaveSelectedDate();
            }
            else
            {   // Remove all saved settings
                RemoveAgent( periodicTaskName );
                chkShowOnLiveTile.Visibility = Visibility.Collapsed;
                chkShowOnLiveTile.IsChecked = false;

                (Application.Current as App).gdtSelectedDate = DateTime.Now; // default setting
                (Application.Current as App).gbHaveLoadedSavedDate = false;
                (Application.Current as App).gbShowOnTile = false;
                (Application.Current as App).gbSaveDate = false;
                
                (Application.Current as App).RemoveSettings();
                SetApplicationTile( "" );
            }
        }

        private void ShowOnLiveTile()
        {
            SetDynamicTile();
            StartPeriodicAgent();
        }


        // Shows 'Days Since' selected date on Live Tile
        private void ShowOnLiveTile_Clicked(object sender, RoutedEventArgs e)
        {
            if ((bool)chkShowOnLiveTile.IsChecked)
            {
                if (true == (Application.Current as App).gbSaveDate) // this will always be true if 'chkShowOnLiveTile' is checked.
                {
                    /*
                    if there is a selected date.
                        get selected date
                        calculate Days Since
                        set up schedule to check current date, recalculate DaysSince.
                        set up flip tile to show selected date
                    */
                    (Application.Current as App).gbShowOnTile = true;
                    ShowOnLiveTile();
                }
            }
            else
            {
                (Application.Current as App).gbShowOnTile = false;
                SetApplicationTile( "" ); // Set Live Tile to blank
                RemoveAgent(periodicTaskName); // Turn off ScheduledAgent
            }
        }

        // Set all the properties of the Application Tile.
        void SetApplicationTile( string strBackContent )
        {
            // Application Tile is always the first Tile, even if it is not pinned to Start.
            ShellTile TileToFind = ShellTile.ActiveTiles.First();

            // Application should always be found
            if ( null != TileToFind )
            {
                // set the properties to update for the Application Tile
                // Empty strings for the text values and URIs will result in the property being cleared.

                FlipTileData NewTileData = new FlipTileData()
                {
                   Title = AppResources.AppTitle,
                   BackTitle = AppResources.AppTitle,
                   
                   SmallBackgroundImage = new Uri("/Content/SmallBlankTileImage159x159.png", UriKind.Relative),
                   
                   BackgroundImage = new Uri("/Content/MediumBlankTileImage336x336.png", UriKind.Relative),
                   BackBackgroundImage = new Uri("/Content/MediumBlankBackTileImage336x336.png", UriKind.Relative),

                   WideBackgroundImage = new Uri("/Content/WideBackBackgroundTileIcon691x336WithGlow.png", UriKind.Relative),
                   WideBackBackgroundImage = new Uri("/Content/WideBlankBackTileIcon691x336.png", UriKind.Relative),
                };

                // Update the Application Tile
                TileToFind.Update(NewTileData);
            }
        } // SetApplicationTile()

        /*********************************
        void SetDynamicTile()
        {
            //
            //1. Create a WriteableBitmap 
            //2. Grab images from our app Content (optional) 
            //3. Create and format some UI Elements to add to our Bitmap 
            //4. Build our Bitmap how we like it 
            //5. Save our Bitmap to our Phone 
            //6. display our Bitmap on the front or back of the Tile. 
            //
            //I create a WriteableBitmap of the correct dimensions and a UI Element to host the Bitmap …
            try
            {
                var bmp = new WriteableBitmap(c_n336, c_n336);

                var logo = new BitmapImage(new Uri("/Content/MediumBlankBackTileImage336x336.png", UriKind.Relative));

                logo.ImageOpened += OnImageOpened;

                var img = new Image { Source = logo };
                // Force the bitmapimage to load it's properties so the transform will work
                logo.CreateOptions = BitmapCreateOptions.None;
            }
            catch (Exception e)
            {
                MessageBox.Show( e.ToString() );
            }
        } // void SetDynamicTile(string strBackContent)
******************************************************************************/

        void SetDynamicTile()
        {
            UpdateLiveTile();
        }
        
        // Called when BitMapImage loads the image
        void OnImageOpened(object sender, RoutedEventArgs e)
        {
            String strBackContent = "";

            // Only called if they've selected to Show On Live Tile
            if ((Application.Current as App).gbShowOnTile)
            {
                FormatResultsClass frc = new FormatResultsClass();
                frc.FormatLiveTileText((Application.Current as App).gdtSelectedDate, DateTime.Now, out strBackContent ); 
            }

            BitmapImage bm = (BitmapImage)sender;

            var img = new Image { Source = bm };

            WriteableBitmap wbm = new WriteableBitmap( bm );
            var tt = new TranslateTransform();
            tt.X = c_n336;
            tt.Y = c_n336;

            /*
            Next, we create the UI Elements that we want to push into our WriteableBitmap using the Render method.  
            We will need to format our Tile to our liking.  
            We will also need to add our image to our bitmap and position it properly.  
            In order to do that, we will need to use Transforms.  
            I’m lazy and uncreative, so I will use a TranslateTransform, but the idea is the same even if you use a MatrixTransform.             
            */
            var tbText = new TextBlock();
            tbText.Height = c_n336;
            tbText.Width = c_n336;
            tbText.Foreground = new SolidColorBrush(Colors.White);
            tbText.FontFamily = new FontFamily("Arial Narrow");
            tbText.FontSize = 30;
            tbText.TextWrapping = TextWrapping.Wrap;
            tbText.Text = strBackContent;
            tbText.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            tbText.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            
            
            wbm.Render(img, tt);
            wbm.Render(tbText, null);
            wbm.Invalidate();

            /*
            Next is a little bit of complexity with IsolatedStorage.  
            In addition to providing local storage for our application, on the Phone, 
            IsolatedStorage has some shared areas.  
            Specifically, the Shared/ShellContent section.  
            We need to store our Bitmap in this area in order to be used by a Tile.             * 
            */
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
                /*StandardTileData NewTileData = new StandardTileData
                {
                    BackBackgroundImage = new Uri("isostore:" + filename, UriKind.Absolute)
                };*/

                FlipTileData NewTileData = new FlipTileData
                {
                    BackBackgroundImage = new Uri("isostore:" + filename, UriKind.Absolute),
                };


                // Update the Application Tile
                TileToFind.Update(NewTileData);
            }
        } // void OnImageOpened(object sender, RoutedEventArgs e)


        // Only called if they've selected to Show On Live Tile
        // Updates the text on the live tile
        public static void UpdateLiveTile()
        {
            ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault();
            if (tile != null)
            {
                FormatResultsClass frc = new FormatResultsClass();

                FlipTileData flipTile = new FlipTileData();
                flipTile.Title = AppResources.AppTitle;
                flipTile.BackTitle = AppResources.AppTitle;

                flipTile.BackContent = " ";
                flipTile.WideBackContent = " ";

                String strBackContent = "";
                frc.FormatLiveTileText((Application.Current as App).gdtSelectedDate, DateTime.Now, out strBackContent );

                //Medium size Tile 336x336 px
                //Create image for BackBackgroundImage in IsoStore
                frc.RenderText(strBackContent, c_n336, c_n336, 28, "BackBackgroundImage");

                flipTile.BackBackgroundImage = new Uri(@"isostore:/Shared/ShellContent/BackBackgroundImage.jpg", UriKind.Absolute); //Generated image for Back Background 336x336
                flipTile.BackgroundImage = new Uri("/Content/MediumBlankTileImage336x336.png", UriKind.Relative); //Default image for Background Image Medium Tile 336x336 px
                //End Medium size Tile 336x336 px

                //Wide size Tile 691x336 px
                //flipTile.WideBackgroundImage = new Uri("/Content/WideBlankTileIcon691x336.png", UriKind.Relative); // Default image for Background Image Wide Tile 691x336 px
                flipTile.WideBackgroundImage = new Uri("/Content/WideBackBackgroundTileIcon691x336WithGlow.png", UriKind.Relative); // Default image for Background Image Wide Tile 691x336 px

                //Create image for WideBackBackgroundImage in IsoStore
                frc.RenderText(strBackContent, 691, c_n336, 40, "WideBackBackgroundImage");
                flipTile.WideBackBackgroundImage = new Uri(@"isostore:/Shared/ShellContent/WideBackBackgroundImage.jpg", UriKind.Absolute);
                
                //End Wide size Tile 691x336 px

                //Update Live Tile
                tile.Update(flipTile);
            }
        } // public static void UpdateLiveTile(string info)
        
        
        /*************
        private static void RenderText(string strBackContent, int width, int height, int fontsize, string imagename)
        {
            WriteableBitmap wbmBitmap = new WriteableBitmap(width, height);
        //**********************************
         //   BitmapImage logo;
         //   if (336 == width)
         //       logo = new BitmapImage(new Uri("/Content/MediumBlankBackTileImage336x336.png", UriKind.Relative));
         //    else
         //       logo = new BitmapImage(new Uri("/Content/WideBlankBackTileIcon691x336.png", UriKind.Relative));
//
  //          var img = new Image { Source = logo };
//
  //          var tt = new TranslateTransform();
    //        tt.X = width;
      //      tt.Y = height;
        //    ***********************************
            // old code
            //var bmp = writeableBitmap( 336,336)
            //var logo = new BitmapImage(new Uri("/Content/MediumBlankBackTileImage336x336.png", UriKind.Relative));
            //var img = new Image( logo )

            //wbm = WriteableBitmap( logo) )
            //wbm.Render( img, tt)


            var grdGrid = new Grid();
            grdGrid.Width = wbmBitmap.PixelWidth;
            grdGrid.Height = wbmBitmap.PixelHeight;

            var cnvBackground = new Canvas();
            cnvBackground.Height = wbmBitmap.PixelHeight;
            cnvBackground.Width = wbmBitmap.PixelWidth;

            Color color = new Color();
            // hex
            //color.R = 14;
            //color.G = 16;
            //color.B = 3D;

            // decimal
            color.A = 255;
            color.R = 20;
            color.G = 101;
            color.B = 61;


            //SolidColorBrush backColor = new SolidColorBrush( (Color)Application.Current.Resources["PhoneAccentColor"]);
            SolidColorBrush backColor = new SolidColorBrush( color );
            cnvBackground.Background = backColor;


            var tbText = new TextBlock();
            tbText.Height = height;
            tbText.Width = width;
            tbText.Foreground = new SolidColorBrush(Colors.White);
            tbText.FontFamily = new FontFamily("Arial Narrow");
            tbText.FontSize = 30;
            tbText.TextWrapping = TextWrapping.Wrap;
            tbText.Text = strBackContent;
            tbText.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            tbText.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            grdGrid.Children.Add(tbText);

            wbmBitmap.Render(cnvBackground, null);
            //wbmBitmap.Render( img, tt);
            wbmBitmap.Render(grdGrid, null);
            wbmBitmap.Invalidate(); //Draw bitmap

            //Save bitmap as jpeg file in Isolated Storage
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream imageStream = new IsolatedStorageFileStream("/Shared/ShellContent/" + imagename + ".jpg", System.IO.FileMode.Create, isf))
                {
                    wbmBitmap.SaveJpeg(imageStream, wbmBitmap.PixelWidth, wbmBitmap.PixelHeight, 0, 100);
                }
            }
        }  // private static void RenderText(string text, int width, int height, int fontsize, string imagename)
        */

        private void StartPeriodicAgent()
        {
            // Variable for tracking enabled status of background agents for this app.
            agentsAreEnabled = true;

            // Obtain a reference to the period task, if one exists
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

            // If the task already exists and background agents are enabled for the
            // application, you must remove the task and then add it again to update 
            // the schedule
            if (periodicTask != null)
            {
                RemoveAgent(periodicTaskName);
            }

            periodicTask = new PeriodicTask(periodicTaskName);

            // The description is required for periodic agents. This is the string that the user
            // will see in the background services Settings page on the device.
            periodicTask.Description = AppResources.txtPeriodicTaskDesc; //periodicTask.Description = "Days Since Tile Update Task";

            // Place the call to Add in a try block in case the user has disabled agents.
            try
            {
                ScheduledActionService.Add(periodicTask);
                // If debugging is enabled, use LaunchForTest to launch the agent in one minute.
            #if(DEBUG_AGENT)
                ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromSeconds(60));
            #endif
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains(AppResources.txtBNSError))  //"BNS Error: The action is disabled"
                {
                    MessageBox.Show(AppResources.txtBackAgentsDisabled); // "Background agents for this application have been disabled by the user."
                    agentsAreEnabled = false;
                }

                if (exception.Message.Contains(AppResources.txtMaxScheduledActions)) //"BNS Error: The maximum number of ScheduledActions of this type have already been added."
                {
                    // No user action required. The system prompts the user when the hard limit of periodic tasks has been reached.

                }
                //PeriodicCheckBox.IsChecked = false;
            }
            catch (SchedulerServiceException exception)
            {
                // No user action required.
#if (DEBUG_AGENT)
                MessageBox.Show( exception.Message );
#endif
            }
        } // StartPeriodicAgent()


        
        private void RemoveAgent(string name)
        {
            try
            {
                ScheduledActionService.Remove(name);
            }
            catch (Exception)
            {
            }
        }// RemoveAgent()


        //code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("Content/appbar.questionmark.rest.png", UriKind.Relative));
            appBarButton.Text = AppResources.txtAbout;
            ApplicationBar.Buttons.Add(appBarButton);
            appBarButton.Click += new EventHandler(ApplicationBarIconButton_Click);

            // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.txtAbout);
            ApplicationBar.MenuItems.Add(appBarMenuItem);
            appBarMenuItem.Click += new EventHandler(ApplicationBarIconButton_Click);
        }
        

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate( new Uri("/AboutPage.xaml", UriKind.Relative) );
            //args.Complete();
            //args.Handled = true
        }  // ApplicationBarIconButton_Click()

    } // public partial class MainPage : PhoneApplicationPage


    public class clsLocalizedStrings
    {
        public clsLocalizedStrings()
        {
        }

        private static AppResources localizedResources = new AppResources();

        public AppResources propLocalizedResources
        {
            get { return localizedResources; }
        }
    }



} // namespace DaysSince