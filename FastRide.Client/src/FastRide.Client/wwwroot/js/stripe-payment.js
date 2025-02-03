let elements = null;
let stripe = null;

window.initializeStripe = (dotNetInstance, clientSecret, publishKey, validationChangedCallback) => {
    stripe = Stripe(publishKey);

    elements = stripe.elements({clientSecret});

    const paymentElementOptions = {
        layout: "tabs",
    };

    const paymentElement = elements.create("payment", paymentElementOptions);
    paymentElement.mount("#payment-element");

    dotNetInstance.invokeMethodAsync(validationChangedCallback, false);

    paymentElement.on('change', function (event) {
        var result = event.complete;

        dotNetInstance.invokeMethodAsync(validationChangedCallback, result);
    });
}

window.checkoutStripe = (dotNetHelper) => {
    stripe.confirmPayment({
        elements,
        confirmParams: {
            return_url: 'https://www.google.com',
        },
        redirect: "if_required"
    }).then((payload) => {
        success(payload);
    });

    function success(payload) {
        /* if (error.type === "card_error" || error.type === "validation_error") {
            return error.message;
        }*/
        dotNetHelper.invokeMethodAsync('OnPaymentChangedAsync', payload.message);
    }
}
