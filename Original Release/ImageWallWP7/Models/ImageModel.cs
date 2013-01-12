using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading;
//
// Application.js
// Creted:	12.2012.
// Author:	Aleksandar Toplek
//

using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ImageWallWP7.Models;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;

namespace ImageWallWP7.Models {
	/// <summary>
	/// Class that contains basic info about image that is shown in ImagesPage
	/// </summary>
	public class ImageModel {
		public string Name { get; set; }
		public ImageSource ImageSource { get; set; }
		public Stream ImageStream { get; set; }
		public Picture Picture { get; set; }
	}
}
