let elements = null;
let stripe = null;

window.initializeStripe = (clientSecret, publishKey) => {
    stripe = Stripe(publishKey);
    
    elements = stripe.elements({ clientSecret });

    const paymentElementOptions = {
        layout: "tabs",
    };

    const paymentElement = elements.create("payment", paymentElementOptions);
    paymentElement.mount("#payment-element");
}

window.checkoutStripe = async () => {
    
    const { error } = await stripe.confirmPayment({
        elements
    });

    if (error.type === "card_error" || error.type === "validation_error") {
        return error.message;
    }
    
    return "";
}