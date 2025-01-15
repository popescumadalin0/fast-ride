/*
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
*/

window.getGeolocation = (dotNetHelper, enableHighAccuracy, maximumAge) => {

    const options = {
        enableHighAccuracy: enableHighAccuracy,
        timeout: 1000,
        maximumAge: maximumAge
    };

    function success(position) {

        const coordinate = {
            latitude: position.coords.latitude,
            longitude: position.coords.longitude,
            accuracy: position.coords.accuracy
        };

        dotNetHelper.invokeMethodAsync('OnSuccessAsync', coordinate);
    }

    function error(error) {

        const errorDetails = {
            errorCode: error.code,
            errorMessage: error.message
        };

        dotNetHelper.invokeMethodAsync('OnErrorAsync', errorDetails);
    }

    navigator.geolocation.getCurrentPosition(success, error, options);
}