window.leafletInterop = (() => {
    let map;
    let userMarkers = {};
    let clickMarker = null;
    let routeControl = null;

    const iconSize = [40, 40];

    const createIcon = (imgUrl) => {
        return L.icon({
            iconUrl: imgUrl,
            iconSize: iconSize,
            iconAnchor: [20, 40]
        });
    };

    const isSameLocation = (latlng1, latlng2, threshold = 0.0005) => {
        return Math.abs(latlng1.lat - latlng2.lat) < threshold &&
            Math.abs(latlng1.lng - latlng2.lng) < threshold;
    };

    return {
        initMap: function (divId, startLat, startLng, zoom = 13) {
            map = L.map(divId).setView([startLat, startLng], zoom);

            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '&copy; OpenStreetMap contributors'
            }).addTo(map);
        },

        enableClickMarker: function () {
            map.on('click', function (e) {
                if (clickMarker && isSameLocation(clickMarker.getLatLng(), e.latlng)) {
                    map.removeLayer(clickMarker);
                    clickMarker = null;
                } else {
                    if (clickMarker) {
                        map.removeLayer(clickMarker);
                    }
                    clickMarker = L.marker(e.latlng).addTo(map);
                }
            });
        },

        addUser: function (userId, lat, lng, imgUrl) {
            if (userMarkers[userId]) {
                map.removeLayer(userMarkers[userId]);
            }

            const marker = L.marker([lat, lng], {
                icon: createIcon(imgUrl)
            }).addTo(map);

            userMarkers[userId] = marker;
        },

        moveUser: function (userId, lat, lng) {
            if (userMarkers[userId]) {
                userMarkers[userId].setLatLng([lat, lng]);
            }
        },

        removeUser: function (userId) {
            if (userMarkers[userId]) {
                map.removeLayer(userMarkers[userId]);
                delete userMarkers[userId];
            }
        },

        drawRoute: function (startLat, startLng, endLat, endLng) {
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
    };
})();
