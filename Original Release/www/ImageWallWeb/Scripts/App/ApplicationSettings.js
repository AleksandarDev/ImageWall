//
// Application.js
// Creted:	12.2012.
// Author:	Aleksandar Toplek
//

function ApplicationSettings() {
	// Aplication settings
	this.ServerOrigin = "";
	//this.ServerImagesPath = "/ImageWallService/";
	//this.ServerImagesPath = "/Images/";
	//this.ServicePath = "/ImageWallService/ImageWallService.svc/rest/";
	this.ServicePath = "/ImageWallService.svc/rest/";

	// Default options
	this.ImageWallColumns = 5;
	this.ImageWallContentWidth = 1100;
	this.ImageWallImagesSpacing = 8;
	this.ImageWallImagesBorder = 15;

	// Embed options
	this.IsEmbeded = false;
	this.EmbedShowTagCloud = false;
	this.EmbedShowImageWall = false;
	this.IsEmbededAnimations = false;

	// Application options
	this.IsTagSpecified = false;
	this.ImageWallTag = undefined;
	
	// State variables
	this.IsTagCloudLoaded = false;
	this.IsImageWallLoaded = false;
}