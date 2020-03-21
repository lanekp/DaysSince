using System;
using System.Net;
using System.Text;                          // StringBuilder
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Shell;                // ShellTile
using System.IO.IsolatedStorage;            // for isolated storage
using System.Windows.Media.Imaging;         // WriteableBitmap

using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;                  // Binding class
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;            // ScheduledAgent


namespace DaysSinceClassLibrary
{
    public class FormatResultsClass
    {
        public FormatResultsClass()
        {
        }

        // Called on for DaysSince
        public void FormatDaysSinceResults(DateTime dtFromDate, DateTime dtToDate, out string strResultsText)
        {
            // Clear Text Block
            strResultsText = "";

            CalculateDateDifference dateDiff = new CalculateDateDifference(dtFromDate, dtToDate);

            // Total days
            TimeSpan tsDaysSince = dtToDate - dtFromDate;
            int nTotalDays = (int)tsDaysSince.TotalDays;

            int nTotalMonths = dateDiff.Months;
            int nDays = dateDiff.Days;
            int nTotalYears = dateDiff.Years;

            // truth table for formatting the string output
            string strDay;
            string strMonth;
            string strYear;
            string sRN = " \r\n";

            StringBuilder sbDays = new StringBuilder( DaysSinceClassLibAppResources.txtDays );
            StringBuilder sbBeginning = new StringBuilder("\r\n\r\n");
            StringBuilder sbResultsText = new StringBuilder("");

            // Days Since page output
            /*
            Number of Days Since <date> is:
            xxxxx days
            xxx years, xx months, xxx days
            */

            if (1 == nTotalMonths)
                strMonth = DaysSinceClassLibAppResources.txtMonthComma; //strMonth = " Month, ";
            else
                strMonth = DaysSinceClassLibAppResources.txtMonthsComma; //strMonth = " Months, ";

            if (1 == nDays)
                strDay = DaysSinceClassLibAppResources.txtDay; //strDay = " Day";
            else
                strDay = DaysSinceClassLibAppResources.txtDays; //strDay = " Days";

            if (1 == nTotalYears)
                strYear = DaysSinceClassLibAppResources.txtYearComma; //strYear = " Year, ";
            else
                strYear = DaysSinceClassLibAppResources.txtYearsComma; //strYear = " Years, ";

            string strFromDate = dtFromDate.ToShortDateString();
            string strToDate = dtToDate.ToShortDateString();
            if ( strFromDate == strToDate ) // Dates are equal.
            {
                //strResultsText = sbBeginning + nDays.ToString() + strDay.ToString(); ;
                sbResultsText.Append( sbBeginning + nDays.ToString() + strDay.ToString() );
                strResultsText += sbResultsText.ToString();
                return;
            }

            if ((0 == nTotalYears) && (0 == nTotalMonths) && (nDays > 0))
                // 0 years, 0 months, x days
                //strResultsText = sbBeginning + nDays.ToString() + strDay.ToString(); ;
                sbResultsText.Append( sbBeginning.ToString() + nDays.ToString() + strDay.ToString() );

            // 0 years, x months, 0 days
            if ((0 == nTotalYears) && (nTotalMonths > 0) && (0 == nDays))
            {
                //strResultsText = sbBeginning + nTotalDays.ToString() + " Days \r\n" + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString();
                sbResultsText.Append( sbBeginning + nTotalDays.ToString() + sbDays.ToString() + sRN + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString() );
            }

            // 0 years, x months, x days
            if ((0 == nTotalYears) && (nTotalMonths > 0) && (nDays > 0))
            {
                //strResultsText = sbBeginning + nTotalDays.ToString() + " Days \r\n" + nTotalMonths + strMonth + nDays + strDay.ToString();
                sbResultsText.Append(sbBeginning + nTotalDays.ToString() + sbDays.ToString() + sRN + nTotalMonths + strMonth + nDays + strDay.ToString());
            }

            // x years, 0 months, 0 days
            if ((nTotalYears > 0) && (0 == nTotalMonths) && (0 == nDays))
            {
                //strResultsText = sbBeginning + nTotalDays.ToString() + " Days \r\n" + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString();
                sbResultsText.Append(sbBeginning + nTotalDays.ToString() + sbDays.ToString() + sRN + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString());
            }

            if ((nTotalYears > 0) && (0 == nTotalMonths) && (nDays > 0))
                // x years, 0 months, x days
                //strResultsText = sbBeginning + nTotalDays.ToString() + " Days \r\n" + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString();
                sbResultsText.Append(sbBeginning + nTotalDays.ToString() + sbDays.ToString() + sRN + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString());

            if ((nTotalYears > 0) && (nTotalMonths > 0) && (0 == nDays))
                // x years, x months, 0 days
                //strResultsText = sbBeginning + nTotalDays.ToString() + " Days \r\n" + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString();
                sbResultsText.Append(sbBeginning + nTotalDays.ToString() + sbDays.ToString() + sRN + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString());

            if ((nTotalYears > 0) && (nTotalMonths > 0) && (nDays > 0))
                // x years, x months, x days
                //strResultsText = sbBeginning + nTotalDays.ToString() + " Days \r\n" + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString();
                sbResultsText.Append(sbBeginning + nTotalDays.ToString() + sbDays.ToString() + sRN + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString());


            strResultsText += sbResultsText.ToString();
        } //FormatDaysSinceResults()

        
        
