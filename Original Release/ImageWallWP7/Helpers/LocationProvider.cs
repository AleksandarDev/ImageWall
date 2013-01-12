//
// Application.js
// Creted:	12.2012.
// Author:	Aleksandar Toplek
//

using System;
using System.Device.Location;
using System.Net;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Linq;

namespace ImageWallWP7.Helpers {
	public class LocationProvider {
		private const string BingMapsLocationsRESTAddress = @"http://dev.virtualearth.net/REST/v1/Locations/{0},{1}?o=xml&key={2}";
		private const string BingMapsApplicationKey = @"AqKZ2WhYAxgIlmg9efYuCd6wyQ6hwMd6n5fQZ9sh5lStxL-vEOkfrB5Fii02X2m8";

		private const double MovementTreshold = 10.0;
		private const double WatcherStartDelay = 1.0;
		private GeoCoordinateWatcher watcher;

		private DateTimeOffset lastPositionTimestamp;
		private GeoCoordinate currentLocationCoordinates;
		private string currentLocationName;

		public event LocationChangedEventHandler OnLocationCoordinateChanged;
		public event LocationChangedEventHandler OnLocationNameChanged;
		public event LocationChangedEventHandler OnLocationCoordinateUpdated;
		public event LocationChangedEventHandler OnLocationNameUpdated;


		public LocationProvider() {
			// Setup position watcher
			this.watcher = new System.Device.Location.GeoCoordinateWatcher(System.Device.Location.GeoPositionAccuracy.Default);
			watcher.MovementThreshold = MovementTreshold;
			watcher.TryStart(false, TimeSpan.FromSeconds(WatcherStartDelay));
			watcher.PositionChanged += OnLocationChanged;
		}


		private void OnLocationChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e) {
			// Location chenged event (test if current location and new location are the same)
			if (this.OnLocationCoordinateChanged != null)
				if (e.Position.Location != this.currentLocationCoordinates)
					this.OnLocationCoordinateChanged(this, new LocationChangedEventArgs() {
						LocationCoordinate = e.Position.Location,
						LocationName = this.currentLocationName
					});

			// Checks if this is newest location and saves it
			if (this.lastPositionTimestamp < e.Position.Timestamp) {
				this.currentLocationCoordinates = e.Position.Location;
				this.lastPositionTimestamp = e.Position.Timestamp;
			}
			else System.Diagnostics.Debug.WriteLine("Got outdated GeoCoordinates");

			// Location updated event
			if (this.OnLocationCoordinateUpdated != null)
				this.OnLocationCoordinateUpdated(this, new LocationChangedEventArgs() {
					LocationCoordinate = this.currentLocationCoordinates,
					LocationName = this.currentLocationName
				});

			// If location is valid retrieve location name
			if (!this.currentLocationCoordinates.IsUnknown)
				RetrieveLocationName();
			else System.Diagnostics.Debug.WriteLine("Unknown GeoCoordinates!");
		}

		private void RetrieveLocationName() {
			if (this.currentLocationCoordinates == null || this.currentLocationCoordinates.IsUnknown) {
				System.Diagnostics.Debug.WriteLine("Invalid GeoCoordinates! Can't retirieve location name.");
				return;
			}

			string requestAddress = String.Format(
				BingMapsLocationsRESTAddress,
				this.currentLocationCoordinates.Latitude, this.currentLocationCoordinates.Longitude,
				BingMapsApplicationKey);

			WebClient client = new WebClient();
			client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(ClientDownloadStringCompleted);
			client.DownloadStringAsync(new Uri(requestAddress, UriKind.RelativeOrAbsolute));
		}

		private void ClientDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e) {
			if (e.Cancelled || e.Error != null) {
				System.Diagnostics.Debug.WriteLine("An error occured while downloading location info.");
				return;
			}

			if (String.IsNullOrEmpty(e.Result)) {
				System.Diagnostics.Debug.WriteLine("Got empty response from Bing Maps Service.");
				return;
			}

			// Read response
			List<string> localityes = new List<string>();
			StringReader stream = new StringReader(e.Result);
			XmlReader reader = XmlReader.Create(stream);
			while (reader.Read())
				if (reader.NodeType == XmlNodeType.Element)
					if (reader.Name == "Locality" && reader.Read())
						if (!String.IsNullOrEmpty(reader.Value))
							localityes.Add(reader.Value);

			// Check for response consistency and save result
			// TODO Take max of one item as current location
			if (localityes.Count > 0 && localityes.All(l => l == localityes.FirstOrDefault())) {
				// Get new location name string
				string location = String.IsNullOrEmpty(localityes.FirstOrDefault()) ? "Unknown" : localityes.First();

				// Location chenged event
				if (this.OnLocationNameChanged != null)
					if (this.currentLocationName != location)
						this.OnLocationNameChanged(this, new LocationChangedEventArgs() {
							LocationCoordinate = this.currentLocationCoordinates,
							LocationName = location
						});

				// Save new location name
				this.currentLocationName = location;

				// Location updated event
				if (this.OnLocationNameUpdated != null)
					this.OnLocationNameUpdated(this, new LocationChangedEventArgs() {
						LocationCoordinate = this.currentLocationCoordinates,
						LocationName = this.currentLocationName
					});
			}
		}

		public string GetLocationName() {
			return this.currentLocationName ?? "Unknown";
		}

		public GeoCoordinate GetCoordinatesAsync() {
			return this.currentLocationCoordinates;
		}
	}
}
