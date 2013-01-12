//
// Application.js
// Creted:	12.2012.
// Author:	Aleksandar Toplek
//
// TODO Remove size variable from TagDetails (Data model)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ImageWallService {
	// TODO Comment
	[ServiceContract(SessionMode = SessionMode.Allowed)]
	public interface IImageWallService {
		#region Image uploading

		[OperationContract]
		[WebInvoke(Method = "PUT",
			UriTemplate = "BeginUpload?Name={name}&Tags={tag}&Hash={hash}&UserID={userId}&Date={created}&Longitude={longitude}&Latitude={latitude}",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json)]
		bool BeginImageUploadREST(string name, string tag, string hash, string userId, DateTime created, double longitude, double latitude);

		[OperationContract]
		bool BeginImageUpload(ImageDetails details);

		[OperationContract]
		[WebInvoke(	Method = "POST", 
					UriTemplate = "UploadPart?Index={index}&Part={imageBytes}", 
					BodyStyle = WebMessageBodyStyle.Wrapped, 
					RequestFormat = WebMessageFormat.Json, 
					ResponseFormat = WebMessageFormat.Json)]
		ImagePartUploadResult UploadImagePart(int index, byte[] imageBytes);

		#endregion

		#region Image Retrieving

		[OperationContract]
		[WebGet(UriTemplate = "GetAllTags",
				BodyStyle = WebMessageBodyStyle.Wrapped,
				RequestFormat = WebMessageFormat.Json,
				ResponseFormat = WebMessageFormat.Json)]
		IEnumerable<TagModel> GetAllTags();

		[OperationContract]
		[WebGet(UriTemplate = "GetTagModified?Tag={tag}",
				BodyStyle = WebMessageBodyStyle.Wrapped,
				RequestFormat = WebMessageFormat.Json,
				ResponseFormat = WebMessageFormat.Json)]
		DateTime GetTagModified(string tag);

		[OperationContract]
		[WebGet(UriTemplate = "GetNearbyTags?Latitude={latitude}&Longitude={longitude}&Distance={distance}",
			BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json)]
		IEnumerable<string> GetNearbyTags(double latitude, double longitude, double distance);
		
		[OperationContract]
		[WebGet(UriTemplate = "GetImagesDate?DateStart={startDate}&DateEnd={endDate}",
				BodyStyle = WebMessageBodyStyle.Wrapped,
				RequestFormat = WebMessageFormat.Json,
				ResponseFormat = WebMessageFormat.Json)]
		IEnumerable<ImageDetails> GetImagesByDate(string startDate, string endDate);

		[OperationContract]
		[WebGet(UriTemplate = "GetImagesAmount?Amount={maxAmount}&Skip={skip}",
				BodyStyle = WebMessageBodyStyle.Wrapped,
				RequestFormat = WebMessageFormat.Json,
				ResponseFormat = WebMessageFormat.Json)]
		IEnumerable<ImageDetails> GetImagesByAmount(int maxAmount, int skip);

		[OperationContract]
		[WebGet(UriTemplate = "GetImagesTagDate?Tag={tag}&DateStart={startDate}&DateEnd={endDate}",
				BodyStyle = WebMessageBodyStyle.Wrapped,
				RequestFormat = WebMessageFormat.Json,
				ResponseFormat = WebMessageFormat.Json)]
		IEnumerable<ImageDetails> GetImagesByTagDate(string tag, string startDate, string endDate);

		[OperationContract]
		[WebGet(UriTemplate = "GetImagesTagAmount?Tag={tag}&Amount={maxAmount}&Skip={skip}",
				BodyStyle = WebMessageBodyStyle.Wrapped,
				RequestFormat = WebMessageFormat.Json,
				ResponseFormat = WebMessageFormat.Json)]
		IEnumerable<ImageDetails> GetImagesByTagAmount(string tag, int maxAmount, int skip);

		#endregion
	}

	// TODO Comment
	[DataContract]
	public class ImageDetails {
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string[] Tags { get; set; }

		[DataMember]
		public string Hash { get; set; }

		[DataMember]
		public string UserId { get; set; }

		[DataMember]
		public DateTime Created { get; set; }

		[DataMember]
		public double Latitude { get; set; }

		[DataMember]
		public double Longitude { get; set; }

		[DataMember]
		public string Extension { get; set; }

		[DataMember]
		public Int64 Size { get; set; }

		[DataMember]
		public string Url { get; set;}

		[DataMember]
		public string UrlThumbnail { get; set; }


		public static implicit operator ImageDetails(Data.ImageDetails details) {
			return new ImageDetails() {
				Created = details.Date,
				Hash = details.Hash,
				Latitude = details.Latitude,
				Longitude = details.Longitude,
				Name = details.Name,
				Tags = details.ImageTag.Select(t => t.Alias).ToArray(),
				UserId = details.User.UserId,
				Url = details.URL,
				UrlThumbnail = details.URLThumb,

				Size = -1,
				Extension = ".*"
			};
		}
	}

	// TODO Comment
	[DataContract]
	public class TagModel {
		[DataMember]
		public string Alias { get; set; }

		[DataMember]
		public int Size { get; set; }

		[DataMember]
		public bool IsPopular { get; set; }

		[DataMember]
		public DateTime Date { get; set; }

		[DataMember]
		public string UserID { get; set; }


		public static implicit operator TagModel(Data.ImageTag tag) {
			return new TagModel() {
				Alias = tag.Alias,
				Date = tag.Date,
				IsPopular = tag.IsPopular,
				Size = tag.ImageDetails.Count,
				UserID = tag.User.UserId
			};
		}
	}

	// TODO Comment
	[DataContract]
	public class ImagePartUploadResult {
		[DataMember]
		public int RecievedPart { get; set; }

		[DataMember]
		public bool HasError { get; set; }

		[DataMember]
		public Exception Error { get; set; }
	}
}
