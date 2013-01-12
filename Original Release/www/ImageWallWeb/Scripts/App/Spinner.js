//
// Application.js
// Creted:	12.2012.
// Author:	Aleksandar Toplek
//

(function ($) {
	$.widget("iw.spinner", {
		options: {
			numberOfBars: 12,
			color: "random"
		},

		_create: function() {
			var self = this,
			    options = self.options,
			    element = self.element;

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

			// Set interval timer to update colors of spinner
			// NOTE using random color generator from http://stackoverflow.com/questions/1484506/random-color-generator-in-javascript
			var spinnerTimerID = setInterval(function() {
				$("div", element).each(function() {
					if ($(this).css("opacity") >= 0.98) {
						$(this).css("background", self._GetColor());
						return false;
					}
				});
			}, spinnerBarAnimationDelay);

			// Save timer id so that it can be stopped when page loaded
			element.attr("data-timerID", spinnerTimerID);
		},

		_setOption: function(key, value) {
			switch (key) {
			case "clear":
				// TODO Clear options
				console.warn("setOption not supported!");
				break;
			}
			$.Widget.prototype._setOption.apply(this, arguments);
		},
		destroy: function() {
			clearInterval(this.element.attr("data-timerID"));
			this.element.remove($("div", this));

			$.Widget.prototype.destroy.call(this);
		},

		_GetColor: function () {
			return this.options.color == "random" ?
				'#' + (Math.random() * 0xFFFFFF << 0).toString(16) :
				this.options.color;
		}
});
}(jQuery));