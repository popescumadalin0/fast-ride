window.getGeolocation = () => {
    return new Promise((resolve, reject) => {
        if (navigator.geolocation) {
            navigator.geolocation.watchPosition(
                (position) => {
                    const location = {
                        Latitude: position.coords.latitude,
                        Longitude: position.coords.longitude,
                    };
                    resolve(location);
                },
                (error) => {
                    reject(error.message);
                }
            );
        } else {
            reject("Geolocation is not supported by this browser.");
        }
    });
};