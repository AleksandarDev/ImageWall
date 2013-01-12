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
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using ImageWallWP7.Helpers;
using ImageWallWP7.ImageWallServiceReference;
using ImageWallWP7.Models;
using ImageWallWP7.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;
using Microsoft.Phone.Shell;

namespace ImageWallWP7.Views {
	public partial class AddTagsView : PhoneApplicationPage {
		private AddTagsViewModel viewModel;
		private LocationProvider locationProvider;


		public AddTagsView() {
			InitializeComponent();
		}


		private void AddTagsViewLoaded(object sender, RoutedEventArgs e) {
			this.viewModel = new AddTagsViewModel();
			this.ContentPanel.DataContext = this.viewModel;

			if (App.SelectedImage != null)
				this.AssignImageModel(App.SelectedImage);
		}

		public void AssignImageModel(ImageModel model) {
			this.viewModel.AssignModel(model);
		}

		private void UploadImageClick(object sender, RoutedEventArgs e) {
			// Gets custom tags and selected popular tags (comma seperated) and removes spaces and hash tags
			var customTags = this.CustomTagsTextBox.Text.Split(',');
			foreach (var tag in customTags.Union(this.PopularTagsListBox.SelectedItems.Cast<string>()).Distinct()) {
				var tagCorrected = tag.Replace(" ", "").Replace("#", "");
				if (!String.IsNullOrEmpty(tagCorrected))
					this.viewModel.Tags.Add(tagCorrected);
			}

			if (this.viewModel.Tags.Count <= 0) {
				this.Dispatcher.BeginInvoke(() => {
					MessageBox.Show("Please enter at least one tag.", "Image upload", MessageBoxButton.OK);
				});
			}
			else this.viewModel.UploadImage();
		}

		private void CustomTagsTextBox_OnKeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter) {
				this.UploadImageButton.Focus();
			}
		}
	}
}