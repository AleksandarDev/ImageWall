//
// Application.js
// Creted:	12.2012.
// Author:	Aleksandar Toplek
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using ImageWallService.Data;

namespace ImageWallService {
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class ImageWallService : IImageWallService {
		public const int DatabaseCacheTimeout = 1;
		public const int DefaultMaxThumbnailWidth = 200;

		#region Static variables

		public static string ServerPath = HttpContext.Current.Server.MapPath(".");
		public static string ImagesDirectory = ConfigurationManager.AppSettings["ImagesDirectory"];
		public static ImageWallServiceDataModelContainer Database = new ImageWallServiceDataModelContainer();
		public static DateTime DatabaseLastTimeCached;		

		#endregion

		public static void CheckIfCacheNeeded() {
			// Do database cache if more than one minute passed
			if (ImageWallService.DatabaseLastTimeCached < DateTime.Now - TimeSpan.FromMinutes(ImageWallService.DatabaseCacheTimeout))
				ImageWallService.CacheDatabase();
		}

		public static void CacheDatabase() {
			if (ImageWallService.Database == null) {
				throw new NullReferenceException("Database not initialized!");
			}

			ImageWallService.Database.Database.Connection.Open();

			ImageWallService.Database.ImageDetailsSet.Load();
			ImageWallService.Database.ImageTagSet.Load();
			ImageWallService.Database.UserSet.Load();

			ImageWallService.Database.Database.Connection.Close();

			ImageWallService.DatabaseLastTimeCached = DateTime.Now;
			System.Diagnostics.Debug.WriteLine("Databse cached {0}", ImageWallService.DatabaseLastTimeCached.ToString());
		}

		#region IImageWallService implementation

		#region Image Uploading

		private bool isReceivingImage;
		private ImageDetails imageDetails;
		private Dictionary<int, byte[]> imageParts;
		private Int64 bytesLeft;

		public bool BeginImageUploadREST(string name, string tag, string hash, string userId, DateTime created, double longitude, double latitude) {
			var details = new ImageDetails() {
				Name = name,
				Tags = new[] {tag},
				Created = created,
				Hash = hash,
				UserId = userId,
				Longitude = longitude,
				Latitude = latitude
			};

			return BeginImageUpload(details);
		}

		public bool BeginImageUpload(ImageDetails details) {
			if (details == null || String.IsNullOrEmpty(details.Hash) || details.Size <= 0)
				throw new ImageUploadException("Invalid image details!");

			// Set tag to "Other" if tag is not defined
			if (details.Tags == null || details.Tags.Length <= 0)
				details.Tags = new[] {"Other"};

			// If image exists, only update tags and cancel upload
			if (this.ImageExists(details.Hash)) {
				System.Diagnostics.Debug.WriteLine(format: "Image[{0}] already exists! Updating tags.", args: details.Hash);

				this.UpdateImageTags(details);
				return false;
			}
			
			// Set receiving flags to begin upload
			this.isReceivingImage = true;
			this.imageDetails = details;
			this.imageParts = new Dictionary<int, byte[]>();
			this.bytesLeft = details.Size;

			return true;
		}

		private void UpdateImageTags(ImageDetails details) {
			ImageWallService.CheckIfCacheNeeded();

			// Retireve all image details that match in hash code
			var imageFromDatabase = (from img in ImageWallService.Database.ImageDetailsSet
			                        where img.Hash == details.Hash
			                        select img).ToList();

			// Check if there is image to be updated
			if (imageFromDatabase.Count <= 0) return;

			// Take only first matched image details
			var image = imageFromDatabase.First();

			// Retrieve tags and add new tags to image
			var allTags = this.RetrieveTags(details.Tags);
			foreach (var tag in allTags) {
				if (!image.ImageTag.Contains(tag)) {
					// Set user and increase size variables
					tag.User = image.User;

					// Add tag to image
					image.ImageTag.Add(tag);
				}

				// Increase tag size and update date
				tag.Date = DateTime.Now;
			}

			// Update image change date if any tag change has happened
			if (allTags.Count > 0)
				image.Date = DateTime.Now;

			// Save changes of image
			ImageWallService.Database.Database.Connection.Open();
			ImageWallService.Database.SaveChanges();
			ImageWallService.Database.Database.Connection.Close();
		}

		private bool ImageExists(string imageHash) {
			ImageWallService.CheckIfCacheNeeded();

			return ImageWallService.Database.ImageDetailsSet.Local.Any(img => img.Hash == imageHash);
		}

		public ImagePartUploadResult UploadImagePart(int index, byte[] imageBytes) {
			var result = new ImagePartUploadResult() {
				RecievedPart = index,
				HasError = false,
				Error = null
			};

			// Check if image details are available (if upload is initiated)
			if (!this.isReceivingImage) {
				result.HasError = true;
				result.Error = new ImageUploadException("Initiate image upload first!");
			}

			// Check if received part has any bytes and indexing is valid
			if (index < 0 || imageBytes == null || imageBytes.Length == 0) {
				result.HasError = true;
				result.Error = new IndexOutOfRangeException("Image part index or image size is out of range!");
			}

			// Check if givenpart index already exists
			if (this.imageParts.ContainsKey(index)) {
				result.HasError = true;
				result.Error = new IndexOutOfRangeException("Image part already received!");
			}

			// Add part to list
			this.imageParts.Add(index, imageBytes);
			this.bytesLeft -= imageBytes.Length;

			System.Diagnostics.Debug.WriteLine("Received Image[{0}] part {1}", this.imageDetails.Hash, index);

			// Check if all bytes received
			if (this.bytesLeft <= 0) {
				try {
					result.Error = this.FinishUpload();
					result.HasError = result.Error != null;
				}
				catch (Exception ex) {
					result.Error = ex;
					result.HasError = true;
				}
			}

			return result;
		}

		private Exception FinishUpload() {
			// Combines image parts
			List<byte> imageBytes = new List<byte>();
			this.imageParts.Keys.ToList().Sort();
			foreach (var bytes in this.imageParts.Values) {
				imageBytes.AddRange(bytes);
			}

			// Get absolute tag directory path
			string path = ImageWallService.GetServerImagesPath(imageDetails.Tags.First());

			// Create tag directory if doesn't exist
			if (!Directory.Exists(path)) {
				try {
					Directory.CreateDirectory(path);
				}
				catch (Exception ex) {
					System.Diagnostics.Debug.WriteLine(ex);
					return ex;
				}
			}

			// Add image signature to absoute path
			string imagePath = Path.Combine(path, imageDetails.Hash + imageDetails.Extension);
			string imageThumbnailPath = Path.Combine(path, imageDetails.Hash + "_Thumb.jpg");

			// Save image and it's thumbnail to disk
			this.SaveImageToDisk(imageBytes.ToArray(), imagePath);
			this.CreateImageThumbnail(imageBytes.ToArray(), imageThumbnailPath);
			
			// Check if databse cache is needed before changing or checking anything
			ImageWallService.CheckIfCacheNeeded();

			// Retrieve tags from image details or create new tags
			var user = RetrieveUser(this.imageDetails.UserId);
			var tags = RetrieveTags(this.imageDetails.Tags);

			// Set tags user
			foreach (var tag in tags) {
				tag.User = user;
				tag.Date = DateTime.Now;
			}

			// Create image details data model
			var imageDetailsModel = new Data.ImageDetails() {
				Name = this.imageDetails.Name,
				Date = this.imageDetails.Created,
				Hash = this.imageDetails.Hash,
				ImageTag = tags,
				Latitude = this.imageDetails.Latitude,
				Longitude = this.imageDetails.Longitude,
				URL = ImageWallService.ToRelativePath(imagePath),
				URLThumb = ImageWallService.ToRelativePath(imageThumbnailPath),
				User = user
			};

			// Add image details and save changes
			ImageWallService.Database.Database.Connection.Open();
			ImageWallService.Database.ImageDetailsSet.Add(imageDetailsModel);
			ImageWallService.Database.SaveChanges();
			ImageWallService.Database.Database.Connection.Close();

			System.Diagnostics.Debug.WriteLine(format: "Image[{0}] Successfully received", args: this.imageDetails.Hash);

			// Set receiving flags to end upload
			this.isReceivingImage = false;
			this.imageDetails = default(ImageDetails);
			this.bytesLeft = 0;
			this.imageParts.Clear();

			// Cache changes
			ImageWallService.CacheDatabase();

			return null;
		}

		private ICollection<ImageTag> RetrieveTags(IEnumerable<string> tags) {
			// Retrieve all tags availabel
			var tagsQuery = (from t in ImageWallService.Database.ImageTagSet.Local
			                 where tags.Contains(t.Alias)
			                 select t).ToList();

			// If not all tags are available, create rest of them
			if (tagsQuery.Count != tags.Count()) {
				// List of tags that are not available
				var newTags = tags.Where(t => !tagsQuery.Exists(tq => tq.Alias == t));

				// To list of tags create and add new tag
				tagsQuery.AddRange(newTags.Select(newTag =>
				                                  new ImageTag() {
					                                  Alias = newTag,
					                                  Date = DateTime.Now
				                                  }
					                   ));
			}

			return tagsQuery;
		}

		private User RetrieveUser(string userId) {
			// Create new user object
			var user = new Data.User() {
				IsRegistered = false,
				UserId = userId
			};

			// Match given user id with available user id's
			var userQuery = (from u in ImageWallService.Database.UserSet.Local
							 where u.UserId == this.imageDetails.UserId
							 select u).ToList();

			// Select first user with matching user id if any
			if (userQuery.Any())
				user = userQuery.First();
			else System.Diagnostics.Debug.WriteLine(format: "Created user [0]", args: user.UserId);

			return user;
		}

		private void CreateImageThumbnail(byte[] imageBytes, string thumbnailPath) {
			// Create memory stream filled with image bytes and empty memory stream for thumbnail
			MemoryStream imageMemoryStream = new MemoryStream(imageBytes.ToArray());
			MemoryStream thumbnailMemoryStream = new MemoryStream();

			// Create image thumbnail
			var image = System.Drawing.Image.FromStream(imageMemoryStream);
			var thumbnal = image.GetThumbnailImage(
				ImageWallService.DefaultMaxThumbnailWidth, 
				(int) ((image.Height / (float)image.Width) * ImageWallService.DefaultMaxThumbnailWidth), 
				() => true, IntPtr.Zero);

			// Fill memory stream with thumbnail image data
			thumbnal.Save(thumbnailMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
			thumbnailMemoryStream.Position = 0;

			// Create byte array and fill it with stream data
			byte[] thumbnailBytes = new byte[thumbnailMemoryStream.Length];
			thumbnailMemoryStream.Read(thumbnailBytes, 0, (int)thumbnailMemoryStream.Length);

			// Save thumbnail bytes to disk
			this.SaveImageToDisk(thumbnailBytes, thumbnailPath);

			// Dispose memory streams
			thumbnailMemoryStream.Close();
			thumbnailMemoryStream.Dispose();
			imageMemoryStream.Close();
			imageMemoryStream.Dispose();
		}

		private void SaveImageToDisk(byte[] bytes, string path) {
			// Write image bytes
			try {
				using (FileStream fileStream = File.Open(path, FileMode.Create, FileAccess.Write)) {
					using (BinaryWriter binaryWriter = new BinaryWriter(fileStream)) {
						binaryWriter.Write(bytes.ToArray(), 0, bytes.Length);
					}
				}
			}
			catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}

		#endregion

		#region Image Retrieving

		public IEnumerable<TagModel> GetAllTags() {
			ImageWallService.CheckIfCacheNeeded();

			return from t in ImageWallService.Database.ImageTagSet.Local
				   where t.ImageDetails.Count > 0
				   orderby t.Date descending 
			       select (TagModel)t;
		}

		public IEnumerable<string> GetNearbyTags(double latitude, double longitude, double distance) {
			ImageWallService.CheckIfCacheNeeded();

			var query = from img in ImageWallService.Database.ImageDetailsSet.Local
			            where img.Date > DateTime.Now - TimeSpan.FromDays(1)
			            where DistanceBetween(latitude, img.Latitude, longitude, img.Longitude) <= distance
						select img.ImageTag;

			return query.SelectMany(tmc => tmc.Select(tm => tm.Alias)).Distinct();
		}

		public DateTime GetTagModified(string tag) {
			ImageWallService.CheckIfCacheNeeded();

			return (from t in ImageWallService.Database.ImageTagSet.Local
			        where t.Alias == tag
			        orderby t.Date descending
			        select t.Date).FirstOrDefault();
		}

		public IEnumerable<ImageDetails> GetImagesByDate(string startDate, string endDate) {
			ImageWallService.CheckIfCacheNeeded();
			
			var query = from img in ImageWallService.Database.ImageDetailsSet.Local
						where img.Date <= new DateTime(1970, 01, 01).AddMilliseconds(long.Parse(startDate))
						where img.Date >= new DateTime(1970, 01, 01).AddMilliseconds(long.Parse(endDate))
						orderby img.Date descending
						select (ImageDetails)img;

			return query;
		}

		public IEnumerable<ImageDetails> GetImagesByAmount(int maxAmount, int skip) {
			ImageWallService.CheckIfCacheNeeded();

			var query = from img in ImageWallService.Database.ImageDetailsSet.Local
			            orderby img.Date descending
			            select (ImageDetails)img;

			return query.Skip(skip).Take(maxAmount);
		}

		public IEnumerable<ImageDetails> GetImagesByTagDate(string tag, string startDate, string endDate) {
			ImageWallService.CheckIfCacheNeeded();

			var tagsQuery = from t in ImageWallService.Database.ImageTagSet.Local
							where t.Alias == tag
							select t;
			var imageTag = tagsQuery.FirstOrDefault();
			if (imageTag != null) {
				return from img in imageTag.ImageDetails
					   where img.Date <= new DateTime(1970, 01, 01).AddMilliseconds(long.Parse(startDate))
					   where img.Date >= new DateTime(1970, 01, 01).AddMilliseconds(long.Parse(endDate))
				       orderby img.Date descending
				       select (ImageDetails) img;
			}
			return null;
		}

		public IEnumerable<ImageDetails> GetImagesByTagAmount(string tag, int maxAmount, int skip) {
			ImageWallService.CheckIfCacheNeeded();

			var tagsQuery = from t in ImageWallService.Database.ImageTagSet.Local
							where t.Alias == tag
							select t;
			var imageTag = tagsQuery.FirstOrDefault();
			if (imageTag != null) {
				return (from img in imageTag.ImageDetails
				        orderby img.Date descending
				        select (ImageDetails) img).Skip(skip).Take(maxAmount);
			}
			return null;
		}

		#endregion

		#endregion

		/// <summary>
		/// Builds path to given tag folder or to "Others" folder if no tag has been specified
		/// </summary>
		/// <param name="tag"></param>
		/// <returns></returns>
		private static string GetServerImagesPath(string tag = null) {
			return ImageWallService.ServerPath +
			       ImageWallService.ImagesDirectory +
				   tag ?? "Others";
		}


		/// <summary>
		/// Removes physical part of image address from given path
		/// </summary>
		/// <param name="path">Path from which to remove absolute part</param>
		/// <returns>Returns relative path</returns>
		private static string ToRelativePath(string path) {
			return path.Replace(ImageWallService.ServerPath, "");
		}

		/// <summary>
		/// Calculates distance between two points (lat-long coordinates) in kilometers
		/// </summary>
		/// <param name="lat1">Latitude of point 1</param>
		/// <param name="lat2">Latitude of point 2</param>
		/// <param name="lon1">Longitude of point 1</param>
		/// <param name="lon2">Longitude of point 2</param>
		/// <returns>Returns distance between points in kilometers</returns>
		/// <remarks>
		/// Source from: http://www.movable-type.co.uk/scripts/latlong.html
		/// </remarks>
		public static double DistanceBetween(double lat1, double lat2, double lon1, double lon2) {
			const double radius = 6371.0;
			
			double deltaLat = ToRadian(lat2 - lat1);
			double deltaLon = ToRadian(lon2 - lon1);
			
			lat1 = ToRadian(lat1);
			lat2 = ToRadian(lat2);

			double a = Math.Sin(deltaLat/2.0)*Math.Sin(deltaLat/2.0) +
			        Math.Sin(deltaLon/2.0)*Math.Sin(deltaLon/2.0)*Math.Cos(lat1)*Math.Cos(lat2);
			double c = 2.0*Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
			double d = radius*c;

			return d;
		}

		/// <summary>
		/// Converts degrees to radians
		/// </summary>
		/// <param name="degrees">Degrees value</param>
		/// <returns>Returns radians value of given degrees value</returns>
		public static double ToRadian(double degrees) {
			return degrees * (Math.PI / 180);
		}

		/// <summary>
		/// Converts radians to degrees
		/// </summary>
		/// <param name="radians">Radians value</param>
		/// <returns>Returns degrees value of given radians value</returns>
		public static double ToDegrees(double radians) {
			return radians * (180 / Math.PI);
		}
	}
}
