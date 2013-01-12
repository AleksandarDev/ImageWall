//
// Application.js
// Creted:	12.2012.
// Author:	Aleksandar Toplek
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ImageWallWP7.Helpers;
using ImageWallWP7.ImageWallServiceReference;
using ImageWallWP7.Models;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;

namespace ImageWallWP7.ViewModels {
	public class AddTagsViewModel : DependencyObject {
		private LocationProvider locationProvider;
		private double longitude, latitude;


		public AddTagsViewModel() {
			this.ProgressCurrent = 0;
			this.Tags = new ObservableCollection<string>();
			this.NearbyTags = new ObservableCollection<string>();

			this.locationProvider = new LocationProvider();
			this.locationProvider.OnLocationCoordinateChanged += locationProvider_OnLocationCoordinateChanged;
			this.locationProvider.OnLocationCoordinateUpdated += locationProvider_OnLocationCoordinateChanged;
		}


		public void AssignModel(ImageModel model) {
			if (model.Picture != null) {
				var bitmap = new BitmapImage();
				bitmap.SetSource(model.Picture.GetImage());
				model.ImageStream = model.Picture.GetImage();
				model.ImageSource = bitmap;
			}

			this.Model = model;
		}

		private void locationProvider_OnLocationCoordinateChanged(object sender, LocationChangedEventArgs e) {
			System.Diagnostics.Debug.WriteLine(e.LocationCoordinate.ToString());

			this.latitude = e.LocationCoordinate.Latitude;
			this.longitude = e.LocationCoordinate.Longitude;

			ImageWallServiceClient client = new ImageWallServiceClient();
			client.GetNearbyTagsCompleted += ClientGetNearbyTagsCompleted;
			client.GetNearbyTagsAsync(e.LocationCoordinate.Latitude, e.LocationCoordinate.Longitude, 0.05);
		}

		private void ClientGetNearbyTagsCompleted(object sender, GetNearbyTagsCompletedEventArgs e) {
			System.Diagnostics.Debug.WriteLine(e.Result.Count);

			if (e.Result.Count > 0)
				this.IsPopularTagsVisible = Visibility.Visible;
			else this.IsPopularTagsVisible = Visibility.Collapsed;

			this.NearbyTags.Clear();
			foreach (var tagModel in e.Result) {
				this.NearbyTags.Add(tagModel);
			}
		}

		public void UploadImage() {
			var client = new ImageWallServiceReference.ImageWallServiceClient();

			object deviceID = null;
			string deviceName = "Unknown";
			if (DeviceExtendedProperties.TryGetValue("DeviceUniqueId", out deviceID)) {
				byte[] bID = (byte[])deviceID;
				deviceName = Convert.ToBase64String(bID);
			}

			this.ProgressCurrent = 0;
			var manager = ImageUploadHelper.UploadImage(client, this.Model, this.Tags, deviceName, this.latitude, this.longitude);
			manager.OnUploadProgressChange += manager_OnUploadProgressChange;
			manager.OnUploadComplete += manager_OnUploadComplete;
			this.IsNotUploading = false;
		}

		void manager_OnUploadComplete(object sender, EventArgs e) {
			this.ProgressCurrent = 0;
			System.Diagnostics.Debug.WriteLine("Upload complete!");

			this.Dispatcher.BeginInvoke(() => {
				this.IsNotUploading = true;
				MessageBox.Show("Image successfully uploaded!", "ImageWall", MessageBoxButton.OK);
				App.NavigateToMainView();
			});
		}

		void manager_OnUploadProgressChange(object sender, UploadProgressEventArgs e) {
			double progress = e.Current/(double) e.Total;
			this.ProgressCurrent = progress;
			System.Diagnostics.Debug.WriteLine("Upload progress: " + progress);
		}

		#region Properties

		public ObservableCollection<string> Tags { get; set; }
		public ObservableCollection<string> NearbyTags { get; set; }


		public bool IsNotUploading {
			get { return (bool)GetValue(IsNotUploadingProperty); }
			set { SetValue(IsNotUploadingProperty, value); }
		}

		public static readonly DependencyProperty IsNotUploadingProperty =
			DependencyProperty.Register("IsNotUploading", typeof(bool), typeof(AddTagsViewModel), new PropertyMetadata(true));


		public Visibility IsPopularTagsVisible {
			get { return (Visibility)GetValue(IsPopularTagsVisibleProperty); }
			set { SetValue(IsPopularTagsVisibleProperty, value); }
		}

		public static readonly DependencyProperty IsPopularTagsVisibleProperty =
			DependencyProperty.Register("IsPopularTagsVisible", typeof(Visibility), typeof(AddTagsViewModel), new PropertyMetadata(Visibility.Collapsed));


		public ImageModel Model {
			get { return (ImageModel)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}

		public static readonly DependencyProperty ModelProperty =
			DependencyProperty.Register("Model", typeof(ImageModel), typeof(AddTagsViewModel), new PropertyMetadata(default(ImageModel)));


		public double ProgressCurrent {
			get { return (double)GetValue(ProgressCurrentProperty); }
			set { SetValue(ProgressCurrentProperty, value); }
		}

		public static readonly DependencyProperty ProgressCurrentProperty =
			DependencyProperty.Register("ProgressCurrent", typeof(double), typeof(AddTagsViewModel), new PropertyMetadata(0.0));

		#endregion
	}
}
