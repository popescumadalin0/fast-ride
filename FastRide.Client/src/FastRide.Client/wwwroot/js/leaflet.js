let map;
let userMarkers = {};
let clickMarker = null;
let routeControl = null;

const iconSize = [40, 40];

window.leafletCreateIcon = (imgUrl) => {
    return L.icon({
        iconUrl: imgUrl,
        iconSize: iconSize,
        iconAnchor: [20, 40]
    });
}

window.leafletIsSameLocation = (latlng1, latlng2, threshold = 0.0005) => {
    return Math.abs(latlng1.lat - latlng2.lat) < threshold &&
        Math.abs(latlng1.lng - latlng2.lng) < threshold;
}

window.leafletDispose = () => {
    map.remove();
}

window.leafletInitMap = (startLat, startLng, zoom = 13) => {
    if (map) {
        map.remove();
    }

    map = L.map('map').setView([startLat, startLng], zoom);

    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(map);

    map.on('click', onMapClick);
}

function onMapClick(e) {
    if (clickMarker && leafletIsSameLocation(clickMarker.getLatLng(), e.latlng)) {
        map.removeLayer(clickMarker);
        clickMarker = null;
    } else {
        if (clickMarker) {
            map.removeLayer(clickMarker);
        }
        clickMarker = L.marker(e.latlng).addTo(map);
    }
}

window.leafletAddUser = (userId, lat, lng, imgUrl) => {
    if (userMarkers[userId]) {
        map.removeLayer(userMarkers[userId]);
    }

    const marker = L.marker([lat, lng], {
        icon: createIcon(imgUrl)
    }).addTo(map);

    userMarkers[userId] = marker;
}

window.leafletMoveUser = (userId, lat, lng) => {
    if (userMarkers[userId]) {
        userMarkers[userId].setLatLng([lat, lng]);
    }
}

window.leafletRemoveUser = (userId) => {
    if (userMarkers[userId]) {
        map.removeLayer(userMarkers[userId]);
        delete userMarkers[userId];
    }
}

window.drawRoute = (startLat, startLng, endLat, endLng) => {
    if (routeControl) {
        map.removeControl(routeControl);
    }

    routeControl = L.Routing.control({
        waypoints: [
            L.latLng(startLat, startLng),
            L.latLng(endLat, endLng)
        ],
        routeWhileDragging: false,
        draggableWaypoints: false,
        addWaypoints: false
    }).addTo(map);
}
