var system = require('system');
var args = system.args;
var userAgent = 'sgPhantom';
//var userAgent = 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36';;

var page = require('webpage').create();

page.settings.userAgent = userAgent;
page.viewportSize = {
    width: 1650,
    height: 8000
};
page.open('https://cms-preprod.ma.cuisinella/fr-fr');
//page.open('http://localhost:60525/fr-fr?sc_site=cuisinellatest');
//page.open('https://www.ma.cuisinella');
//page.open('https://www.ma.cuisinella');
//page.open('https://www.home-design.schmidt/fr-fr/cuisines-equipees');
//page.open('https://github.com/');


setTimeout(function () {
    
    page.render('D:\\temp\\cla-20180719-0214\\debug.jpg');

    console.log("Screenshot done!");
    setTimeout(function () {
        phantom.exit();
    }, 30);
}, 10000);



