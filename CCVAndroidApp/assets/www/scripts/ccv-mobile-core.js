function onBodyLoad()
{		
	document.addEventListener("deviceready", onDeviceReady, false);
}


// declare a prayer object
function PrayerRequest(id,name,category,date,request) {
        this.Id = id;
        this.Name=name;
        this.Category=category;
        this.Date=date;
        this.Request=request;
}

// declare a group object
function Group(id, groupName, leaderName, meetingDay, latitude, longitude, distance, averageAge, size) {
        this.Id=id;
        this.GroupName=groupName;
        this.LeaderName=leaderName;
        this.MeetingDay=meetingDay;
        this.Latitude=latitude;
        this.Longitude=longitude;
        this.Distance=distance;
        this.AverageAge=averageAge;
        this.Size=size;
}

// phone number array for campuses
var campusPhone = new Array(); 
campusPhone[1] = "623.376.2444";       
campusPhone[5] = "623.875.9000";
campusPhone[6] = "480.502.9800";

// define global data vars
var adTitle = '';
var adDetail = '';
var adHighlightImageUrl = '';
var adContactEmail = '';
var adContactPhone = '';
var adContactName = '';
var adUrl = ''; 
var lastFetchDate = '';
var prayerRequests = new Array();
var currentCampus = localStorage.getItem("current-campus");
console.log('[Init]Initialized current campus to: ' + currentCampus);
var lastFetchChannel = new Date("1/1/2012");
var channelXml = null;

var groupNeighborhood = '';
var groupSearchLatitude = '';
var groupSearchLongitude = '';
var groups = [];

var URL_CAMPUS_INFO = 'http://mobileapp.ccvonline.com/arena/MobileApp/MobileXML.aspx?r=False&m=1&p=40&c=';
var URL_GROUP_FINDER = 'http://mobileapp.ccvonline.com/Arena/MobileApp/Groups.aspx';//'http://www.ccvonline.com/jon/groups-xml3.xml';
var URL_PRAYER_ACTIVITY = 'http://mobileapp.ccvonline.com/arena/mobileapp/SavePrayerActivity.aspx';
var URL_PRAYER_ADD_REQUEST = 'http://mobileapp.ccvonline.com/arena/mobileapp/SavePrayerRequest.aspx';
var URL_SEND_INVITE = 'http://mobileapp.ccvonline.com/arena/mobileapp/SendEmail.aspx';
var URL_GROUP_REGISTRATION_EMAIL = 'http://mobileapp.ccvonline.com/Arena/default.aspx?page=16893';
var URL_SERIES_LIST = 'http://mobileapp.ccvonline.com/Arena/mobileapp/feed/channel.aspx?r=true&channel=1'; /*parms count=, start= */
var URL_SERIES_SERMON_LIST = 'http://mobileapp.ccvonline.com/Arena/mobileapp/feed/topic.aspx?r=true&topic=';
var URL_SERIES_SERMON_DETAILS = 'http://mobileapp.ccvonline.com/Arena/mobileapp/feed/item.aspx?r=true';  /*parms item=, channel= use channel for lastest sermon */
var URL_CHANNEL_DETAILS = 'http://mobileapp.ccvonline.com/Arena/MobileApp/Feed/Channel.aspx?channel=1&count=25';


/*
	The parameters URL_PRAYER_ACTIVITY accepts are:
 
	R = the prayer request id
	P = (optional) the person id of the person that prayed.  If omitted, the activity is associated with the Anonymous Anonymous person.
	C = (optional) a comment.  If a comment is included, the activity will need to be approved
	U = (optional) the userid to use when saving the activity.  If omitted, ÒMobileAppÓ will be used
	
	The parameters URL_PRAYER_ADD_REQUEST accepts are:
 
	first-name
	last-name
	email
	is-public (optional, default is false)
	source (optional)
	content-category (optional)
	category (optional)
	request
	U = (optional) the userid to use when saving the activity.  If omitted, ÒMobileAppÓ will be used
	
	The parameters URL_SEND_INVITE accepts are:
	to-address
	from-name
	from-address
	subject
	body
	
*/

