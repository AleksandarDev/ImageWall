//
// Application.js
// Creted:	12.2012.
// Author:	Aleksandar Toplek
//

function Application() {
	$(window).resize(function() {
		CheckPageSize();
	});

	var AnimationHoverSizeChange = 10;
	var FlipAnimationT = 400;
	var HoverAnimationT = 300;
	var isTagCloudLoaded = false;
	var isImageWallLoaded = false;

	var self = this;
	this.applicationSettings = new ApplicationSettings();
	this.serviceProxy = undefined;
	this.imagesOnPage = new Array();

	this.Initialize = function() {
		/// <summary>
		/// Initializes Application object
		/// </summary>

		// Declare variables
		self.applicationSettings.ServerOrigin = window.location.origin;
		if (!self.applicationSettings.ServerOrigin)
			self.applicationSettings.ServerOrigin =
				window.location.protocol + "//" + window.location.host;

		self.serviceProxy = new ServiceProxy(self.applicationSettings.ServerOrigin + self.applicationSettings.ServicePath);

		console.log(self.applicationSettings.ServerOrigin + self.applicationSettings.ServicePath);
		// Process current location query
		var parameters = ReadLocationQuery();
		ProcessParameters(parameters);

		// Create spinner to indicate that page is loading
		$(".Spinner").spinner();

		// Apply page settings
		if (self.applicationSettings.IsEmbeded) {
			$("head").append($("<link>").attr("href", "css/ImageWallEmbedStyle.css").attr("type", "text/css").attr("rel", "stylesheet"));
			$("head").append($("<link>").attr("href", "css/PageEmbedStyle.css").attr("type", "text/css").attr("rel", "stylesheet"));
		} else {
			$("head").append($("<link>").attr("href", "css/ImageWallStyle.css").attr("type", "text/css").attr("rel", "stylesheet"));
			$("head").append($("<link>").attr("href", "css/PageStyle.css").attr("type", "text/css").attr("rel", "stylesheet"));
		}

		// Check page size for resize
		CheckPageSize();

		// Show ImageWall
		BuildImageWall();
	};

	var ReadLocationQuery = function() {
		/// <summary>
		/// Reads current location query
		/// </summary>
		/// <returns type="Array">Returns array of parametes from query request</returns>

		var parameters = new Array();

		// Get current location query an remove leading questionmark
		var query = window.location.search.replace('?', "");

		// Split query by '&' symbol
		var hashes = query.split('&');

		// Go through all parameters and add each to array
		for (var index = 0; index < hashes.length; index++) {
			var hash = hashes[index].split('=');
			parameters[hash[0]] = hash[1];
		}

		return parameters;
	};

	var ProcessParameters = function(parameters) {
		/// <summary>
		/// Processes parameters passed and saves them to application settings
		/// </summary>
		/// <param name="parameters">Parameters to process</param>

		// Tag option
		if (parameters["Tag"] !== undefined) {
			self.applicationSettings.IsTagSpecified = true;
			self.applicationSettings.ImageWallTag = parameters["Tag"];
		}

		// Embed option
		if (parameters["Embed"] !== undefined) {
			if (parameters["Embed"] == "wall") {
				self.applicationSettings.IsEmbeded = true;
				self.applicationSettings.EmbedShowImageWall = true;
			} else if (parameters["Embed"] == "tags") {
				self.applicationSettings.IsEmbeded = true;
				self.applicationSettings.EmbedShowTagCloud = true;
			} else if (parameters["Embed"] == "both") {
				self.applicationSettings.IsEmbeded = true;
				self.applicationSettings.EmbedShowImageWall = true;
				self.applicationSettings.EmbedShowTagCloud = true;
			} else self.applicationSettings.IsEmbeded = false;
		}

		// Handle embed parameters
		if (self.applicationSettings.IsEmbeded) {
			if (parameters["SizeW"] !== undefined) {
				self.applicationSettings.ImageWallContentWidth = parseInt(parameters["SizeW"], 10);
			}
			if (parameters["Chrome"] !== undefined) {
				self.applicationSettings.ImageWallImagesBorder = parseInt(parameters["Chrome"], 10);
			}
			if (parameters["Spacing"] !== undefined) {
				self.applicationSettings.ImageWallImagesSpacing = parseInt(parameters["Spacing"], 10);
			}
		}

		// TODO Implement embed options
	};

	var BuildTagCloud = function() {
		/// <summary>
		/// Builds tag cloud from tags returned from REST API
		/// Bolds tag if selected and adds link with query for each tag
		/// </summary>

		// Dont show tag cloud if not enabled (embed-nocloud)
		if (self.applicationSettings.IsEmbeded && !self.applicationSettings.EmbedShowTagCloud) {
			self.applicationSettings.IsTagCloudLoaded = true;
			$(".TagsCloud").css("display", "none");
			CheckIfAllLoaded();
			return;
		}

		self.serviceProxy.Invoke("GetAllTags", "GET", function(res) {
			if (!res || res.length === 0) return;

			// Find min and max tag size
			var minTagSize = res[0].Size;
			var maxTagSize = res[0].Size;
			for (var index = 0; index < res.length; index++) {
				if (res[index].Size < minTagSize) minTagSize = res[index].Size;
				if (res[index].Size > maxTagSize) maxTagSize = res[index].Size;
			}

			// Calculate tag size step (max 2)
			var step = 2.5 / (maxTagSize / minTagSize);

			// Clear tags list
			$(".TagsCloud").html("");

			// Create tags
			for (var index = 0; index < res.length; index++) {
				var tagElement = $("<a>");
				tagElement.attr("href", window.location.pathname + "?Tag=" + res[index].Alias);
				tagElement.attr("class", "TagElement");
				tagElement.append("#" + res[index].Alias);

				// Chenge tag element size based on number of pictures in tag
				var size = res[index].Size - minTagSize;
				tagElement.css("font-size", (0.75 + size * step).toString() + "em");

				// Check if current tag element is selected
				if (self.applicationSettings.IsTagSpecified &&
					res[index].Alias == self.applicationSettings.ImageWallTag) {
					tagElement.css("font-weight", "bold");
				}

				$(".TagsCloud").append(tagElement);

				self.applicationSettings.IsTagCloudLoaded = true;
				CheckIfAllLoaded();
			}
		});
	};

	var CheckIfAllLoaded = function() {
		/// <summary>
		/// Checks if all on page is loaded, if so, do some last lines of code
		/// </summary>

		if (self.applicationSettings.IsTagCloudLoaded && self.applicationSettings.IsImageWallLoaded) {
			$(".Spinner").remove();
		}
	};

	var BuildImageWall = function() {
		/// <summary>
		/// Builds image wall from images returned from REST API
		/// </summary>

		if (self.applicationSettings.IsEmbeded && !self.applicationSettings.EmbedShowImageWall) {
			self.applicationSettings.IsImageWallLoaded = true;
			$(".ImagesHolderRoot").css("display", "none");
			BuildTagCloud();
			CheckIfAllLoaded();
			return;
		}

		CheckIfNewImagesAvailable();

		// Set interval to check for new images
		setInterval(CheckIfNewImagesAvailable, 5000);
	};

	var GetImageWallRequestCall = function () {
		/// <summary>
		/// Builds request call depending upon current application settings
		/// </summary>
		/// <returns type="string">Returns string representing complete REST call</returns>

		if (self.applicationSettings.IsTagSpecified)
			return self.serviceProxy.BuildURLCall("GetImagesTagAmount", {
				Tag: self.applicationSettings.ImageWallTag,
				Amount: 50,
				Skip: 0
			});
		else {
			var currentTime = new Date();
			var currentTimeUTC = Date.UTC(
				currentTime.getYear() + 1900, currentTime.getMonth(), currentTime.getDate(),
				currentTime.getHours(), currentTime.getMinutes(), currentTime.getSeconds(), currentTime.getMilliseconds());

			return self.serviceProxy.BuildURLCall("GetImagesDate", {
				DateStart: currentTimeUTC.toString(),
				DateEnd: (currentTimeUTC - 86400000).toString()
			});
		}
	};

	var HandleNewImages = function (res) {
		/// <summary>
		/// Handles new images, inserts them to page and reorganizes page
		/// </summary>
		/// <param name="res">Array of new images</param>

		if (res === null || !res || res.length == 0) return;

		// Insert all images from response
		for (var index = res.length - 1; index >= 0; index--) {
			InsertImage(res[index]);
		}

		// Update tags
		BuildTagCloud();

		// When all images are added to  wall, wait for them to load
		ImageWallBuilt();
	};

	var CheckIfNewImagesAvailable = function () {
		/// <summary>
		/// Checks if there are any new images available for current page
		/// </summary>

		console.log("Checking if new images are available");

		self.serviceProxy.Invoke(
			GetImageWallRequestCall(),
			"GET",
			function(response) {
				console.log("Got response (" + response.length + ")");

				// Get only new images
				var newImages = GetOnlyNewImages(response);

				// Add all new images to list of images available on page
				jQuery.each(newImages, function() {
					self.imagesOnPage[self.imagesOnPage.length] = this.Hash;
				});

				HandleNewImages(newImages);
			});
	};

	var GetOnlyNewImages = function (images) {
		/// <summary>
		/// Filters images to only new images (images not already on page)
		/// </summary>
		/// <param name="images">Array of all images</param>
		/// <returns type="Array">Returns array that contains only images that are not present on page</returns>

		console.log("Matching images...");

		var newImages = new Array();
		for (var index = 0; index < images.length; index++) {
			if (jQuery.inArray(images[index].Hash, self.imagesOnPage) < 0)
				newImages[newImages.length] = images[index];
		}

		console.log("Matched " + newImages.length + " as new");
		return newImages;
	};

	var ImageWallBuilt = function() {
		/// <summary>
		/// Call this when all images are added to wall, this will wait
		/// for all images to load and then show the wall
		/// </summary>

		// Wait for images to load before organizing and showing them
		$(".ImagesHolderRoot").imagesLoaded(function() {
			OrganizeWall();
			$(".ImagesHolderRoot").css("visibility", "visible");

			// Apply animations to all images if not embed
			if (!self.applicationSettings.IsEmbeded ||
				(self.applicationSettings.IsEmbeded && self.applicationSettings.IsEmbededAnimations))
				ApplyAnimationsToImages();


			// Set image wall loaded and check if page is loaded
			self.applicationSettings.IsImageWallLoaded = true;
			CheckIfAllLoaded();
		});
	};

	var OrganizeWall = function() {
		/// <summary>
		/// Organizes image wall to values from application settings
		/// </summary>
		/// <returns type="object">Object that was organized (images container)</returns>

		var object = $(".ImagesHolderRoot").BlocksIt({
			numOfCol: self.applicationSettings.ImageWallColumns,
			offsetX: self.applicationSettings.ImageWallImagesSpacing,
			offsetY: self.applicationSettings.ImageWallImagesSpacing
		});

		$(".ImagesHolderRoot .ImageThumbnailHolder").each(function() {
			SaveAnimationProperties($(this));
		});

		return object;
	};

	var UpdatePageWidth = function() {
		/// <summary>
		/// Sets body size to its content size + 30 if not embed
		/// Content size is pulled from ApplicationSettings.ImageWallContentWidth
		/// </summary>

		var contentWidth = self.applicationSettings.ImageWallContentWidth;
		if (self.applicationSettings.IsEmbeded)
			$("body").css("width", (contentWidth).toString());
		else $("body").css("width", (contentWidth + 30).toString());
	};

	var InsertImage = function (imageDetails) {
		/// <summary>
		/// Inserts image built from given detals to the begining of images list
		/// </summary>
		/// <param name="imageDetails">Image details from which to construct image</param>

		// Image element
		var image = $("<img>");
		image.attr("class", "ImageThumbnail");
		image.attr("src", self.applicationSettings.ServerOrigin + imageDetails.UrlThumbnail.replace("\\", "/"));

		// Details element
		var details = $("<div>");
		details.attr("class", "ImageDetails");
		details.append("<div><b>Photo by</b></div>");
		details.append("<div style='font-size: " + Math.min(1, 1 - (0.3 / 10) * (imageDetails.UserId.length - 17)) + "em'>" + imageDetails.UserId + "</div>");
		var date = fromMSJSON(imageDetails.Created);
		details.append("<div><b>Created on</b></div>");
		details.append($("<div>").append(date.getDate() + "." + date.getMonth() + "." + date.getYear()));
		details.append("<div><b>Tags</b></div>");
		var tags = $("<div>").css("font-size", "0.7em");
		jQuery.each(imageDetails.Tags, function () {
			tags.append($("<div>").css("float", "left").append($("<a>").css("text-decoration", "none").attr("href", window.location.pathname + "?Tag=" + this).append(this + "&nbsp;&nbsp;&nbsp;")));
		});
		details.append(tags);
		details.append($("<a>").attr("href", imageDetails.Url).attr("target", "_blank").attr("class", "DownloadButton").append($("<img>").attr("src", "css/Images/Download.png")));

		// Image corner
		var corner = $("<div>");
		var cornerImage = $("<img>");
		cornerImage.attr("class", "ImageThumbnailHolderCorner");
		cornerImage.attr("src", "css/Images/Corner.png");
		if (self.applicationSettings.IsEmbeded)
			cornerImage.css("display", "none");
		corner.append(cornerImage);

		// Image container
		var imageHolder = $("<div>");
		imageHolder.attr("class", "ImageThumbnailHolder");
		imageHolder.append(image);
		imageHolder.append(details);
		imageHolder.append(corner);
		imageHolder.css("padding", self.applicationSettings.ImageWallImagesBorder);
		if (self.applicationSettings.ImageWallImagesSpacing == 0) {
			imageHolder.css("border-style", "none");
			imageHolder.css("box-shadow", "0 0 0 black");
		}

		// Add image to images holder root
		$(".ImagesHolderRoot").prepend(imageHolder);
	};

	var ApplyAnimationsToImages = function() {
		$(".ImagesHolderRoot .ImageThumbnailHolder").each(function() {
			var holder = $(this);

			ApplyHoverAnimation(holder);
			ApplyClickAnimation(holder);
		});
	};

	var ApplyHoverAnimation = function (holder) {
		/// <summary>
		/// Creates event handler for holder hover
		/// </summary>
		/// <param name="holder">Object for which to create event handling</param>

		holder.hover(function () { ImageHolderHoverOut(holder); }, function() { ImageHolderHoverDown(holder); });
	};

	var ImageHolderHoverOut = function (holder) {
		/// <summary>
		/// Animates given object so it appears to be hovering
		/// </summary>
		/// <param name="holder">Object to animate</param>

		// Fix - Image wasn't scaling according to parent, so we have to set fix height
		var newHeight = parseInt(holder.attr("data-height"), 10) + parseInt(holder.attr("data-hoverChange"), 10);
		$(".ImageThumbnail", holder).stop(false, true).animate({ height: newHeight }, { duration: HoverAnimationT / 2 });

		// Cancel flipback if already hovered out once
		if (holder.attr("data-timeoutid") !== undefined) {
			clearTimeout(holder.attr("data-timeoutid"));
			holder.removeAttr("data-timeoutid");
		}

		holder.stop(false, true);
		AnimateHoverOut(
			holder,
			parseInt(holder.attr("data-width"), 10),
			parseInt(holder.attr("data-height"), 10),
			parseInt(holder.attr("data-hoverChange"), 10),
			HoverAnimationT / 2);
	};

	var ImageHolderHoverDown = function (holder) {
		/// <summary>
		/// Animates given object so it appears to hover down 
		/// </summary>
		/// <param name="holder">Object to animate</param>

		// Fix - Image wasn't scaling according to parent, so we have to set fix height
		$(".ImageThumbnail", holder).stop(false, true).animate({ height: parseInt(holder.attr("data-height"), 10) }, { duration: HoverAnimationT / 3 });

		holder.stop(false, true);
		AnimateHoverDown(
			holder,
			parseInt(holder.attr("data-width"), 10),
			parseInt(holder.attr("data-height"), 10),
			HoverAnimationT / 3);

		var timeoutId = setTimeout(function () { ImageHolderFlipToImage(holder); }, 3000);
		holder.attr("data-timeoutid", timeoutId);
	};

	var ApplyClickAnimation = function (obj) {
		/// <summary>
		/// Creates event handler for holder click
		/// </summary>
		/// <param name="obj">Object for which to create event handler</param>

		$(obj).click(function() {
			ToggleHolderDetailsImage(obj);
		});
	};

	var ToggleHolderDetailsImage = function (obj) {
		/// <summary>
		/// Toggles between image and details view
		/// </summary>
		/// <param name="obj">Object for which to do toggling</param>

		var holder = $(obj);

		ImageHolderFlipToImage(holder);
		ImageHolderFlipToDetails(holder);
	};

	var ImageHolderFlipToImage = function (holder) {
		/// <summary>
		/// Flips image holder to show image
		/// </summary>
		/// <param name="holder">Which image holder to flip</param>

		if (holder.attr("data-flipped") == "image") return;

		var details = $(".ImageDetails", holder);
		var image = $(".ImageThumbnail", holder);

		// Hode details and show image
		holder.css("opacity", 0);
		details.css("display", "none");
		image.css("display", "block");

		holder.stop(false, true);
		AnimateFadeOut(holder, FlipAnimationT, function () {
			holder.attr("data-flipped", "image");
		});
	};

	var ImageHolderFlipToDetails = function (holder) {
		/// <summary>
		/// Flips image holder to show details
		/// </summary>
		/// <param name="holder"></param>

		if (holder.attr("data-flipped") != "image") return;

		var details = $(".ImageDetails", holder);
		var image = $(".ImageThumbnail", holder);

		// Hode image and show details
		holder.css("opacity", 0);
		details.css("display", "block");
		image.css("display", "none");

		holder.stop(false, true);
		AnimateFadeOut(holder, FlipAnimationT, function () {
			holder.attr("data-flipped", "details");
		});
	};


	var SaveAnimationProperties = function(holder) {
		/// <summary>
		/// Saves initial values if not already saved
		/// </summary>
		/// <param name="holder">jQuery object who's properties to save</param>

		// Clear any previously owned properties
		ClearAnimationProperties(holder);
		
		// Save current properties
		holder.attr("data-width", holder.width());
		holder.attr("data-height", holder.height());
		holder.attr("data-hoverChange", AnimationHoverSizeChange);
		if (holder.attr("data-flipped") === undefined)
			holder.attr("data-flipped", "image");

		console.log("Object properties saved!");
	};

	var ClearAnimationProperties = function (holder) {
		/// <summary>
		/// Crears some data attributes from given holder
		/// </summary>
		/// <param name="holder">Object from which to delete some data attributes</param>

		holder.removeAttr("data-width");
		holder.removeAttr("data-height");
		holder.removeAttr("data-hoverChange");
	};

	var AnimateHoverOut = function(object, width, height, sizeChange, time, onComplete) {
		/// <summary>
		/// Animates given object so that it rises (width and height change plus shadow)
		///  </summary>
		/// <param name="object">jQuery object to animate</param>
		/// <param name="width">Width of object</param>
		/// <param name="height">Height of object</param>
		/// <param name="sizeChange">Enlarge width and height by this value</param>
		/// <param name="time">Duration of animation in ms</param>
		/// <param name="onCompleted">Callback function on animation completed</param>
		/// <returns type="object">Returns animated object</returns>

		//object.stop(false, true);
		return object.animate({
				"boxShadow": "0 1px 8px rgba(34,25,25,0.4)",
				width: width + sizeChange,
				height: height + sizeChange,
				marginLeft: -sizeChange / 2,
				marginTop: -sizeChange / 2
			}, {
				duration: time,
				complete: onComplete
			});
	};
	var AnimateHoverDown = function(object, width, height, time, onComplete) {
		/// <summary>
		/// Animates given object so that is lovers (width and height change plus shadow)
		/// </summary>
		/// <param name="object">jQuery object to animate</param>
		/// <param name="width">Width to set to the object</param>
		/// <param name="height">Height to set to the object</param>
		/// <param name="time">Duration of animation in ms</param>
		/// <param name="onCompleted">Callback function on animation completed</param>
		/// <returns type="object">Returns animated object</returns>

		//object.stop(false, true);
		return object.animate({
				"boxShadow": "0 1px 5px rgba(34,25,25,0.4)",
				width: width,
				height: height,
				marginLeft: 0,
				marginTop: 0,
				opacity: 1
			}, {
				duration: time,
				complete: onComplete
			});
	};
	var AnimateFadeIn = function(object, time, onComplete) {
		/// <summary>
		/// Animates given object by setting opacity to zero
		/// </summary>
		/// <param name="object">jQuery object to animate</param>
		/// <param name="time">Duration of animation in ms</param>
		/// <param name="onCompleted">Callback function on animation completed</param>
		/// <returns type="object">Returns animated object</returns>

		object.stop(false, true);
		return object.animate({
				opacity: 0
			}, {
				duration: time,
				complete: onComplete
			});
	};
	var AnimateFadeOut = function(object, time, onComplete) {
		/// <summary>
		/// Animates given object by setting opacity to one
		/// </summary>
		/// <param name="object">jQuery object to animate</param>
		/// <param name="time">Duration of animation in ms</param>
		/// <param name="onCompleted">Callback function on animation completed</param>
		/// <returns type="object">Returns animated object</returns>

		object.stop(false, true);
		return object.animate({
				opacity: 1
			}, {
				duration: time,
				complete: onComplete
			});
	};

	var CheckPageSize = function() {
		/// <summary>
		/// Called upon window resize
		/// Calculates needed content width and columns needed to show all images so that they fit
		/// Saves values to ApplicationSettings.ImageWallContentWidth and ApplicationSettings.ImageWallColumns
		/// </summary>

		if (!self.applicationSettings.IsEmbeded) {
			var windowWidth = $(window).width();

			// 220 = 200 for image width and 20 for images spacing
			var newImagesColumns = Math.floor(windowWidth / 220);

			if (newImagesColumns != self.applicationSettings.ImageWallColumns) {
				self.applicationSettings.ImageWallContentWidth = newImagesColumns * 220;
				self.applicationSettings.ImageWallColumns = newImagesColumns;
				
				// Apply changes
				UpdatePageWidth();
				OrganizeWall();
			}
		}
		
		// Updates page width
		UpdatePageWidth();
	};
}