//
// Application.js
// Creted:	12.2012.
// Author:	Aleksandar Toplek
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ImageWallWP7.Models;
using ImageWallWP7.ViewModels;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;

namespace ImageWallWP7 {
	public partial class MainView : PhoneApplicationPage {
		private static MainView view;
		private MainViewModel viewModel;
		private double lastFetch;
		private ScrollViewer listScrollViewer;

		
		public MainView() {
			InitializeComponent();

			// Gives access to static methods to this view
			MainView.view = this;
		}

		private void MainViewLoaded(object sender, RoutedEventArgs e) {
			// Loads view model
			this.viewModel = new MainViewModel(this.TitlePanel.ActualHeight);
			this.viewModel.OnImageTap += OnImageTap;
			this.ContentPanel.DataContext = viewModel;

			// Diable take photo button if no camera support
			if (!PhotoCamera.IsCameraTypeSupported(CameraType.Primary)) {
				this.TakePhotoButton.Visibility = Visibility.Collapsed;
			}
		}

		private void OnImageTap(object sender, EventArgs e) {
			if (!(sender is ImageModel)) {
				System.Diagnostics.Debug.WriteLine("Sender isn't ImageModel!");
				return;
			}

			App.SelectedImage = sender as ImageModel;
			this.Dispatcher.BeginInvoke(App.NavigateToAddTagsView);
		}

		private void LoadMoreImages() {
			// Loads available images
			this.viewModel.LoadPages(1, true);
		}

		private void TakePhotoClick(object sender, RoutedEventArgs e) {
			CameraCaptureTask cameraCapture = new CameraCaptureTask();
			cameraCapture.Completed += CameraCaptureCompleted;
			cameraCapture.Show();
		}

		void CameraCaptureCompleted(object sender, PhotoResult e) {
			if (e.TaskResult == TaskResult.OK) {
				var bitmap = new BitmapImage();
				bitmap.SetSource(e.ChosenPhoto);

				OnImageTap(new ImageModel() {
					Name = e.OriginalFileName,
					ImageSource = bitmap,
					ImageStream = e.ChosenPhoto
				}, null);
			}
		}

		#region ListBox methods 

		private void ScrollViewerLoaded(object sender, RoutedEventArgs e) {
			listScrollViewer = sender as ScrollViewer;

			// Bind to vertical offset property of list box
			Binding binding = new Binding();
			binding.Source = listScrollViewer;
			binding.Path = new PropertyPath("VerticalOffset");
			binding.Mode = BindingMode.OneWay;
			this.SetBinding(ListVerticalOffsetProperty, binding);
		}

		private static void OnListVerticalOffsetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			ScrollViewer viewer = view.listScrollViewer;

			if (viewer != null) {
				// Check if scrolling to bottom
				if (view.lastFetch < viewer.ScrollableHeight) {
					// Trigger within 1/4 the viewport.
					if (viewer.VerticalOffset >= viewer.ScrollableHeight - viewer.ViewportHeight/0.2) {
						view.lastFetch = viewer.ScrollableHeight;
						view.LoadMoreImages();
					}
				}
			}
		}

		#endregion

		#region Properties

		public static readonly DependencyProperty ListVerticalOffsetProperty = DependencyProperty.Register(
			"ListVerticalOffset",
			typeof (double),
			typeof (MainView),
			new PropertyMetadata(OnListVerticalOffsetChanged));

		public double ListVerticalOffset {
			get { return (double) this.GetValue(ListVerticalOffsetProperty); }
			set { this.SetValue(ListVerticalOffsetProperty, value); }
		}

		#endregion
	}
}