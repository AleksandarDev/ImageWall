//
// Application.js
// Creted:	12.2012.
// Author:	Aleksandar Toplek
//

using System;
using System.Device.Location;

namespace ImageWallWP7.Helpers {
	public class LocationChangedEventArgs : EventArgs {
		public string LocationName { get; set; }
		public GeoCoordinate LocationCoordinate { get; set; }
	}
}