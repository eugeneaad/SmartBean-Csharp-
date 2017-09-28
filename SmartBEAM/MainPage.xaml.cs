using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SmartBEAM.Resources;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Threading;

namespace SmartBEAM
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private SmartBEAMDataContext smartbeamDB;

        private ObservableCollection<SmartBEAMDB> _time,_channel;
        public StreamSocket receviesocket;
        BackgroundWorker bw = new BackgroundWorker();

        TimePicker timepk = new TimePicker();
        DateTime tpktime = new DateTime();
        DateTime timenow;
        string receivechanneldata;
        bool diditsend = true;
        int now;

        #region IR Code
        string stringPower = "0xFF00FF";
        string stringUp = "0xFF08F7";
        string stringDown = "0xFF28D7";
        string stringLeft = "0xFF8877";
        string stringRight = "0xFFC837";
        string stringOK = "0xFF6897";
        string stringVolumeUp = "0xFF9867";
        string stringVolumeDown = "0xFF38C7";
        string stringMenu = "0xFF708F";
        //string stringExit = "0xFFF00F";
        string stringMute = "0xFF807F";
        string stringPlayPause = "0xFFA05F";
        string stringNext = "0xFF58A7";
        string stringPrevious = "0xFFE817";
        string stringStop = "0xFFE01F";
        string stringPlayBack = "0xFF10EF";
        string stringPlayForward = "0xFF50AF";
        string stringPhoto = "0xFF40BF";
        string stringText = "0xFF609F";
        string stringMusic = "0xFFC03F";

        #endregion


        public ObservableCollection<SmartBEAMDB> smartbeamtime
        {
            get
            {
                return _time;
            }
            set
            {
                if (_time != value)
                {
                    _time = value;
                    NotifyPropertyChanged("smartbeamtime");
                }
            }
        }

        public ObservableCollection<SmartBEAMDB> smartbeamchannel
        {
            get
            {
                return _channel;
            }
            set
            {
                if (_channel != value)
                {
                    _channel = value;
                    NotifyPropertyChanged("smartbeamchannel");
                }
            }
        }
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            grid1.Children.Add(timepk);
            timenow = new DateTime();
            smartbeamDB = new SmartBEAMDataContext(SmartBEAMDataContext.DBConnectionString);
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
            this.DataContext = this;
            
            StartTimer(1000);
            
        }

        public void StartTimer(int dueTime)
        {
            Timer t = new Timer(new TimerCallback(TimerProc));
            t.Change(dueTime, 1000);
            

        }
        
        private void TimerProc(object state)
        {
            // The state object is the Timer object.
            Timer t = (Timer)state;
            //t.Dispose();
            UpdateUI(() =>
            {
                showtimenow.Text = DateTime.Now.ToString();
                now = DateTime.Now.Second;
                if (now == 0&&diditsend==false)
                {
                    diditsend = true;

                }
                
                // Your UI Update Code Here
                TimeMatched.ItemsSource = smartbeamtime.Where(oly => oly.tableTime.Contains(DateTime.Now.ToShortTimeString()));
                string matchedChannel = "";
                if (TimeMatched.ItemsSource != null && diditsend ==true)
                {
                    for (int i = 0; i < TimeMatched.Items.Count(); i++)
                    {
                        TimeMatched.SelectedIndex = i;
                        SmartBEAMDB matched = new SmartBEAMDB();
                        matched = (SmartBEAMDB)TimeMatched.SelectedItem;
                        matchedChannel = matched.tableChannel;
                        
                        switch (matchedChannel)
                        {
                            case "Power":
                                PaymentSendAsync(stringPower);
                                break;
                            case "Up":
                                PaymentSendAsync(stringUp);
                                break;
                            case "Down":
                                PaymentSendAsync(stringDown);
                                break;
                            case "Left":
                                PaymentSendAsync(stringLeft);
                                break;
                            case "Right":
                                PaymentSendAsync(stringRight);
                                break;
                            case "OK":
                                PaymentSendAsync(stringOK);
                                break;
                            case "VolumeUp":
                                PaymentSendAsync(stringVolumeUp);
                                break;
                            case "VolumeDown":
                                PaymentSendAsync(stringVolumeDown);
                                break;
                            case "Mute":
                                PaymentSendAsync(stringMute);
                                break;
                            case "PlayPause":
                                PaymentSendAsync(stringPlayPause);
                                break;
                            case "Next":
                                PaymentSendAsync(stringNext);
                                break;
                            case "Previous":
                                PaymentSendAsync(stringPrevious);
                                break;
                            case "Stop":
                                PaymentSendAsync(stringStop);
                                break;
                            case "PlayBack":
                                PaymentSendAsync(stringPlayBack);
                                break;
                            case "PlayForward":
                                PaymentSendAsync(stringPlayForward);
                                break;
                            case "Photo":
                                PaymentSendAsync(stringPhoto);
                                break;
                            case "Text":
                                PaymentSendAsync(stringText);
                                break;
                            case "Music":
                                PaymentSendAsync(stringMusic);
                                break;
                            case "Menu":
                                PaymentSendAsync(stringMenu);
                                break;
                            //case "Exit":
                            //    PaymentSendAsync(stringExit);
                            //    break;
                            
                        }

                        diditsend = false;
                        //string command = "TURN_OFF_RED";
                        //PaymentSendAsync(command);
                    }
                }
            });
        }
        public void UpdateUI(Action displayCall)
        {
            if (Dispatcher.CheckAccess() == false)
            {
                Dispatcher.BeginInvoke(() =>
                     displayCall()
                );
            }
            else
            {
                displayCall();
            }
        }


        


        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new
                    PropertyChangedEventArgs(propertyName));
            }
        }
        
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

            if (receviesocket == null)
            {
                MessageBox.Show("Please setup the bluetooth connection!");
                this.NavigationService.Navigate(new Uri("/BT.xaml", UriKind.Relative));
            }
            else
            {
                if (receivechanneldata != null)
                {
                    //---create a new customer object---
                    tpktime = (DateTime)timepk.Value;

                    SmartBEAMDB newtime = new SmartBEAMDB
                    {

                        tableTime = tpktime.ToShortTimeString(),
                        tableChannel = receivechanneldata
                    };

                    //---add a customer to the observable collection---
                    smartbeamtime.Add(newtime);

                    //---add the customer to the database---
                    smartbeamDB.smartbeam.InsertOnSubmit(newtime);
                    diditsend = true;
                    receivechanneldata = null;
                }
                else
                {
                    MessageBox.Show("Please choose a channel");
                }
            }
        }

        protected override void OnNavigatedTo(
                System.Windows.Navigation.NavigationEventArgs e)
        {
            //---call the base method---
            base.OnNavigatedTo(e);

            //---define the query to gather all of the customers---
            var timeInDB = from SmartBEAMDB smartbeamdb in
                               smartbeamDB.smartbeam
                           select smartbeamdb;

            //    customerDB.Customers
            //select customer;

            //---execute the query and place the results into a 
            // collection---
            //smartbeamchannel = new ObservableCollection<SmartBEAMDB>(timeInDB);
            smartbeamtime = new ObservableCollection<SmartBEAMDB>(timeInDB);

            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }

            //if (NavigationContext.QueryString.ContainsKey("key"))
            //{
            //    receivechanneldata = NavigationContext.QueryString["key"];
            //    //txttest.Text = receivechanneldata;
            //    // etc ...
            //}
            if (PhoneApplicationService.Current.State.ContainsKey("Text"))
                receivechanneldata = (string)PhoneApplicationService.Current.State["Text"];
        }

        protected override void OnNavigatedFrom(
        System.Windows.Navigation.NavigationEventArgs e)
        {
            //---call the base method---
            base.OnNavigatedFrom(e);

            //---save changes to the database---
            smartbeamDB.SubmitChanges();

            controller passsocket = e.Content as controller;
            if (passsocket != null)
            {
                passsocket.socket1 = receviesocket;
            }
            
        }

        private async void PaymentSendAsync(string command)
        {

            if (receviesocket != null)
            {
                DataWriter writer = new DataWriter(receviesocket.OutputStream);
                writer.WriteString(command);
                await writer.StoreAsync();
            }
            else
            {
                MessageBox.Show("Please setup the Bluetooth Connection");
                this.NavigationService.Navigate(new Uri("/BT.xaml", UriKind.Relative));
            }

        }

        private void deleteCustomerButton_Click(object sender,
        RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                //---get a reference customer bound to the button---
                SmartBEAMDB customerForDelete = btn.DataContext as SmartBEAMDB;

                //---remove the customer from the observable collection---
                smartbeamtime.Remove(customerForDelete);
                //smartbeamchannel.Remove(customerForDelete);

                //---remove the customer from the database---
                smartbeamDB.smartbeam.DeleteOnSubmit(customerForDelete);

                //---save changes to the database---
                smartbeamDB.SubmitChanges();

                //---set the focus back to the main page---
                this.Focus();
            }
        }


        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string command = "TURN_OFF_RED";
            PaymentSendAsync(command);
        }

        private void btnAddChannel_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/controller.xaml", UriKind.Relative));
            
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string command = "TURN_OFF_RED";
            PaymentSendAsync(command);
            
        }

        private void btnPower_Click(object sender, RoutedEventArgs e)
        {
            PaymentSendAsync(stringPower);
        }

        private void Mute_Click(object sender, RoutedEventArgs e)
        {
            PaymentSendAsync(stringMute);
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            PaymentSendAsync(stringUp);
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            PaymentSendAsync(stringDown);
        }

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            PaymentSendAsync(stringLeft);
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            PaymentSendAsync(stringRight);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            PaymentSendAsync(stringOK);
        }

        private void VolumeUp_Click(object sender, RoutedEventArgs e)
        {
            PaymentSendAsync(stringVolumeUp);
            soundSlider.Value++;
        }

        private void VolumeDown_Click(object sender, RoutedEventArgs e)
        {
            PaymentSendAsync(stringVolumeDown);
            soundSlider.Value--;
        }

        private void btnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            PaymentSendAsync(stringPlayPause);
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            PaymentSendAsync(stringNext);
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            PaymentSendAsync(stringPrevious);
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            PaymentSendAsync(stringStop);
        }

        private void btnPlayForward_Click(object sender, RoutedEventArgs e)
        {
            PaymentSendAsync(stringPlayForward);
        }

        private void btnPlayBack_Click(object sender, RoutedEventArgs e)
        {
            PaymentSendAsync(stringPlayBack);
        }

        private void btnText_Click(object sender, RoutedEventArgs e)
        {
            PaymentSendAsync(stringText);
        }

        private void btnMusic_Click(object sender, RoutedEventArgs e)
        {
            PaymentSendAsync(stringMusic);
        }

        private void btnPhoto_Click(object sender, RoutedEventArgs e)
        {
            PaymentSendAsync(stringPhoto);
        }

        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            PaymentSendAsync(stringMenu);
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}


    }
}