function getCampusName(campusId) {
	switch (campusId) {
	   case '1':
	      return 'Peoria';
	      break;
	   case '5':
	      return 'Surprise';
	      break;
	   case '6':
	      return 'Scottsdale';
	      break;
	   default:
	   	  return campusId;
	}
}

function getCurrentCampusName() {
	currentCampus = localStorage.getItem("current-campus");
	
	return getCampusName(currentCampus);
}

function initGlobalVariables() {

	adTitle = localStorage.getItem("ad-title");
	adDetail = localStorage.getItem("ad-detail");
	adHighlightImageUrl = localStorage.getItem("ad-highlight-image-url");
	adContactEmail = localStorage.getItem("ad-contact-email");
	adContactPhone = localStorage.getItem("ad-contact-phone");
	adContactName = localStorage.getItem("ad-contact-name");
	adUrl = localStorage.getItem("ad-url"); 
	
	lastFetchDate = new Date(localStorage.getItem("last-fetch"));
	prayerRequests = JSON.parse(localStorage.getItem("prayer-requests"));
	currentCampus = localStorage.getItem("current-campus");
	
	if (lastFetchDate == null) {
	
		lastFetchDate = new Date(1900, 1, 1);
	}	
}

function onDeviceReady()
{
    initGlobalVariables();

	// create event for app resume
	document.addEventListener("resume", onResume, false);
	
	// load saved campus
	currentCampus = localStorage.getItem("current-campus");
	console.log('[Core]Initialized current campus to: ' + currentCampus);
 
	//refreshFeature(true);
}

// alert dialog dismissed
function alertDismissed() {
    // do something
}


// Handle the resume event
function onResume() {
	console.log('[Core]Resuming app');
	refreshFeature(true);
}

// Handle the online event
function onOnline() {
	//alert('welcome to the internets');
	refreshFeature(true);
}

function refreshFeature(useCache) {
	
	console.log('[Core]Refresh feature called use cache = ' + useCache);
	
	var networkState = navigator.network.connection.type;
    if (networkState == Connection.NONE || networkState == Connection.UNKNOWN)
    {
    	
    	// set standard campus background
    	//var currentCampus = localStorage.getItem("current-campus");


		// remove campus css classes
		$(".feature-image").removeClass (function (index, css) {
			return (css.match (/\bcampus-\S+/g) || []).join(' ');
		});
		
    	$('.feature-image').addClass('campus-' + currentCampus);
    	
    	// clear global vars
    	adTitle = '';
		adDetail = '';
		adHighlightImageUrl = '';
		adDetails = '';
		adContactEmail = '';
		adContactPhone = '';
		adContactName = '';
		adUrl = ''; 

    }
    else {
    	console.log('Here2' + lastFetchDate.getTime());
    	var currentTime = new Date();
    	var timeSinceFetch = currentTime.getTime() - lastFetchDate.getTime();
    	console.log("[Core] Time since last fetch: " + timeSinceFetch);
    	
    	if (useCache == false || adTitle == '') { 
    		
    		console.log('[Core]Ignorning cache, requesting remote data');
    		getRemoteData();
    	}
    	else
    	{
    		if (timeSinceFetch > 3600000) {
    			console.log('[Core]Cache expried, requesting remote data');;
    			getRemoteData();
    		}
    	}
    	
    	renderFeature();
    }
}

function renderFeature() {
	
	// show the ads unless there isn't one to show
	
	// remove campus css classes
	$(".feature-image").removeClass (function (index, css) {
		return (css.match (/\bcampus-\S+/g) || []).join(' ');
	});
	
	if (adTitle != '') {
		$('.feature-image').css('background-image', 'url(' + adHighlightImageUrl + ')');
	}
	else
	{			
		$('.feature-image').addClass('campus-' + currentCampus);
	}
}

