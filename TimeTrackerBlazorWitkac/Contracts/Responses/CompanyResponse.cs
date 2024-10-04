namespace TimeTrackerBlazorWitkac.Contracts.Responses;

public class CompanyResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateOnly? DateOfFoundation { get; set; }
}