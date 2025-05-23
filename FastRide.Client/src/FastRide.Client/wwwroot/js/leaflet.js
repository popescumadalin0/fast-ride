let map;
let userMarkers = {};
let clickMarker = null;
let routeControl = null;

let dotNetInstance;

let onClickCallback;

const iconSize = [40, 40];

window.leafletCreateIcon = (imgUrl) => {
    return L.icon({
        iconUrl: imgUrl,
        iconSize: iconSize,
        iconAnchor: [20, 40]
    });
}

window.leafletIsSameLocation = (latlng1, latlng2, threshold = 0.00005) => {
    return Math.abs(latlng1.lat - latlng2.lat) < threshold &&
        Math.abs(latlng1.lng - latlng2.lng) < threshold;
}

window.leafletDispose = () => {
    //map.remove();
}

window.leafletInitMap = (dotNet, startLat, startLng, zoom = 13, clickCallback = 'OnClickMap') => {
/*    if (map) {
        map.remove();
    }*/

    onClickCallback = clickCallback;
    dotNetInstance = dotNet;

    map = L.map('map').setView([startLat, startLng], zoom);

    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(map);

    map.on('click', onMapClick);
}

function onMapClick(e) {
    if (dotNetInstance) {
        if (clickMarker && leafletIsSameLocation(clickMarker.getLatLng(), e.latlng)) {
            map.removeLayer(clickMarker);
            clickMarker = null;
        } else {
            if (clickMarker) {
                map.removeLayer(clickMarker);
            }
            clickMarker = L.marker(e.latlng, {
                icon: leafletCreateIcon("icons/pin.png")
            }).addTo(map);
        }
        const latLng = clickMarker ==null ? null : { lat: clickMarker.getLatLng().lat, lng: clickMarker.getLatLng().lng };
        dotNetInstance.invokeMethodAsync(onClickCallback, latLng);
    }
}

window.leafletAddUser = (userId, lat, lng, imgUrl) => {
    if (map == null)
    {
        return;
    }
    if (userMarkers[userId]) {
        map.removeLayer(userMarkers[userId]);
    }

    const marker = L.marker([lat, lng], {
        icon: leafletCreateIcon(imgUrl)
    }).addTo(map);

    userMarkers[userId] = marker;
}

window.leafletRemoveUser = (userId) => {
    if (map == null)
    {
        return;
    }
    if (userMarkers[userId]) {
        map.removeLayer(userMarkers[userId]);
        delete userMarkers[userId];
    }
}

window.drawRoute = (startLat, startLng, endLat, endLng) => {
    if (map == null)
    {
        return;
    }
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
