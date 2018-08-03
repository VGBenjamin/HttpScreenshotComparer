var system = require('system');
var args = system.args;
var userAgent = 'sgPhantom';
//var userAgent = 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36';;

phantom.onError = function (msg, trace) {
    console.log('ERROR : phantom.onError:' + msg + "\nTrace: " + trace);
};

if (args.length !== 5) {
    console.log("ERROR : Incorrect number of arguments expected 4 but " + args.length + "found");
} else {
    var params = {};
    params.targetPath = args[1];
    params.urlName = args[2];
    params.url = args[3];
    params.screenWidth = args[4];    
    params.waitTimeBetweenTwoCheckStatus = 300;
    params.maxTotalWaitTime = 6000;
    params.jsTimeToComplete = 500;

    var numberOfRessourcesToLoad = 0;
    var waitingTimeSpent = 0;

    console.log("DEBUG : executing the phantomjs with the parameters:\n" + JSON.stringify(params, null, "    "));


    var page = require('webpage').create();

    page.onConsoleMessage = function (msg, lineNum, sourceId) {
        console.log('ERRROR : page.onConsoleMessage: ' + msg + ' (from line #' + lineNum + ' in "' + sourceId + '")');
        //system.stderr.writeLine('console: ' + msg);
    };

    // Setup the capture options
    page.viewportSize = {
        width: params.screenWidth,
        height: 8000
    };

    page.settings = { loadImages: true, javascriptEnabled: true };
    page.settings.userAgent = userAgent;

    // Handle the errors & console messages
    page.onError = function (msg, trace) {
        console.log('ERROR : page.onError: ' + msg);
    };

    page.onResourceError = function (resourceError) {
        console.log('ERROR : page.resourceError:' + JSON.stringify(resourceError));

        numberOfRessourcesToLoad -= 1;
    };    

    // Wait for all ressources to be loaded
    page.onResourceRequested = function (req) {
        numberOfRessourcesToLoad += 1;
        console.log('TRACE : onResourceRequested: number of ressources to load: ' + numberOfRessourcesToLoad);
    };

    page.onResourceReceived = function (res) {
        if (res.stage === 'end') {
            numberOfRessourcesToLoad -= 1;
            console.log('TRACE : onResourceRequested: number of ressources to load: ' + numberOfRessourcesToLoad);
        }
    };

    page.onLoadFinished = function (status) {
        console.log('TRACE : Loading is finished with status: ' + status);
        if (status !== 'success') {
            console.log('ERROR : Unable to access network (' + status + ')');
        } else {
            setTimeout(checkIfAllRessourcesHasBeenLoaded, params.waitTimeBetweenTwoCheckStatus);
        }
    };

    console.log('DEBUG : opening the url: ' + params.url + '');
    page.open(params.url);
}


function checkIfAllRessourcesHasBeenLoaded() {
    if (numberOfRessourcesToLoad >= 1) {
        if (waitingTimeSpent > params.maxTotalWaitTime) {  
            console.log('WARN : The ressource waiting time has been reached! The program will continue without waiting anymore');
            onAllRessourceLoaded(); //If the max time is reached 
        }
        else {
            console.log("TRACE : Waiting for ressources. Still " + numberOfRessourcesToLoad + " to load.");
            waitingTimeSpent += params.waitTimeBetweenTwoCheckStatus;
            setTimeout(checkIfAllRessourcesHasBeenLoaded, params.waitTimeBetweenTwoCheckStatus);
        }
    }
    else {
        console.log('DEBUG : All ressources has been loaded succefully');
        onAllRessourceLoaded();
    }
}

function onAllRessourceLoaded() {
    if (page.settings.javascriptEnabled) {
        setTimeout(onPageLoadIsCompletellyFinished, params.jsTimeToComplete);
    } else {
        onPageLoadIsCompletellyFinished();
    }
}

function onPageLoadIsCompletellyFinished() {

    console.log(page.evaluate(function () {
        return JSON.stringify({
            "document.body.scrollHeight": document.body.scrollHeight,
            "document.body.offsetHeight": document.body.offsetHeight,
            "document.documentElement.clientHeight": document.documentElement.clientHeight,
            "document.documentElement.scrollHeight": document.documentElement.scrollHeight
        }, undefined, 4);
    }));

    console.log("BEFORE: " + page.evaluate(function () {
        return JSON.stringify({
            "document.body.clientHeight": document.body.clientHeight,
            "document.body.scrollHeight": document.body.scrollHeight,
            "document.body.offsetHeight": document.body.offsetHeight,
            "document.documentElement.clientHeight": document.documentElement.clientHeight,
            "document.documentElement.scrollHeight": document.documentElement.scrollHeight,
            "document.documentElement.offsetHeight:": document.documentElement.offsetHeight
        }, undefined, 4);
    }));

    //Take resize to ensure that we take the full height of the page
    page.viewportSize = { width: params.screenWidth, height: 8000 };


    console.log("AFTER" + page.evaluate(function () {
        return JSON.stringify({
            "document.body.clientHeight": document.body.clientHeight,
            "document.body.scrollHeight": document.body.scrollHeight,
            "document.body.offsetHeight": document.body.offsetHeight,
            "document.documentElement.clientHeight": document.documentElement.clientHeight,
            "document.documentElement.scrollHeight": document.documentElement.scrollHeight,
            "document.documentElement.offsetHeight:": document.documentElement.offsetHeight
        }, undefined, 4);
    }));

    var curHeight = page.evaluate(function () {        
        return document.documentElement.scrollHeight;
    });
    console.log("TRACE : Detected height: " + curHeight);

    page.clipRect = {
        top: 0,
        left: 0,
        height: curHeight,
        width: params.screenWidth
    };

    console.log("INFO : Saving the screenshot to: " + params.targetPath + " page size:  " + params.screenWidth + "x" + curHeight);
    page.render(params.targetPath);

    exitPhantom();
}

function exitPhantom() {
    console.log('INFO : Exit phantomjs!')
    // prevent CI from failing from 'Unsafe JavaScript attempt to access frame with URL about:blank from frame with URL' errors. See https://github.com/n1k0/casperjs/issues/1068
    setTimeout(function () {
        phantom.exit();
    }, 30);
}
