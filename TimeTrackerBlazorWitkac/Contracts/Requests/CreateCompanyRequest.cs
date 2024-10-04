namespace TimeTrackerBlazorWitkac.Contracts.Requests;

public class CreateCompanyRequest
{
    public string Name { get; set; }
    public DateOnly? DateOfFoundation { get; set; }
}