//
// Application.js
// Creted:	12.2012.
// Author:	Aleksandar Toplek
//

function ServiceProxy(serviceUrl) {
/// <summary>
/// Service proxy class that has some useful methods for sending AJAX requests to given service
/// </summary>
/// <param name="serviceUrl">Url of service</param>

	var self = this;
	this.ServiceUrl = serviceUrl;

	this.Invoke = function (request, method, callback, data, bare, error) {
		/// <summary>
		/// Invokes AJAX request call with given parameters
		/// </summary>
		/// <param name="request">Request that is added to the end of service url</param>
		/// <param name="method">Method used to send request (GET, POST, ...)</param>
		/// <param name="callback">Callback function with result</param>
		/// <param name="data">Data to include in request</param>
		/// <param name="bare">If this is set to true, result will not be processed, only parsed</param>
		/// <param name="error">On error, this function is called</param>

		// Convert data to JSON
		var jsonData = undefined;
		if (data) jsonData = JSON.stringify(data);

		// Build url
		var url = self.ServiceUrl + request;
		console.log(url);

		// Send ajax request
		$.ajax({
			url: url,
			data: jsonData,
			type: method,
			processData: false,
			contentType: "application/json",
			timeout: 10000,
			dataType: "text",
			success: function (res) {
				if (!callback) return;
				
				var result = JSON.parse(res);
				
				// If bare result requested
				if (bare || !result || result.length)
				{ callback(result); return; }

				// Wrapped message contains top level object node, strip it off
				for (var property in result) {
					callback(result[property]);
					break;
				}
			},
			error: function (xhr) {
				console.error(xhr);
				if (!error) return;
				if (xhr.responseText) {
					var err = json.parse(xhr.responseText);
					if (err) error(err);
					else error({ Message: "Unknown server error." });
				}
			}
		});
	};

	this.BuildURLCall = function(method) {
		/// <summary>
		/// Builds Url for service proxy call
		/// Pass other arguments after method in object
		/// eg. {Type: "Sample", Embed: "Full"} will result in "Mathod?Tag=Sample&Embed=Full"
		/// </summary>
		/// <param name="method">Method of request</param>
		/// <returns type="String">Returns query url for given parameters</returns>

		var urlCall = method;

		if (arguments.length > 1)
			urlCall += "?";

		var propertyIndex = 0;
		for (var i in arguments[1]) {
			if (propertyIndex != 0) urlCall += "&";
			urlCall += i + "=" + arguments[1][i];
			propertyIndex++;
		}

		return urlCall;
	};
}