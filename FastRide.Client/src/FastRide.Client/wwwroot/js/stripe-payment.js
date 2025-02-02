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

async function sendCheckout() {
    const {error} = await stripe.confirmPayment({
        elements,
        confirmParams: {
            return_url: 'https://www.google.com',
        },
        redirect: "if_required"
    });

    /* if (error.type === "card_error" || error.type === "validation_error") {
         return error.message;
     }*/

    return error.message;
}

window.checkoutStripe = () => {
    var string
}