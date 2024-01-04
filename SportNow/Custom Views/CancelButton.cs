using System;
using Xamarin.Forms;

namespace SportNow.CustomViews
{
    public class CancelButton : Frame
    {

        /*public double width { get; set; }
        public string text { get; set; }*/

        //public Frame frame;
        public Button button;

        public CancelButton(string text, double width, double height)
        {

            createCancelButton(text, width, height, 1);
        }

        public CancelButton(string text, double width, double height, double screenAdaptor)
        {
            createCancelButton(text, width, height, screenAdaptor);
        }

        public void createCancelButton(string text, double width, double height, double screenAdaptor)
        {
            //BUTTON
            button = new Button
            {
                Text = text,
                BackgroundColor = Color.FromRgb(233, 93, 85), //gradient,
                TextColor = Color.White,
                FontSize = App.itemTitleFontSize, //* App.screenWidthAdapter,
                WidthRequest = width,
                HeightRequest = height
            };
            //geralButton.Clicked += OnGeralButtonClicked;

            //frame = new Frame { BackgroundColor = Color.FromRgb(25, 25, 25), BorderColor = Color.LightGray, CornerRadius = 20, IsClippedToBounds = true, Padding = 0 };
            this.BackgroundColor = Color.FromRgb(233, 93, 85);
            //this.BorderColor = Color.LightGray;
            this.CornerRadius = (float)(10 * screenAdaptor);
            this.IsClippedToBounds = true;
            this.Padding = 0;
            this.WidthRequest = width;
            this.HeightRequest = height;
            this.Content = button; // relativeLayout_Button;
        }
    }
}
