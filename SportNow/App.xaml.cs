using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SportNow.Views;
using System.Collections.Generic;
using SportNow.Model;
using Xamarin.Essentials;
using System.Diagnostics;
using Plugin.FirebasePushNotification;
using SportNow.Services.Data.JSON;
using System.Threading.Tasks;
using Plugin.DeviceOrientation;
using Plugin.DeviceOrientation.Abstractions;
using SportNow.Views.Profile;

namespace SportNow
{
    public partial class App : Application
    {

        public static List<Member> members;
        public static Member original_member;
        public static Member member;

        public static string VersionNumber = "1.0.4";
        public static string BuildNumber = "15";

        public static Competition competition;

        public static Competition_Participation competition_participation;
        public static Event_Participation event_participation;

        public static double screenWidthAdapter = 1, screenHeightAdapter = 1;
        public static double screenWidth = 1, screenHeight = 1;
        public static int bigTitleFontSize = 0, titleFontSize = 0, menuButtonFontSize = 0, rankingBigTitleFontSize = 0, rankingTitleFontSize = 0,
                formLabelFontSize = 0, formValueFontSize = 0, itemTitleFontSize = 0, gridTextFontSize = 0, itemTextFontSize = 0, consentFontSize = 0, smallTextFontSize = 0;

        public static int ItemWidth = 0, ItemHeight = 0;

        //SELECTED TABS
        public static string DO_activetab = "quotas";
        public static string EVENTOS_activetab = "estagios";
        public static string EQUIPAMENTOS_activetab = "karategis";

        public static string token = "";

        public static string notification = "";

        public static bool isToPop = false;

        public static Color backgroundcolor = Color.FromRgb(255, 255, 255);
        public static Color topColor = Color.FromRgb(16, 160, 213);
        public static Color bottomColor = Color.FromRgb(5, 91, 121);
        public static Color normalTextColor = Color.FromRgb(117, 117, 117);

        public static string DefaultImageId = "default_image";
        public static string ImageIdToSave = null;
        public static bool feeCreated = false;
        public static bool idDocumentCreated = false;
        public static bool idParentDocumentCreated = false;
        public static bool medicalDocumentCreated = false;

        public App(bool hasNotification = false, object notificationData = null)
        {
            Debug.Print("App Constructor");
            if (hasNotification)
            {
                Debug.Print("App Constructor hasNotification true");
            }
            else
            {
                Debug.Print("App Constructor hasNotification false");
                new App();
            }
        }

        public App()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.Android)
            {
                startNotifications();
            }
            
            checkPreviousLoginOk();

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mjc5NTM3MUAzMjMzMmUzMDJlMzBvZ2xGV2VJekVhaGd3N1dCeEdTa3BTRExzNWI5VjZCRUdsaEQvdVFwcmI4PQ==");

            //MainPage = new MainPage();

