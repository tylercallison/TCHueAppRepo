using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using TC.HueApplication.Views;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TC.HueApplication.Views
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class FindingBridgePage : Page
  {

    private IEnumerable<LocatedBridge> Bridges { get; set; }

    public FindingBridgePage()
    {
      this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      FindBridgesAsync();
      try
      {
        LoadingIcon.IsActive = true;
        //await LongOperationAsync();
      }
      finally
      {
        //LoadingIcon.IsActive = false;
      }
    }

    public async System.Threading.Tasks.Task FindBridgesAsync()
    {
      try
      {
        LoadingIcon.IsActive = true;
        IBridgeLocator locator = new HttpBridgeLocator();
        System.Diagnostics.Debug.WriteLine("Starting async method");
        IEnumerable<LocatedBridge> bridgeIPs = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));
        Bridges = bridgeIPs;
      }
      finally
      {
        System.Diagnostics.Debug.WriteLine("Finished async method");
        foreach (LocatedBridge bridge in Bridges)
        {
          System.Diagnostics.Debug.WriteLine(bridge.IpAddress);
        }
        LoadingIcon.IsActive = false;
        SearchingFrame.Navigate(typeof(ConnectionPage), Bridges);
      }
    }
  }
}

