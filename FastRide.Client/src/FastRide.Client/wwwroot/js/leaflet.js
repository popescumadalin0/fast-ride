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

window.leafletInitMap = (dotNet, userId, startLat, startLng, img, zoom = 13, clickCallback = 'OnClickMap') => {
    onClickCallback = clickCallback;
    dotNetInstance = dotNet;

    routeControl = null;
    clickMarker = null;
    userMarkers = {};

    map = L.map('map').setView([startLat, startLng], zoom);

    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(map);

    leafletAddUser(userId, startLat, startLng, img);

    map.on('click', onMapClick);
}

function onMapClick(e) {
    if (dotNetInstance) {
        const latLng = clickMarker && leafletIsSameLocation(clickMarker.getLatLng(), e.latlng) ? null : {
            lat: e.latlng.lat,
            lng: e.latlng.lng
        };
        dotNetInstance.invokeMethodAsync(onClickCallback, latLng);
    }
}

window.leafletSetPinLocation = (e) => {
    if (e == null) {
        if (clickMarker) {
            map.removeLayer(clickMarker);
            clickMarker = null;
        } else {
            return;
        }
    } else {
        if (clickMarker) {
            map.removeLayer(clickMarker);
        }
        clickMarker = L.marker(e.latlng, {
            icon: leafletCreateIcon("icons/pin.png")
        }).addTo(map);
    }
}

window.leafletAddUser = (userId, lat, lng, imgUrl) => {
    if (map == null) {
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
    if (map == null) {
        return;
    }
    if (userMarkers[userId]) {
        map.removeLayer(userMarkers[userId]);
        delete userMarkers[userId];
    }
}

window.leafletDrawRoute = (startLat, startLng, endLat, endLng) => {
    if (map == null) {
        return;
    }
    if (routeControl) {
        routeControl.setWaypoints([
            L.latLng(startLat, startLng),
            L.latLng(endLat, endLng)
        ]);
    } else {
        routeControl = L.Routing.control({
            waypoints: [
                L.latLng(startLat, startLng),
                L.latLng(endLat, endLng)
            ],
            routeWhileDragging: true,
            lineOptions: {
                styles: [{color: 'blue', opacity: 1, weight: 5}]
            },
            createMarker: function () {
                return null;
            },
        }).addTo(map);
    }
}

window.leafletRemoveRoute = () => {
    if (map == null) {
        return;
    }
    if (routeControl) {
        map.removeControl(routeControl);
    }
}