        // Called on for DateDiff
        public void FormatDateDiffResults(DateTime dtFromDate, DateTime dtToDate, out string strResultsText)
        {
            // Clear Text Block
            strResultsText = "";

            CalculateDateDifference dateDiff = new CalculateDateDifference(dtFromDate, dtToDate);

            // Total days
            TimeSpan tsDaysSince = dtToDate - dtFromDate;
            int nTotalDays = (int)tsDaysSince.TotalDays;

            int nTotalMonths = dateDiff.Months;
            int nDays = dateDiff.Days;
            int nTotalYears = dateDiff.Years;

            // truth table for formatting the string output
            string strDay;
            string strMonth;
            string strYear;
            string sRN = " \r\n";

            StringBuilder sbDays = new StringBuilder(DaysSinceClassLibAppResources.txtDays);
            StringBuilder sbBeginning = new StringBuilder("\r\n\r\n");
            StringBuilder sbResultsText = new StringBuilder("");

            // Days Since page output
            /*
            Number of Days Since <date> is:
            xxxxx days
            xxx years, xx months, xxx days
            */

            if (1 == nTotalMonths)
                //strMonth = " Month, ";
                strMonth = DaysSinceClassLibAppResources.txtMonthComma;
            else
                //strMonth = " Months, ";
                strMonth = DaysSinceClassLibAppResources.txtMonthsComma;

            if (1 == nDays)
                //strDay = " Day.";
                strDay = DaysSinceClassLibAppResources.txtDayPeriod;
            else
                //strDay = " Days";
                strDay = DaysSinceClassLibAppResources.txtDays;

            if (1 == nTotalYears)
                //strYear = " Year, ";
                strYear = DaysSinceClassLibAppResources.txtYearComma;
            else
                //strYear = " Years, ";
                strYear = DaysSinceClassLibAppResources.txtYearsComma;

            // clear text block

            if ((0 == nTotalYears) && (0 == nTotalMonths) && (nDays > 0))
            {
                // 0 years, 0 months, x days
                //strResultsText = sbBeginning + nDays.ToString() + strDay.ToString();
                sbResultsText.Append(sbBeginning + nDays.ToString() + strDay.ToString());
            }

            // 0 years, x months, 0 days
            if ((0 == nTotalYears) && (nTotalMonths > 0) && (0 == nDays))
            {
                //strResultsText = sbBeginning + nTotalDays.ToString() + sbDays.ToString() + sRN + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString();
                sbResultsText.Append( sbBeginning + nTotalDays.ToString() + sbDays.ToString() + sRN + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString() );
            }

            // 0 years, x months, x days
            if ((0 == nTotalYears) && (nTotalMonths > 0) && (nDays > 0))
            {
                //strResultsText = sbBeginning + nTotalDays.ToString() + sbDays.ToString() + sRN + nTotalMonths + strMonth + nDays + strDay.ToString();
                sbResultsText.Append( sbBeginning + nTotalDays.ToString() + sbDays.ToString() + sRN + nTotalMonths + strMonth + nDays + strDay.ToString() );
            }

            // x years, 0 months, 0 days
            if ((nTotalYears > 0) && (0 == nTotalMonths) && (0 == nDays))
            {
                //strResultsText = sbBeginning + nTotalDays.ToString() + sbDays.ToString() + sRN + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString();
                sbResultsText.Append( sbBeginning + nTotalDays.ToString() + sbDays.ToString() + sRN + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString() );
            }

            if ((nTotalYears > 0) && (0 == nTotalMonths) && (nDays > 0))
                // x years, 0 months, x days
                //strResultsText = sbBeginning + nTotalDays.ToString() + sbDays.ToString() + sRN + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString();
                sbResultsText.Append( sbBeginning + nTotalDays.ToString() + sbDays.ToString() + sRN + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString() );

            if ((nTotalYears > 0) && (nTotalMonths > 0) && (0 == nDays))
                // x years, x months, 0 days
                //strResultsText = sbBeginning + nTotalDays.ToString() + sbDays.ToString() + sRN + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString();
                sbResultsText.Append( sbBeginning + nTotalDays.ToString() + sbDays.ToString() + sRN + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString() );

            if ((nTotalYears > 0) && (nTotalMonths > 0) && (nDays > 0))
                // x years, x months, x days
                //strResultsText = sbBeginning + nTotalDays.ToString() + sbDays.ToString() + sRN + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString();
                sbResultsText.Append( sbBeginning + nTotalDays.ToString() + sbDays.ToString() + sRN + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString() );

            strResultsText = sbResultsText.ToString();
        } //FormatDateDiffResults()
        

    
        // Format the text to display on live tile
        public void FormatLiveTileText(DateTime dtFromDate, DateTime dtToDate, out string strResultsText )
        {
            CalculateDateDifference dateDiff = new CalculateDateDifference(dtFromDate, dtToDate);

            // Total days
            TimeSpan tsDaysSince = dtToDate - dtFromDate;
            int nTotalDays = (int)tsDaysSince.TotalDays;

            int nTotalMonths = dateDiff.Months;
            int nDays = dateDiff.Days;
            int nTotalYears = dateDiff.Years;

            // truth table for formatting the string output
            string strDay;
            string strMonth;
            string strYear;
            StringBuilder sbBeginning = new StringBuilder("");
            StringBuilder sbResultsText = new StringBuilder("");

            /*
            Backtile output
            
             <date>
            xxxxx days
            xxx years, xx months, xxx days

            // Days Since page output
            Number of Days Since <date> is:
            xxxxx days
            xxx years, xx months, xxx days
            */

            sbBeginning.Append("      " + dtFromDate.ToShortDateString() + "\r\n\r\n");
            if (1 == nTotalMonths)
                strMonth = DaysSinceClassLibAppResources.txtSpaceMonthCommaSpace; //" month, ";
            else
                strMonth = DaysSinceClassLibAppResources.txtSpaceMonthsCommaSpace; // " months, ";

            if (1 == nDays)
                strDay = DaysSinceClassLibAppResources.txtSpaceDay; // " day";
            else
                strDay = DaysSinceClassLibAppResources.txtSpaceDays; // " days";

            if (1 == nTotalYears)
                strYear = DaysSinceClassLibAppResources.txtSpaceYearCommaSpace; // " year, ";
            else
                strYear = DaysSinceClassLibAppResources.txtYearsCommaSpace; // " years, ";

            //strResultsText = "";
            string strFromDate = dtFromDate.ToShortDateString();
            string strToDate = dtToDate.ToShortDateString();
            if (strFromDate == strToDate) // Dates are equal.
            {
                strResultsText = sbBeginning + nDays.ToString() + strDay.ToString(); ;
                return;
            }
            
            if ((0 == nTotalYears) && (0 == nTotalMonths) && (nDays > 0))
                // 0 years, 0 months, x days
                //strResultsText = sbBeginning + nDays.ToString() + strDay.ToString(); ;
                sbResultsText.Append( sbBeginning + nDays.ToString() + strDay.ToString() );

            // 0 years, x months, 0 days
            if ((0 == nTotalYears) && (nTotalMonths > 0) && (0 == nDays))
            {
                //strResultsText = sbBeginning + nTotalDays.ToString() + DaysSinceClassLibAppResources.txtSpaceDaysSpace + "\r\n" + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString();
                sbResultsText.Append(sbBeginning + nTotalDays.ToString() + DaysSinceClassLibAppResources.txtSpaceDaysSpace + "\r\n" + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString());
            }

            // 0 years, x months, x days
            if ((0 == nTotalYears) && (nTotalMonths > 0) && (nDays > 0))
            {
                //strResultsText = sbBeginning + nTotalDays.ToString() + DaysSinceClassLibAppResources.txtSpaceDaysSpace + "\r\n" + nTotalMonths + strMonth + nDays + strDay.ToString();
                sbResultsText.Append(sbBeginning + nTotalDays.ToString() + DaysSinceClassLibAppResources.txtSpaceDaysSpace + "\r\n" + nTotalMonths + strMonth + nDays + strDay.ToString());
            }

            // x years, 0 months, 0 days
            if ((nTotalYears > 0) && (0 == nTotalMonths) && (0 == nDays))
            {
                //strResultsText = sbBeginning + nTotalDays.ToString() + DaysSinceClassLibAppResources.txtSpaceDaysSpace + "\r\n" + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString();
                sbResultsText.Append(sbBeginning + nTotalDays.ToString() + DaysSinceClassLibAppResources.txtSpaceDaysSpace + "\r\n" + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString());
            }

            if ((nTotalYears > 0) && (0 == nTotalMonths) && (nDays > 0))
                // x years, 0 months, x days
                //strResultsText = sbBeginning + nTotalDays.ToString() + DaysSinceClassLibAppResources.txtSpaceDaysSpace + "\r\n" + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString();
                sbResultsText.Append( sbBeginning + nTotalDays.ToString() + DaysSinceClassLibAppResources.txtSpaceDaysSpace + "\r\n" + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString() );

            if ((nTotalYears > 0) && (nTotalMonths > 0) && (0 == nDays))
                // x years, x months, 0 days
                //strResultsText = sbBeginning + nTotalDays.ToString() + DaysSinceClassLibAppResources.txtSpaceDaysSpace + "\r\n" + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString();
                sbResultsText.Append( sbBeginning + nTotalDays.ToString() + DaysSinceClassLibAppResources.txtSpaceDaysSpace + "\r\n" + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString() );

            if ((nTotalYears > 0) && (nTotalMonths > 0) && (nDays > 0))
                // x years, x months, x days
                //strResultsText = sbBeginning + nTotalDays.ToString() + DaysSinceClassLibAppResources.txtSpaceDaysSpace + "\r\n" + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString();
                //sbResultsText.Append( sbBeginning + nTotalDays.ToString() + DaysSinceClassLibAppResources.txtSpaceDaysSpace + "\r\n" + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString() );
                sbResultsText.Append(sbBeginning + nTotalDays.ToString() + DaysSinceClassLibAppResources.txtSpaceDaysSpace + "\r\n" + nTotalYears + strYear.ToString() + nTotalMonths + strMonth.ToString() + nDays + strDay.ToString());
            
            strResultsText = sbResultsText.ToString();// need to return as a string

        } // void FormatLiveTileText

        
        public void RenderText(string strBackContent, int width, int height, int fontsize, string imagename)
        {
            const int cnWideTileWidth = 691; // the width of a wide tile.

            WriteableBitmap wbmBitmap = new WriteableBitmap(width, height);
            /**********************************
                BitmapImage logo;
                    if (336 == width)
                    logo = new BitmapImage(new Uri("/Content/MediumBlankBackTileImage336x336.png", UriKind.Relative));
                    else
                    logo = new BitmapImage(new Uri("/Content/WideBlankBackTileIcon691x336.png", UriKind.Relative));

                var img = new Image { Source = logo };

                var tt = new TranslateTransform();
                tt.X = width;
                tt.Y = height;
                ***********************************/
            /* old code
            var bmp = writeableBitmap( 336,336)
            var logo = new BitmapImage(new Uri("/Content/MediumBlankBackTileImage336x336.png", UriKind.Relative));
            var img = new Image( logo )

            wbm = WriteableBitmap( logo) )
            wbm.Render( img, tt)
                */

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

            SolidColorBrush backColor = new SolidColorBrush(color);
            cnvBackground.Background = backColor;

            var tbText = new TextBlock();
            tbText.Height = height;
            tbText.Width = width;
            tbText.Foreground = new SolidColorBrush(Colors.White);
            tbText.FontFamily = new FontFamily("Arial Narrow");
            
            if (cnWideTileWidth == width) // its a wide tile, so use a larger font
                tbText.FontSize = 50;
            else
                tbText.FontSize = 40; // medium tile font size

            tbText.TextWrapping = TextWrapping.Wrap;
            tbText.Text = strBackContent;
            tbText.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            tbText.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            grdGrid.Children.Add(tbText);

            wbmBitmap.Render(cnvBackground, null);
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

    
    
    } //public class FormatResultsClass

