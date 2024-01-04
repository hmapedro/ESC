using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace SportNow.CustomViews
{
    public class RoundImage : Image
    {

        public void createRoundImage(int size)
        {

            this.BackgroundColor = Color.Transparent;
            this.WidthRequest = (int)size * App.screenHeightAdapter;
            this.HeightRequest = (int)size * App.screenHeightAdapter;
            this.HorizontalOptions = LayoutOptions.Center;
            this.VerticalOptions = LayoutOptions.Center;
            this.Aspect = Aspect.AspectFill;
            this.Source = "iconadicionarfoto.png";

            this.Clip = new EllipseGeometry()
            {
                Center = new Point((size / 2) * App.screenHeightAdapter, size / 2 * App.screenHeightAdapter),
                RadiusX = (size / 2) * App.screenHeightAdapter,
                RadiusY = (size / 2) * App.screenHeightAdapter
            };
        }

        public RoundImage(int size)
        {
            createRoundImage(size);
        }

        public RoundImage()
        {
            createRoundImage(180);
        }
    }
}   
