//
// jquery.Spinner.js
// Creted:	12.2012.
// Author:	Aleksandar Toplek
// Version:	0.2.1
// 
// Spinner widget was created for ImageWall, implementation 
// can be found at http://imagewall.toplek.net/
//
// Namespace:	iw
// Alias:			Spinner
//

(function ($) {
	$.widget("iw.Spinner", {
		// Options of current widget
		options: {
			numberOfBars: 12,
			color: "random"
		},

		// Variables used to run widget
		_variables: {
			intervalTimerID: -1
		},

		_create: function () {
			/// <summary>
			/// Builds spinner bar
			/// Clears element content if any before building spinner
			/// </summary>

			//this.destroy();
			this._BuildSpinner();
		},

		_setOption: function (key, value) {
			/// <summary>
			/// Sets individual option
			/// </summary>
			/// <param name="key">Property name</param>
			/// <param name="value">Value to set</param>

			switch (key) {
				case "numberOfBars":
					this.options.numberOfBars = value;
					this._create();
					break;
				case "color":
					this.options.color = value;
					this._create();
					break;
				case "clear":
					this.options.color = "random";
					this.options.numberOfBars = 12;
					this._create();
					break;
			}
			$.Widget.prototype._setOption.apply(this, arguments);
		},

		destroy: function () {
			/// <summary>
			/// Removes element content and stops any timers if used
			/// </summary>

			clearInterval(this._variables.intervalTimerID);
			this.element.remove($("div", this));

			$.Widget.prototype.destroy.call(this);
		},

		_BuildSpinner: function () {
			/// <summary>
			/// Builds element content and sets interval for update if necessary
			/// </summary>

			var self = this,
			    options = self.options,
			    element = self.element;

			// Sets spinner class
			element.addClass("iw-Spinner");

			// Calculate needed values and get jQuery object of root element
			var barRotationDeg = 360 / options.numberOfBars;
			var spinnerBarAnimationDelay = 1 / options.numberOfBars;

			// Add bars to spinner element
			for (var index = 0; index < options.numberOfBars; index++) {
				var bar = $("<div>");
				bar.css({
					"transform": "rotate(" + barRotationDeg * index + "deg) translate(0, -142%)",
					"animation": "fade 1s ease-out infinite",
					"animation-delay": (1 - spinnerBarAnimationDelay * (index + 1)) + "s",
					"background": self._GetColor()
				});
				element.append(bar);
			}

			// Sets interval for updating color of bars if color is random
			if (options.color == "random") {
				// Set interval timer to update colors of spinner
				var spinnerTimerID = setInterval(function () {
					$("div", element).each(function () {
						if ($(this).css("opacity") >= 0.98) {
							$(this).css("background", self._GetColor());
							return false;
						}
					});
				}, spinnerBarAnimationDelay);

				// Save timer id so that it can be stopped when page loaded
				this._variables.intervalTimerID = spinnerTimerID;
			}
		},

		_GetColor: function () {
			/// <summary>
			/// Gets color for spinner bar
			/// Color depends on options.color property
			/// Use 'random' in options.color to get random color and '#******' to get specific color
			/// NOTE using random color generator from http://stackoverflow.com/questions/1484506/random-color-generator-in-javascript
			/// </summary>
			/// <returns type="String">Returns color string for spinner bars</returns>

			return this.options.color == "random" ?
				'#' + ((1 << 24) * (Math.random() + 1) | 0).toString(16).substr(1) :
				this.options.color;
		}
	});
}(jQuery));