function getRemoteData()
{
	// clear local variables
	localStorage.setItem('ad-title', '');
    localStorage.setItem('ad-highlight-image-url', null);
    localStorage.setItem('ad-contact-email', null);
    localStorage.setItem('ad-contact-phone', null);
    localStorage.setItem('ad-contact-name', null);
    localStorage.setItem('ad-url', null);
	localStorage.setItem('ad-detail', null);
	
	var requestUrl = URL_CAMPUS_INFO + localStorage.getItem("current-campus");
	console.log('[Core]Requesting:' + requestUrl);

    // get updated xml for populating ads and prayers
    $.ajax({
	    type: 'GET',
	    url: requestUrl,
	    async: false,
	    dataType: 'xml',
	    error: function(xhr, status, error) {
			navigator.notification.alert("An error occurred trying to retrieve data from the CCV server.  Please ensure you have a connection to the Internet.", alertDismissed, 'Ooops...', 'Continue');
		},
	    success: function(xml) {
	        
           // get ads
           $(xml).find('ad').each(function(){
                localStorage.setItem('ad-title', $(this).attr('title'));
			    localStorage.setItem('ad-highlight-image-url', $(this).attr('highlight-image'));
			    localStorage.setItem('ad-contact-email', $(this).attr('contact-email'));
			    localStorage.setItem('ad-contact-phone', $(this).attr('contact-phone'));
			    localStorage.setItem('ad-contact-name', $(this).attr('contact-name'));
			    localStorage.setItem('ad-url', $(this).attr('url'));
				localStorage.setItem('ad-detail', $(this).find('details').text());
			});
           
           
           // load prayer requests
           prayerRequests = new Array();
           
           $(xml).find('prayer').each(function(){
           		var requestId = $(this).attr('id');
           		var requestName = $(this).attr('name');
           		var requestCategory = $(this).attr('category');
           		var requestDate = $(this).attr('date');
           		var requestText = $(this).find('request').text();
           		
           		prayerRequests.push(new PrayerRequest(requestId, requestName, requestCategory, requestDate, requestText));
           }); 
           
           // store requests in local storage
           localStorage.setItem('prayer-requests', JSON.stringify(prayerRequests));
           
           
           
           // set fetch date
           localStorage.setItem('last-fetch', new Date());
           console.log('[Core]Setting last fetch date to: ' + new Date());
           
           // reinit vars
           initGlobalVariables();
	    }
	});
}
 
function setCampusPhone() {
    $('.campus-phone .ui-btn-text').text(campusPhone[currentCampus]);
	$('.campus-phone').attr("href", 'tel:' + campusPhone[currentCampus].replace(".", ""));
}

function getChannelData() {
	
	// check channel data cache
	var millisecondsPerHour = 1000 * 60 * 60;
	var dateNow = new Date();

	var timeDiff = (dateNow - lastFetchChannel) / millisecondsPerHour;
	
	if (timeDiff > 1 || channelXml == null) // get updated data from server
	{
		console.log('[Channel Data]Getting updated channel data. Time diff: ' + timeDiff);
		
		// check that we have a internet connection
		var networkState = navigator.network.connection.type;
		if (networkState == Connection.NONE || networkState == Connection.UNKNOWN) {
			console.log('[Channel Data]Error no network connection available.');
			return null;
		}

		// get remote data
		var requestChannelDetails = URL_CHANNEL_DETAILS;
		
		$.ajax({
		    type: 'GET',
		    url: requestChannelDetails,
		    async: false,
		    dataType: 'xml',
		    error: function(xhr, status, error) {
				console.log('[Channel Data]Error attempting to get channel details: ' + error);
				return null;
			},
		    success: function(xml) {
		        channelXml = xml;
		    	lastFetchChannel = Date();
		    }
		});		       
		
		return channelXml;

	} else {
		console.log('[Channel Data]Using cached data. Time diff: ' + timeDiff);
		return channelXml;
	}
	
}