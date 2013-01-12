using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using ImageWallClientDesktop.ImageWallServiceReference;
using Microsoft.Win32;

namespace ImageWallClientDesktop.Views {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainView : Window {
		private ImageWallServiceClient client;
		private const int packetSize = 2867;


		public MainView() {
			InitializeComponent();

			System.Diagnostics.Debug.WriteLine((new DateTime(1970, 01, 01)).AddMilliseconds(long.Parse("1355759923321")));
		}

		private void MainViewLoaded(object sender, RoutedEventArgs e) {
			try {
				this.client = new ImageWallServiceClient();
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void GetDataButton_OnClick(object sender, RoutedEventArgs e) {
			OpenFileDialog fd = new OpenFileDialog();
			fd.Filter = "JPEG files|*.jpg";
			if (fd.ShowDialog() == true) {
				try {
					Stream s = fd.OpenFile();
					byte[] bytes = new byte[s.Length];
					s.Read(bytes, 0, (int)s.Length);
					s.Close();

					// Create hash for image
					var md5Hash = System.Security.Cryptography.MD5.Create().ComputeHash(bytes);
					string hashString = md5Hash.Aggregate(String.Empty, (current, hashByte) => current + hashByte.ToString("X2"));

					var details = new ImageDetails() {
						Name = System.IO.Path.GetFileName(fd.FileName),
						Hash = hashString,
						Tags = imageTagTextBox.Text.Split(new[] {','}),
						UserId = "Aleksandar Toplek",
						Created = DateTime.Now,
						Latitude = 0,
						Longitude = 0,

						Size = bytes.Length,
						Extension = System.IO.Path.GetExtension(fd.FileName)
					};

					int partsNeeded = (int)Math.Ceiling((float)bytes.Length/packetSize); // Packets are 20kB
					bool isUploadNeeded = client.BeginImageUpload(details);
					if (isUploadNeeded)
						SendNextPacket(bytes, 0, partsNeeded);
				}
				catch (Exception ex) {
					System.Diagnostics.Debug.WriteLine(ex);
				}
			}
		}

		private void SendNextPacket(byte[] data, int currentPacket, int numberOfPackets) {
			if (currentPacket == numberOfPackets) return;

			byte[] packet = data.Skip(packetSize*currentPacket).Take(packetSize).ToArray();
			var result = client.UploadImagePart(currentPacket, packet);
			if (result == null) {
				System.Diagnostics.Debug.WriteLine("Packet {0}|{1} successfully sent!", currentPacket+1, numberOfPackets);
				this.SendNextPacket(data, currentPacket + 1, numberOfPackets);
			}
			else {
				System.Diagnostics.Debug.WriteLine("Trying to resend package {0}. Error: {1}", currentPacket, result.Message);
				this.SendNextPacket(data, currentPacket, numberOfPackets);
			}
		}
	}
}
