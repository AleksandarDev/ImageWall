//
// jquery.HeaderBar.js
// Creted:	12.2012.
// Author:	Aleksandar Toplek
// Version:	0.2.0
// 
// HeaderBar widget was created for ImageWall, implementation 
// can be found at http://imagewall.toplek.net/
//
// Namespace:	iw
// Alias:			HeaderBar
//

(function ($) {
	$.widget("iw.HeaderBar", {
		// Options of current widget
		options: {
			minLineWidth: 40,
			maxLineWidth: 80,
			color: "random"
		},

		// Variables used to run widget
		//_variables: {
			
		//},

		_create: function () {
			/// <summary>
			/// Builds header bar
			/// Clears element content if any before building header
			/// </summary>

			//this.destroy();
			this._BuildHeaderBar();
		},

		_setOption: function (key, value) {
			/// <summary>
			/// Sets individual option
			/// </summary>
			/// <param name="key">Property name</param>
			/// <param name="value">Value to set</param>

			switch (key) {
				case "minLineWidth":
					this.options.minLineWidth = value;
					this._create();
					break;
				case "maxLineWidth":
					this.options.maxLineWidth = value;
					this._create();
					break;
				case "color":
					this.options.color = value;
					this._create();
					break;
				case "clear":
					this.options.minLineWidth = 40;
					this.options.maxLineWidth = 80;
					this.options.color = "random";
					this._create();
					break;
			}
			$.Widget.prototype._setOption.apply(this, arguments);
		},

		destroy: function () {
			/// <summary>
			/// Removes element content
			/// </summary>

			this.element.remove($("div", this));

			$.Widget.prototype.destroy.call(this);
		},

		_BuildHeaderBar: function () {
			/// <summary>
			/// Builds element content
			/// </summary>

			var self = this,
			    options = self.options,
			    element = self.element;

			// Set header bar class
			element.addClass("iw-HeaderBar");

			// Set some variables used to generate lines
			var barWidth = element.width();
			var lineWidthDifference = options.maxLineWidth - options.minLineWidth;
			var barLinesWidthCurrent = 0;

			var line;
			var lineWidth;
			while (barLinesWidthCurrent < barWidth) {
				// Calculate new random line width and add it to sum
				lineWidth = Math.round(Math.random() * lineWidthDifference) + options.minLineWidth;
				barLinesWidthCurrent += lineWidth;

				// Check if this is last line
				if (barLinesWidthCurrent >= barWidth) {
					lineWidth = barWidth - (barLinesWidthCurrent - lineWidth);
					barLinesWidthCurrent = barWidth;
				}

				// Create new bar
				line = $("<div>");
				line.css("background", self._GetColor());
				line.css("width", lineWidth);
				element.append(line);
			}
		},

		_GetColor: function () {
			/// <summary>
			/// Gets color for header line
			/// Color depends on options.color property
			/// Use 'random' in options.color to get random color and '#******' to get specific color
			/// NOTE using random color generator from http://stackoverflow.com/questions/1484506/random-color-generator-in-javascript
			/// </summary>
			/// <returns type="String">Returns color string for header line</returns>

			return this.options.color == "random" ?
				'#' + ((1 << 24) * (Math.random() + 1) | 0).toString(16).substr(1) :
				this.options.color;
		}
	});
}(jQuery));