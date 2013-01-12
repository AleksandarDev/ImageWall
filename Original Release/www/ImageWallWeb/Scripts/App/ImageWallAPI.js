//
// Application.js
// Creted:	12.2012.
// Author:	Aleksandar Toplek
//

function ImageWallAPI() {
	function ShowTagImages(tag) {
		var restCall = BuildURLCall(
			"GetImageThumbnailLinkTagAmount",
			{
				Tag: tag,
				Amount: 50,
				Skip: 0
			}); //getImagesThumbnailTagAmount + tag + getImagesThumbnailTagPlusAmount;
		ImageWallRESTGetCall(restCall, function (res) {
			for (var index = 0; index < res.length; index++) {
				InsertImage("", restServiceRootURI + res[index]);
			}

			ImageWallBuilt();
		});
	}
}