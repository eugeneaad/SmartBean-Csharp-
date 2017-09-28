using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using Windows.Networking.Sockets;
using Microsoft.Phone.Tasks;
using Windows.Networking.Proximity;
using SmartBEAM.Resources;

namespace SmartBEAM
{
    public partial class BT : PhoneApplicationPage
    {

        ObservableCollection<PairedDeviceInfo> _pairedDevices;  // a local copy of paired device information
        StreamSocket _socket; // socket object used to communicate with the device

        public BT()
        {
            InitializeComponent();
            Loaded += BT_Loaded;
        }

        private void BT_Loaded(object sender, RoutedEventArgs e)
        {
            _pairedDevices = new ObservableCollection<PairedDeviceInfo>();
            PairedDevicesList.ItemsSource = _pairedDevices;
        }

        private async void RefreshPairedDevicesList()
        {
            try
            {
                // Search for all paired devices
                PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
                var peers = await PeerFinder.FindAllPeersAsync();

                // By clearing the backing data, we are effectively clearing the ListBox
                _pairedDevices.Clear();

                if (peers.Count == 0)
                {
                    MessageBox.Show(AppResources.Msg_NoPairedDevices);
                }
                else
                {
                    // Found paired devices.
                    foreach (var peer in peers)
                    {
                        _pairedDevices.Add(new PairedDeviceInfo(peer));
                    }
                }
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x8007048F)
                {
                    var result = MessageBox.Show("Bluetooth is turned off. To see the current Bluetooth settings tap 'ok'.", "Bluetooth Off", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        ShowBluetoothcControlPanel();
                    }
                }
                else if ((uint)ex.HResult == 0x80070005)
                {
                    MessageBox.Show("To run this app, you must have ID_CAP_PROXIMITY enabled in WMAppManifest.xaml");
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ShowBluetoothcControlPanel()
        {
            ConnectionSettingsTask connectionSettingsTask = new ConnectionSettingsTask();
            connectionSettingsTask.ConnectionSettingsType = ConnectionSettingsType.Bluetooth;
            connectionSettingsTask.Show();
        }

        private async void ConnectToDevice(PeerInformation peer)
        {
            if (_socket != null)
            {
                // Disposing the socket with close it and release all resources associated with the socket
                _socket.Dispose();
            }

            try
            {
                _socket = new StreamSocket();
                string serviceName = (String.IsNullOrWhiteSpace(peer.ServiceName)) ? tbServiceName.Text : peer.ServiceName;

                // Note: If either parameter is null or empty, the call will throw an exception
                await _socket.ConnectAsync(peer.HostName, serviceName);

                // If the connection was successful, the RemoteAddress field will be populated
                MessageBox.Show(String.Format("Sucessfully Connected to {0}!", peer.DisplayName));
                //DataWriter writer = new DataWriter(_socket.OutputStream);
                //writer.WriteByte(0x22);
                //await writer.StoreAsync();
                if (this.NavigationService.CanGoBack)
                    this.NavigationService.GoBack();

            }
            catch (Exception ex)
            {
                // In a real app, you would want to take action dependent on the type of 
                // exception that occurred.
                MessageBox.Show(ex.Message);

                _socket.Dispose();
                _socket = null;
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            MainPage passsocket = e.Content as MainPage;
            if (passsocket != null)
            {
                passsocket.receviesocket = _socket;
            }
        }


        private void FindPairedDevices_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            RefreshPairedDevicesList();
        }

        private void ConnectToSelected_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // Connect to the device
            PairedDeviceInfo pdi = PairedDevicesList.SelectedItem as PairedDeviceInfo;
            PeerInformation peer = pdi.PeerInfo;

            // Asynchronous call to connect to the device
            ConnectToDevice(peer);
        }

        private void PairedDevicesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check whether the user has selected a device
            if (PairedDevicesList.SelectedItem == null)
            {
                // No - hide these fields
                ConnectToSelected.IsEnabled = false;
                //ServiceNameInput.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Yes - enable the connect button
                ConnectToSelected.IsEnabled = true;

                // Show the service name field, if the ServiceName associated with this device is currently empty
                PairedDeviceInfo pdi = PairedDevicesList.SelectedItem as PairedDeviceInfo;
                //ServiceNameInput.Visibility = (String.IsNullOrWhiteSpace(pdi.ServiceName)) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }

    /// <summary>
    ///  Class to hold all paired device information
    /// </summary>
    public class PairedDeviceInfo
    {
        internal PairedDeviceInfo(PeerInformation peerInformation)
        {
            this.PeerInfo = peerInformation;
            this.DisplayName = this.PeerInfo.DisplayName;
            this.HostName = this.PeerInfo.HostName.DisplayName;
            this.ServiceName = this.PeerInfo.ServiceName;
        }

        public string DisplayName { get; private set; }
        public string HostName { get; private set; }
        public string ServiceName { get; private set; }
        public PeerInformation PeerInfo { get; private set; }
    }
}
