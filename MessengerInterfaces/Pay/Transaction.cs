using MessengerInterfaces.Security.Pay;

namespace MessengerInterfaces.Pay;

public record Transaction(PaymentToken Token, float newBalance);