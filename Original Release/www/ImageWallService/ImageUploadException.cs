//
// Application.js
// Creted:	12.2012.
// Author:	Aleksandar Toplek
//

using System;
using System.Runtime.Serialization;

namespace ImageWallService {
	public class ImageUploadException : Exception {
		public ImageUploadException() : base() {}
		public ImageUploadException(string message) : base(message) {}
		public ImageUploadException(string message, Exception innerException) : base(message, innerException) {}
		public ImageUploadException(SerializationInfo info, StreamingContext context) : base(info, context) {}
	}
}