    //Class that will calculate the number of days, months and years between two given dates.
    public class CalculateDateDifference
    {
        /// <summary>
        /// defining Number of days in month; index 0 represents january and 11 represents December
        /// february contain either 28 or 29 days, so here its value is -1
        /// which will be calculate later.
        /// </summary>

        private int[] monthDay = new int[12] { 31, -1, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        /// <summary>
        /// contains 'from' date
        /// </summary>
        private DateTime fromDate;

        /// <summary>
        /// contains 'To' Date
        /// </summary>
        private DateTime toDate;

        /// <summary>
        /// these three variable of integer type for output representation..
        /// </summary>
        private int year;
        private int month;
        private int day;


        //Public type Constructor
        public CalculateDateDifference(DateTime d1, DateTime d2)
        {
            int increment;

            //To Date must be greater
            if (d1 > d2)
            {
                this.fromDate = d2;
                this.toDate = d1;
            }
            else
            {
                this.fromDate = d1;
                this.toDate = d2;
            }

            ///
            /// Day Calculation
            ///
            increment = 0;

            if (this.fromDate.Day > this.toDate.Day)
            {
                increment = this.monthDay[this.fromDate.Month - 1];
            }

            /// if it is february month
            /// if it's to day is less then from day
            if (increment == -1)
            {
                if (DateTime.IsLeapYear(this.fromDate.Year))
                {
                    // leap year february contain 29 days
                    increment = 29;
                }
                else
                {
                    increment = 28;
                }
            }

            if (increment != 0)
            {
                day = (this.toDate.Day + increment) - this.fromDate.Day;
                increment = 1;
            }
            else
            {
                day = this.toDate.Day - this.fromDate.Day;
            }

            ///
            ///month calculation
            ///
            if ((this.fromDate.Month + increment) > this.toDate.Month)
            {
                this.month = (this.toDate.Month + 12) - (this.fromDate.Month + increment);
                increment = 1;
            }
            else
            {
                this.month = (this.toDate.Month) - (this.fromDate.Month + increment);
                increment = 0;
            }

            ///
            /// year calculation
            ///
            this.year = this.toDate.Year - (this.fromDate.Year + increment);
        } // public CalculateDateDifference(DateTime d1, DateTime d2)

        public override string ToString()
        {
            //return base.ToString();
            return this.year + " Year(s), " + this.month + " month(s), " + this.day + " day(s)";
        }

        public int Years
        {
            get
            {
                return this.year;
            }
        }

        public int Months
        {
            get
            {
                return this.month;
            }
        }

        public int Days
        {
            get
            {
                return this.day;
            }
        }
    }  // public class CalculateDateDifference

} // namespace DaysSinceClassLibrary
