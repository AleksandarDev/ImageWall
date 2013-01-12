//
// Application.js
// Creted:	12.2012.
// Author:	Aleksandar Toplek
//

Date.prototype.toMSJSON = function () {
	/// <summary>
	/// Converts JS Date to Microsft JSON Date
	/// Source: http://www.west-wind.com/weblog/posts/2009/Sep/15/Making-jQuery-calls-to-WCFASMX-with-a-ServiceProxy-Client
	/// </summary>
	/// <returns type="">Returns JSON String for MS Date used in WCF Services</returns>

	return '\\/Date(' + this.getTime() + ')\\/';
};

function fromMSJSON(value) {
	/// <summary>
	/// Converts JSON WCF Date to native JS date object
	/// Source: http://www.west-wind.com/weblog/posts/2009/Sep/15/Making-jQuery-calls-to-WCFASMX-with-a-ServiceProxy-Client
	/// </summary>
	/// <param name="value">Valut to convert</param>
	/// <returns type="">Returns JS Date object</returns>

	var b = value.split(/[()-+,.]/);
	return new Date(b[1] ? +b[1] : 1 - +b[2]);
};