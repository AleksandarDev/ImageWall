//
// Application.js
// Creted:	12.2012.
// Author:	Aleksandar Toplek
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageWallWP7.Models {
	/// <summary>
	/// Class that contains Grid control which is populated with given images
	/// </summary>
	public class ImagesPage {
		private const int GridRows = 4;
		private const int GridColumns = 4;
		public Grid Grid { get; set; }

		public event EventHandler OnImageTap;


		/// <summary>
		/// Class contructor builds grid from given images
		/// </summary>
		/// <param name="models">Images to add to the grid</param>
		/// <param name="hasHightligh">If true, sets one image as highlighted (double size) in random position on the grid</param>
		public ImagesPage(IEnumerable<ImageModel> models, bool hasHightligh = false) {
			// Initialize grid so that it can be populated
			this.InitializeGrid(GridRows, GridColumns);

			// Check if any images were passed 
			if (models == null)
				models = new List<ImageModel>();

			// Take random popular image position inside the grid
			Random random = new Random();
			int randomValue = 0;
			do { randomValue = random.Next(0, 11); } while (randomValue == 3 || randomValue == 7);
			ImageHighlighPositions position = (ImageHighlighPositions) randomValue;

			int imageIndex = 0;
			int currentRow = 0, currentColumn = 0;

			// Go through all available space
			for (int index = 0; index < GridRows * GridColumns; index++, currentColumn++) {
				// Check if any images are left
				if (imageIndex >= models.Count()) break;

				// Go to next row if it's end of current
				if (index != 0 && index%4 == 0) {
					currentColumn = 0;
					currentRow++;
				}

				// If image is highlighted, it can be positioned on following indexes
				// Top-Left 0, 1, 4, 5
				// TopRight 2, 3, 6, 7
				// Bottom-Left 8, 9, 12, 13
				// Bottom-Right 10, 11, 14, 15
				if (hasHightligh) {
					// Position 0 isn't skipped because we need to add image 
					// but size change is accounted before adding image to grid
					if (index - (int) position == 1 ||
						index - (int) position == 4 ||
						index - (int) position == 5) {
						continue;
					}
				}

				// Retrieve image model of current image
				var imageModel = models.ElementAt(imageIndex++);

				// Create Image control and apply properties from image model
				var image = new Image() {
					Source = imageModel.ImageSource,
					Stretch = Stretch.UniformToFill
				};
				image.Tap += (s, e) => { OnImageTap(imageModel, null); };

				// Setup image grid position
				Grid.SetRow(image, currentRow);
				Grid.SetColumn(image, currentColumn);

				// Account for image size if popular
				if (index - (int) position == 0) {
					Grid.SetRowSpan(image, 2);
					Grid.SetColumnSpan(image, 2);

					// Load picture with more detail
					BitmapImage bitmap = new BitmapImage();
					imageModel.ImageSource = bitmap;
					image.Source = imageModel.ImageSource;
					Application.Current.Resources.Dispatcher.BeginInvoke(() => {
						bitmap.SetSource(imageModel.Picture.GetImage());
					});

				}

				// Add image control to grid
				this.Grid.Children.Add(image);
			}
		}

		private void InitializeGrid(int rows, int columns) {
			// Creates new Grid control
			this.Grid = new Grid();

			var gridSpaceSize = Application.Current.Host.Content.ActualWidth/4;
			
			// Creates row definitions
			for (int index = 0; index < rows; index++) {
				this.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(gridSpaceSize) });
			}

			// Creates column definitions
			for (int columnIndex = 0; columnIndex < columns; columnIndex++) {
				this.Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(gridSpaceSize) });
			}
		}
	}
}