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

window.toLocalTimeString = (utcString) => {
    const date = new Date(utcString);

    const day = date.getDate().toString().padStart(2, '0');
    const month = date.toLocaleString('default', { month: 'short' });
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');

    return `${day} ${month}, ${hours}:${minutes}`;
};