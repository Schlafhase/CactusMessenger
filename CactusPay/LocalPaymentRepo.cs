using MessengerInterfaces.Local;

namespace CactusPay;

public class LocalPaymentRepo(string root) : LocalRepositoryBase<PaymentManager>(root, "paymentManager") { }