//
// Application.js
// Creted:	12.2012.
// Author:	Aleksandar Toplek
//

using System;
using System.Linq;
using ImageWallWP7.ImageWallServiceReference;

namespace ImageWallWP7.Helpers {
	public class UploadProgressEventArgs : EventArgs {
		public int Current { get; set; }
		public int Total { get; set; }
	}

	// TODO Comment
	public class ImageUploadManager {
		private ImageWallServiceClient client;
		private ImageDetails detail;
		private byte[] data;
		private int numberOfPackets;

		public event EventHandler OnUploadComplete;
		public event EventHandler<UploadProgressEventArgs> OnUploadProgressChange;


		public ImageUploadManager(byte[] data, ImageDetails details) {
			this.data = data;
			this.detail = details;

			// Calculate number of packets to send
			this.numberOfPackets = (int)Math.Ceiling((float)this.data.Length / ImageUploadHelper.PacketSize);

			// Create client and event handles
			this.client = new ImageWallServiceClient();
			this.client.UploadImagePartCompleted += HandleUploadImagePartCompleted;
			this.client.BeginImageUploadCompleted += HandleBeginImageUploadComplete;
		}


		public void BeginUploadAsync() {
			// Begin upload
			this.client.BeginImageUploadAsync(this.detail);
		}

		private void HandleBeginImageUploadComplete(object sender, BeginImageUploadCompletedEventArgs e) {
			System.Diagnostics.Debug.WriteLine("ImageDetails uploaded, got result: " + e.Result);
			// Check if we can start uploading
			if (e.Result) {
				this.SendNextPacket(0);
			}
			else {
				if (OnUploadProgressChange != null)
					OnUploadProgressChange(this, new UploadProgressEventArgs() {Current = 1, Total = 1});
				if (OnUploadComplete != null)
					OnUploadComplete(this, null);
			}
		}

		private void SendNextPacket(int currentPacket) {
			if (currentPacket == numberOfPackets) return;

			// Get bytes that need to be sent
			byte[] packet = data.Skip(ImageUploadHelper.PacketSize * currentPacket).Take(ImageUploadHelper.PacketSize).ToArray();

			// Initiate upload
			client.UploadImagePartAsync(currentPacket, packet);
		}

		private void HandleUploadImagePartCompleted(object sender, UploadImagePartCompletedEventArgs e) {
			if (!e.Result.HasError) {
				if (OnUploadProgressChange != null)
					OnUploadProgressChange(this, new UploadProgressEventArgs() { Current = e.Result.RecievedPart, Total = this.numberOfPackets });

				if (e.Result.RecievedPart == numberOfPackets - 1) {
					if (OnUploadComplete != null)
						OnUploadComplete(this, null);
					return;
				}

				this.SendNextPacket(e.Result.RecievedPart + 1);
				System.Diagnostics.Debug.WriteLine("Packet {0}|{1} successfully sent!", e.Result.RecievedPart + 1, numberOfPackets);
			}
			else {
				this.SendNextPacket(e.Result.RecievedPart);
				System.Diagnostics.Debug.WriteLine("Trying to resend package {0}. Error: {1}", e.Result.RecievedPart, e.Result.Error);
			}
		}
	}
}