//
// Application.js
// Creted:	12.2012.
// Author:	Aleksandar Toplek
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ImageWallWP7.Models;
using ImageWallWP7.Views;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;

namespace ImageWallWP7.ViewModels {
	public class MainViewModel : DependencyObject {
		private const int ImagesPerPage = 16;
		private const int ImagesPerPagePopular = 13;
		public ObservableCollection<ImagesPage> Pages { get; set; }
		private int imagesLoaded = 0;

		public event EventHandler OnImageTap;


		/// <summary>
		/// MainViewModel constructor
		/// </summary>
		public MainViewModel(double titleHeight) {
			this.Pages = new ObservableCollection<ImagesPage>();

			// Add spacer to top of page
			var spacerPage = new ImagesPage(null);
			spacerPage.Grid.Height = titleHeight;
			this.Pages.Add(spacerPage);

			// Initiail load if pages
			this.LoadPages(2, true);
		}


		/// <summary>
		/// Loads multiple pages of images that are wither all popular or not
		/// </summary>
		/// <param name="number">Number of pages to load</param>
		/// <param name="arePopular">Set this ti true if all pages are pupular</param>
		/// <remarks>In order to make some pages popular and some not, you have to manually call LoadNextPage method for each that is different from the others</remarks>
		public void LoadPages(int number, bool arePopular = false) {
			for (int index = 0; index < number; index++) {
				this.LoadNextPage(arePopular);
			}
		}

		/// <summary>
		/// Loads image page that contains four rows and four columns and can contain up to 16 images
		/// </summary>
		/// <param name="isPopular">If this is set to true, page will be marked as popular</param>
		public void LoadNextPage(bool isPopular = false) {
			// Work around for known _bug in the media framework.  Hits the static constructors
			// so the user does not need to go to the picture hub first.
			// ReSharper disable ReturnValueOfPureMethodIsNotUsed
			MediaPlayer.Queue.ToString();
			// ReSharper restore ReturnValueOfPureMethodIsNotUsed

			var mediaLib = new MediaLibrary();

			var models = new List<ImageModel>();
			var pictures = mediaLib.Pictures
			                       .OrderByDescending(p => p.Date)
			                       .Skip(this.imagesLoaded)
			                       .Take(isPopular ? ImagesPerPagePopular : ImagesPerPage);
			
			// Go through all pictures
			foreach (var picture in pictures) {
				// Create image model
				var model = new ImageModel() {
					Name = picture.Name,
					Picture = picture
				};
				
				// Create bitmap image and set source to thumbnail stream
				var bitmap = new BitmapImage();
				bitmap.SetSource(picture.GetThumbnail());
				model.ImageSource = bitmap;
				
				// Add mdoel to current list
				models.Add(model);
			}

			if (models.Count > 0) {
				// Increase loaded images number to match real count and add model to the pages list
				this.imagesLoaded += isPopular ? ImagesPerPagePopular : ImagesPerPage;
				var imagesPage = new ImagesPage(models, isPopular);
				imagesPage.OnImageTap += ImagesPageImageTap;
				this.Pages.Add(imagesPage);
			}
		}

		private void ImagesPageImageTap(object sender, EventArgs e) {
			if (!(sender is ImageModel)) {
				System.Diagnostics.Debug.WriteLine("Sender isn't ImageModel!");
				return;
			}

			if (OnImageTap != null) {
				OnImageTap(sender as ImageModel, null);
			}
		}
	}
}
