using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DaysSinceClassLibrary
{
    public class DaysSinceClassLib
    {


        // Called by both 'DaysSince()' and 'DateDiff()' to format text output to UI.
        void FormatResultsText(DateTime dtFromDate, DateTime dtToDate, out string strResultsText, eFormatType eFT)
        {
            CalculateDateDifference dateDiff = new CalculateDateDifference(dtFromDate, dtToDate);

            // Total days
            TimeSpan tsDaysSince = dtToDate - dtFromDate;
            int nTotalDays = (int)tsDaysSince.TotalDays;

            int totalMonths = dateDiff.Months;
            int nDays = dateDiff.Days;
            int totalYears = dateDiff.Years;

            // truth table for formatting the string output
            string sDay;
            string sMonth;
            string sYear;
            string sBeginning = "";

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
            if (eFormatType.DATEDIFF == eFT)
                sBeginning = "Number of days between\r\n" + dtFromDate.ToShortDateString() + " and " + dtToDate.ToShortDateString() + " is:  ";

            if (eFormatType.MAINPAGE == eFT)
                sBeginning = "Number of days since " + dtFromDate.ToShortDateString() + " is: ";

            if (eFormatType.BACKTILE == eFT)
                sBeginning = dtFromDate.ToShortDateString() + "\r\n";

            if (1 == totalMonths)
                sMonth = " month, ";
            else
                sMonth = " months, ";

            if (1 == nDays)
                sDay = " day.";
            else
                sDay = " days.";

            if (1 == totalYears)
                sYear = " year, ";
            else
                sYear = " years, ";

            // clear text block
            strResultsText = "";
            if ((0 == totalYears) && (0 == totalMonths) && (nDays > 0))
                // 0 years, 0 months, x days
                strResultsText = sBeginning + nDays.ToString() + sDay.ToString(); ;

            // 0 years, x months, 0 days
            if ((0 == totalYears) && (totalMonths > 0) && (0 == nDays))
            {
                strResultsText = sBeginning + nTotalDays.ToString() + " days or,\r\n" + totalMonths + sMonth.ToString() + nDays + sDay.ToString();
            }

            // 0 years, x months, x days
            if ((0 == totalYears) && (totalMonths > 0) && (nDays > 0))
            {
                strResultsText = sBeginning + nTotalDays.ToString() + " days or,\r\n" + totalMonths + sMonth + nDays + sDay.ToString();
            }

            // x years, 0 months, 0 days
            if ((totalYears > 0) && (0 == totalMonths) && (0 == nDays))
            {
                strResultsText = sBeginning + nTotalDays.ToString() + " days or,\r\n" + totalYears + sYear.ToString() + totalMonths + sMonth.ToString() + nDays + sDay.ToString();
            }

            if ((totalYears > 0) && (0 == totalMonths) && (nDays > 0))
                // x years, 0 months, x days
                strResultsText = sBeginning + nTotalDays.ToString() + " days or,\r\n" + totalYears + sYear.ToString() + totalMonths + sMonth.ToString() + nDays + sDay.ToString();

            if ((totalYears > 0) && (totalMonths > 0) && (0 == nDays))
                // x years, x months, 0 days
                strResultsText = sBeginning + nTotalDays.ToString() + " days or,\r\n" + totalYears + sYear.ToString() + totalMonths + sMonth.ToString() + nDays + sDay.ToString();

            if ((totalYears > 0) && (totalMonths > 0) && (nDays > 0))
                // x years, x months, x days
                strResultsText = sBeginning + nTotalDays.ToString() + " days or,\r\n" + totalYears + sYear.ToString() + totalMonths + sMonth.ToString() + nDays + sDay.ToString();
        } // void FormatResultsText
    }
}
