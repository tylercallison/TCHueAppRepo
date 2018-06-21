using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TC.HueApplication.Views
{

  public sealed partial class ConnectionPage : Page
  {
    public ConnectionPage()
    {
      this.InitializeComponent();
      ConnectButton.Click += ConnectButton_Click;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      IEnumerable<LocatedBridge> BridgeList = (IEnumerable<LocatedBridge>)e.Parameter;
      int ipListCount = BridgeList.Count<LocatedBridge>();
      System.Diagnostics.Debug.WriteLine("Initiating ConnectionPage page");
      BridgeCountBlock.Text = String.Format("There were {0} bridge(s) found", ipListCount.ToString());

      BridgeListBox.SelectionMode = SelectionMode.Single;
      foreach (LocatedBridge bridge in BridgeList)
      {
        BridgeListBox.Items.Add(bridge.IpAddress);
        //BridgeListBox.Items.Add("Test");
      }
    }

    private void ConnectButton_Click(object sender, RoutedEventArgs e)
    {
      ConnectAttempt();
    }


    public void ConnectAttempt() { 
      String selectedIP = (String)BridgeListBox.SelectedItem;
        if (selectedIP == null) return;
        System.Diagnostics.Debug.WriteLine("Connecting to {0}", selectedIP);
        System.Diagnostics.Debug.WriteLine(selectedIP);
        ILocalHueClient client = new LocalHueClient(selectedIP);
        String appKey = null;
        Boolean connected = false;
        int timeLeft = 30;

        System.Threading.Thread myThread = new System.Threading.Thread(new System.Threading.ThreadStart(connectLoop));
        myThread.Start();


        async void connectLoop() {
          System.Diagnostics.Debug.WriteLine("Time Left: {0}", timeLeft);
          while (timeLeft > 0 && !connected)
          { 
            try
            {
              System.Diagnostics.Debug.WriteLine("Press link button");
              appKey = await client.RegisterAsync("TCHueApp", "mydevicename");
            }
            catch
            {
              Thread.Sleep(1000);
              timeLeft--;
              System.Diagnostics.Debug.WriteLine("Time Left: {0}", timeLeft);
            }
            finally
            {
              if (appKey != null)
              {
                System.Diagnostics.Debug.WriteLine("appKey has changed");
                connected = true;
              }
            }
          }
          if (!connected) {
            System.Diagnostics.Debug.WriteLine("Failed to connect");
          }
        }

        //does not run after this line

        if (appKey != null && connected)
        {
          System.Diagnostics.Debug.WriteLine(appKey);
          client.Initialize(appKey);
          Properties property = new Properties(@"C:\Program Files\TCHueApp\properties.txt");
          property.set("appKey", appKey);
          System.Diagnostics.Debug.WriteLine("Connected to {0}", selectedIP);
          ConnectionPageFrame.Navigate(typeof(MainRoomPage));
        }
      }
  }
}
