using AsyncDemoApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AsyncDemoApp
{

	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;			
		}

		#region Sync Execution

		private void executeSync_Click(object sender, RoutedEventArgs e)
		{
			var watch = Stopwatch.StartNew();

			RunDownloadSync();

			watch.Stop();

			var ellapsedMs = watch.ElapsedMilliseconds;

			resultsWindow.Text += $"Total exec. time = {ellapsedMs} ms\n" +
				$"(Window can't be moved And all info pop up simultaneously)";
		}


		private void RunDownloadSync()
		{
			List<string> websites = PredData();

			foreach (var site in websites)
			{
				var result = DownloadWebsite(site);
				ReportWebsiteInfo(result);
			}
		}

		private WebsiteDataModel DownloadWebsite(string url)
		{
			var websiteModel = new WebsiteDataModel();

			//WebClinet is obsolete -> We should use HttpClient instead
			//But HttpClient doesn't have sync method GetString, so I'm using a depricated WebClient

			WebClient client = new();

			websiteModel.WebsiteUrl = url;
			websiteModel.WebsiteData = client.DownloadString(url);

			return websiteModel;
		}

		#endregion

		#region Async Execution

		private async void executeAsync_Click(object sender, RoutedEventArgs e)
		{
			var watch = Stopwatch.StartNew();

			await RunDownloadAsync();

			watch.Stop();

			var ellapsedMs = watch.ElapsedMilliseconds;

			resultsWindow.Text += $"Total exec. time = {ellapsedMs} ms\n" +
				$"(Window can be moved And all info pops up as it's downloaded)";
		}

		private async Task RunDownloadAsync()
		{
		
			List<string> websites = PredData();

			foreach (var site in websites)
			{
				var result = await DownloadWebsiteAsync(site);
				ReportWebsiteInfo(result);
			}
		}

		private async Task<WebsiteDataModel> DownloadWebsiteAsync(string url)
		{
			var websiteModel = new WebsiteDataModel();
			HttpClient client = new();

			websiteModel.WebsiteUrl = url;
			websiteModel.WebsiteData = await client.GetStringAsync(url);

			return websiteModel;
		}

		#endregion

		#region Parallel Asycn Execution

		private async void executeParallelAsync_Click(object sender, RoutedEventArgs e)
		{
			var watch = Stopwatch.StartNew();

			await RunDownloadParallelAsync();

			watch.Stop();

			var ellapsedMs = watch.ElapsedMilliseconds;

			resultsWindow.Text += $"Total exec. time = {ellapsedMs} ms\n" +
				$"(Window can be moved And all info pops up as it's downloaded)";
		}

		private async Task RunDownloadParallelAsync()
		{

			List<string> websites = PredData();
			List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();

			foreach (var site in websites)
			{
				tasks.Add(DownloadWebsiteAsync(site));
			}

			var results = await Task.WhenAll(tasks);

			foreach (var item in results)
			{
				ReportWebsiteInfo(item);
			}

		}

		#endregion

		#region General Actions

		private List<string> PredData()
		{
			resultsWindow.Text = string.Empty;
			List<string> output = new();

			

			output.Add("https://www.yahoo.com");
			output.Add("https://www.google.com");
			return output;

		}

		


		

		private void ReportWebsiteInfo(WebsiteDataModel dataModel)
		{
			resultsWindow.Text += $"URL: {dataModel.WebsiteUrl} | downloaded {dataModel.WebsiteData.Length} symbols.{Environment.NewLine}";
		}

		#endregion

		
	}
}
