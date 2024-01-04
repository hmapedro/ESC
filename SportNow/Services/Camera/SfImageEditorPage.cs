using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.SfImageEditor.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SportNow.Services.Camera
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SfImageEditorPage : ContentPage
    {
        SfImageEditor editor;
        public SfImageEditorPage()
        {
            //InitializeComponent();
            editor = new SfImageEditor();
        }

        public SfImageEditorPage(ImageSource source)
        {
            editor.Source = source;
        }

        private ImageSource _imageSource;
        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set
            {
                _imageSource = value;
                OnPropertyChanged();
                OpenImageEditor(_imageSource);
            }
        }

        void OpenImageEditor(ImageSource imageSource)
        {
            editor.Source = imageSource;
        }
    }
}