            CrossDeviceOrientation.Current.LockOrientation(DeviceOrientations.Portrait);

        }

        public void checkPreviousLoginOk()
        {
            if (App.Current.Properties.ContainsKey("EMAIL") & App.Current.Properties.ContainsKey("PASSWORD"))
            {
                MainPage = new NavigationPage(new MainTabbedPageCS("",""))
                {
                    BarBackgroundColor = Color.White,
                    BackgroundColor = Color.White,
                    BarTextColor = Color.Black//FromRgb(75, 75, 75)
                };
            }
            else
            {

                MainPage = new NavigationPage(new LoginPageCS(""))
                {
                    BarBackgroundColor = Color.FromRgb(15, 15, 15),
                    BarTextColor = Color.Black
                };
            }
        }

        protected void startNotifications() {
            CrossFirebasePushNotification.Current.Subscribe("General");

            CrossFirebasePushNotification.Current.OnTokenRefresh += async (s, p) =>
            {
                Debug.WriteLine($"TOKEN : {p.Token}");
                
                if ((App.original_member != null) & (App.token != p.Token))
                {
                    Debug.Print("App.original_member = " + App.original_member.id + ". App.token =" + App.token + ". p.Token=" + p.Token);
                    MemberManager memberManager = new MemberManager();
                    var result = await memberManager.updateToken(App.original_member.id, p.Token);
                }
                App.token = p.Token;
            };

            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                Debug.Print("OnNotificationReceived Cross");
                App.notification = "OnNotificationReceived";
            };

            CrossFirebasePushNotification.Current.OnNotificationOpened += async (s, p) =>
            {
                Debug.Print("OnNotificationOpened Cross");

                App.notification = "OnNotificationOpened";

                executionActionPushNotification(p.Data);

                if (!string.IsNullOrEmpty(p.Identifier))
                {
                    System.Diagnostics.Debug.WriteLine($"ActionID: {p.Identifier}");
                }

            };
        }

        protected override async void OnStart()

        {
            Debug.Print("OnStart");

            if (Device.RuntimePlatform != Device.Android)
            {
                startNotifications();
            }

        }

        public void executionActionPushNotification(IDictionary<string, object> dataDict)
        {
            var actiontype = "";
            var actionid = "";
            Debug.Print("Opened");
            App.notification = App.notification + " executionActionPushNotification";
            foreach (var data in dataDict)
            {
                Debug.Print("Push Notification " + data.Key.ToString() + " = "+data.Value.ToString());

                if (data.Key == "actiontype")
                {
                    Debug.Print("Push Notification Action = " + data.Value.ToString());
                    actiontype = data.Value.ToString();
                }

                if (data.Key == "actionid")
                {
                    Debug.Print("Push Notification Message = " + data.Value.ToString());
                    actionid = data.Value.ToString();
                    
                }
            }

            App.notification = App.notification + " actiontype = "+ actiontype;
            App.notification = App.notification + " actionid = " + actionid;
            if (((actiontype == "openevent") | (actiontype == "opencompetition") | (actiontype == "openexaminationsession")) & (actionid != ""))
            {
                MainPage = new NavigationPage(new MainTabbedPageCS(actiontype, actionid))
                {
                    BarBackgroundColor = Color.White,
                    BackgroundColor = Color.White,
                    BarTextColor = Color.Black//FromRgb(75, 75, 75)
                };
                //MainPage = new NavigationPage(new DetailEventPageCS(event_i));
            }
            else if ((actiontype == "openweekclass") | (actiontype == "opentodayclass") | (actiontype == "opentodayclassinstructor") | (actiontype == "authorizeregistration") | (actiontype == "openmonthfee"))
            {
                App.Current.MainPage = new NavigationPage(new MainTabbedPageCS(actiontype, actionid))
                {
                    BarBackgroundColor = Color.White,
                    BackgroundColor = Color.White,
                    BarTextColor = Color.Black
                };
            }
        }


        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }


        public static void screenWidthAdapterCalculator()
        {

            screenWidthAdapter = 1;
        }

        public static void screenHeightAdapterCalculator()
        {
            screenHeightAdapter = 1;
        }



        public static void AdaptScreen()
        {
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            App.screenWidth = ((mainDisplayInfo.Width) / mainDisplayInfo.Density);
            App.screenHeight = ((mainDisplayInfo.Height) / mainDisplayInfo.Density);
            App.screenWidthAdapter = ((mainDisplayInfo.Width) / mainDisplayInfo.Density) / 400;
            App.screenHeightAdapter = ((mainDisplayInfo.Height) / mainDisplayInfo.Density) / 850;
            //Debug.Print("App.screenWidthAdapter = " + App.screenWidthAdapter);
            //Debug.Print("App.screenHeightAdapter = " + App.screenHeightAdapter);

            App.ItemWidth = (int)(155 * App.screenWidthAdapter);
            App.ItemHeight = (int)(120 * App.screenHeightAdapter);

            App.rankingBigTitleFontSize = (int)(80 * App.screenHeightAdapter);
            App.rankingTitleFontSize = (int)(24 * App.screenHeightAdapter);

            App.bigTitleFontSize = (int)(26 * App.screenHeightAdapter);
            App.titleFontSize = (int)(18 * App.screenHeightAdapter);
            App.menuButtonFontSize = (int)(14 * App.screenHeightAdapter);
            App.formLabelFontSize = (int)(16 * App.screenHeightAdapter);
            App.formValueFontSize = (int)(16 * App.screenHeightAdapter);
            App.itemTitleFontSize = (int)(16 * App.screenHeightAdapter);
            App.itemTextFontSize = (int)(12 * App.screenHeightAdapter);
            App.gridTextFontSize = (int)(14 * App.screenHeightAdapter);
            App.smallTextFontSize = (int)(10 * App.screenHeightAdapter);
            App.consentFontSize = (int)(14 * App.screenHeightAdapter);
        }
    }

}
