namespace ASC.Model.BaseTypes
{
    public static class Constants
    {
    }

    public enum Roles
    {
        Admin, Engineer, User
    }

    public enum MasterKeys
    {
        VehicleName, VehicleType
    }

    public enum Status
    {
        New, Denied, Pending, Initiated, InProgress, PendingCustomerApproval,
        RequestForInformation, Completed
    }
}
