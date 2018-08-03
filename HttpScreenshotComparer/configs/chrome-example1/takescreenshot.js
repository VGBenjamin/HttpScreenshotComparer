const puppeteer = require('puppeteer');

(async () =>  {
  var params = {};

  try 
  {
    if (process.argv.length !== 6) {
        console.log("ERROR : Incorrect number of arguments expected 4 but " + process.argv.length + "found");
	} else {		
        params.targetPath = clearString(process.argv[2]);
        params.urlName = clearString(process.argv[3]);
        params.url = clearString(process.argv[4]);
        params.screenWidth = parseInt(process.argv[5]);
		
		console.log("DEBUG : executing the chrome headless with the parameters:\n" + JSON.stringify(params, null, "    "));
				
		const browser = await puppeteer.launch();
		try {
			const page = await browser.newPage();
			await page.setViewport({ width: params.screenWidth, height: 800 })
			
			console.log("DEBUG - Opening the url: " + params.url);
			await page.goto(params.url);
			
			console.log("DEBUG - Your screenshot will be saved here: '" + params.targetPath + "'");
			await page.screenshot({ path: params.targetPath, fullPage: true});
		} finally {
			await browser.close();
		}
	}
  } catch (error) {
	  console.log('ERROR : Unexpected error - ' + error.message);		
  }
  
})();

// Remove the single quote around the string if needed
function clearString(pathToClean) {
    return pathToClean.replace(/'(.+)'/, '$1');
}