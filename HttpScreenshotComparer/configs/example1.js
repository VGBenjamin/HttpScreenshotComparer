var system = require('system');
var imageTarget = system.args["imageTarget"];

var page = require('webpage').create();

page.viewportSize = {
  width: 1600,
  height: 8000
};

page.settings = { loadImages: true, javascriptEnabled: true};
page.settings.userAgent = 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36';                           

page.onConsoleMessage = function(msg, lineNum, sourceId) {
  console.log('CONSOLE: ' + msg + ' (from line #' + lineNum + ' in "' + sourceId + '")');
};


page.open('https://www.ma.cuisinella/fr-fr');
page.onLoadFinished = function(status) {
	  setTimeout(function () {
		page.render(imageTarget);
		phantom.exit();
	}, 2000);   
};