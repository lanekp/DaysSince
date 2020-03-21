using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks; // MarketplaceReviewTask, EmailComposeTask

namespace DaysSince
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void btnReview_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask(); 
            marketplaceReviewTask.Show();
        } // btnReview_Click


        private void btnEmail_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = "lanekpa@hotmail.com";
            //emailComposeTask.Subject = "Feedback on DaysSince app v1.1.0.0";
            emailComposeTask.Subject = DaysSince.Resources.AppResources.txtEmailFeedback;
            emailComposeTask.Body = "";
            emailComposeTask.Show();
        } // SendEmail
    }
}