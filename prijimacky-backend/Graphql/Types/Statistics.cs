namespace prijimacky_backend.Graphql.Types;

public record Statistics(int TotalSignups, int Capacity, int RemainingCapacity, int RemainingCapacityOver, int RemovedSignups);