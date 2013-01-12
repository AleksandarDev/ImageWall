//
// Application.js
// Creted:	12.2012.
// Author:	Aleksandar Toplek
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ImageWallWP7.ImageWallServiceReference;
using ImageWallWP7.Models;

namespace ImageWallWP7.Helpers {
	public static class ImageUploadHelper {
		public const int PacketSize = 2048;

		public static ImageUploadManager UploadImage(ImageWallServiceClient client, ImageModel model, IEnumerable<string> tags, string author, double latitude, double longitude) {
			try {
				// Gets image buffer
				var buffer = ImageUploadHelper.GetBuffer(model.ImageStream);

				// Create hash for image
				string hash = ImageUploadHelper.GetHash(buffer);

				// Build image details
				var details = new ImageDetails() {
					Name = System.IO.Path.GetFileName(model.Name),
					Hash = hash,
					Tags = new ObservableCollection<string>(tags),
					UserId = author,
					Created = DateTime.Now,
					Latitude = latitude,
					Longitude = longitude,

					Size = buffer.Length,
					Extension = System.IO.Path.GetExtension(model.Name)
				};

				// Begin upload with upload manager
				var manager = new ImageUploadManager(buffer, details);
				manager.BeginUploadAsync();
				return manager;
			}
			catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex);
				return null;
			}
		}

		/// <summary>
		/// Calculates MD5 hash of given byte array
		/// </summary>
		/// <param name="buffer">Byte array for which to calulate hash</param>
		/// <returns>Returns string representing hex hash of given byte array</returns>
		public static string GetHash(byte[] buffer) {
			var md5Hash = MD5.Create().ComputeHash(buffer);
			return md5Hash.Aggregate(String.Empty, (current, hashByte) => current + hashByte.ToString("X2"));
		}

		/// <summary>
		/// Reads byte array from given image stream
		/// </summary>
		/// <param name="stream">Stream from which to read bytes</param>
		/// <returns>Returns byte array containing bytes from given stream</returns>
		public static byte[] GetBuffer(Stream stream) {
			byte[] buffer = null;
			BinaryReader br = new BinaryReader(stream);
			
			if (stream != null && stream.Length > 0) {
				if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);
				buffer = br.ReadBytes((Int32)stream.Length);
			}

			return buffer;
		}
	}
}
