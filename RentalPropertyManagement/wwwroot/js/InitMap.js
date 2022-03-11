function initMap(lat, lng, title) {
    console.log('dsdsdsd')
    var mapDiv = document.getElementsByClassName("pleaseWork")
    console.log('Find by class: ')
    console.log(mapDiv)


    var mapDivId = document.getElementById('map')
    console.log('Find by id: ')
    console.log(mapDivId)

    var latlng = new google.maps.LatLng(lat,lng);
    var options = {
        zoom: 14, center: latlng,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    var map = new google.maps.Map(document.getElementById
        ("map"), options);

    new google.maps.Marker({
        position: latlng,
        map,
        title: title,
    });
}
