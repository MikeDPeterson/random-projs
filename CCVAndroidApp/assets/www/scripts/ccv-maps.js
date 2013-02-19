/* marker maker http://powerhut.co.uk/googlemaps/custom_markers.php */


var houseImage = new google.maps.MarkerImage(
	'assets/icons/house.png',
	new google.maps.Size(31,31),
	new google.maps.Point(0,0),
	new google.maps.Point(16,31)
	); 
	
var houseShadow = new google.maps.MarkerImage(
	'assets/icons/house-shadow.png',
	new google.maps.Size(51,31),
	new google.maps.Point(0,0),
	new google.maps.Point(16,31)
	); 

var houseShape = {
	coord: [15,0,16,1,17,2,18,3,19,4,20,5,21,6,22,7,23,8,24,9,25,10,26,11,27,12,28,13,29,14,30,15,30,16,29,17,27,18,27,19,27,20,27,21,27,22,27,23,27,24,27,25,27,26,27,27,27,28,27,29,27,30,2,30,2,29,2,28,2,27,2,26,2,25,2,24,2,23,2,22,2,21,2,20,2,19,2,18,0,17,0,16,0,15,0,14,1,13,2,12,3,11,4,10,5,9,6,8,7,7,8,6,9,5,10,4,11,3,12,2,13,1,14,0,15,0],
	type: 'poly'
	}; 
	

var flagImage = new google.maps.MarkerImage(
	'assets/icons/flag.png',
	new google.maps.Size(31,31),
	new google.maps.Point(0,0),
	new google.maps.Point(0,31)
	);
	
var flagShadow = new google.maps.MarkerImage(
	'assets/icons/flag-shadow.png',
	new google.maps.Size(51,31),
	new google.maps.Point(0,0),
	new google.maps.Point(0,31)
	);
	
var flagShape = {
	coord: [15,0,18,1,30,2,30,3,30,4,30,5,30,6,30,7,30,8,30,9,30,10,30,11,30,12,30,13,30,14,30,15,30,16,30,17,30,18,29,19,24,20,19,21,6,22,6,23,6,24,6,25,6,26,6,27,6,28,6,29,6,30,1,30,1,29,1,28,1,27,1,26,1,25,1,24,1,23,1,22,1,21,1,20,1,19,1,18,1,17,1,16,1,15,1,14,1,13,1,12,1,11,1,10,1,9,1,8,1,7,1,6,1,5,1,4,1,3,1,2,1,1,1,0,15,0],
	type: 'poly'
	};
	
	
var ccvImage = new google.maps.MarkerImage(
  	'assets/icons/ccv.png',
  	new google.maps.Size(32,37),
  	new google.maps.Point(0,0),
  	new google.maps.Point(16,37)
  	);

var ccvShadow = new google.maps.MarkerImage(
  	'assets/icons/ccv-shadow.png',
  	new google.maps.Size(54,37),
  	new google.maps.Point(0,0),
  	new google.maps.Point(16,37)
  	);

var ccvShape = {
  	coord: [29,0,30,1,31,2,31,3,31,4,31,5,31,6,31,7,31,8,31,9,31,10,31,11,31,12,31,13,31,14,31,15,31,16,31,17,31,18,31,19,31,20,31,21,31,22,31,23,31,24,31,25,31,26,31,27,31,28,31,29,30,30,29,31,23,32,22,33,21,34,20,35,19,36,12,36,11,35,10,34,9,33,8,32,2,31,1,30,0,29,0,28,0,27,0,26,0,25,0,24,0,23,0,22,0,21,0,20,0,19,0,18,0,17,0,16,0,15,0,14,0,13,0,12,0,11,0,10,0,9,0,8,0,7,0,6,0,5,0,4,0,3,0,2,1,1,2,0,29,0],
  	type: 'poly'
  	};