namespace prijimacky_backend.Graphql.Types;

public record EmailStatistics(
    int Accepted,
    int WithoutPaymentConfirmationEmail,
    int WithoutCancelationConfirmationEmail,
    int WaitingForPayment,
    int Canceled, 
    int WithoutEmailNotifyingOfFreeSpot);