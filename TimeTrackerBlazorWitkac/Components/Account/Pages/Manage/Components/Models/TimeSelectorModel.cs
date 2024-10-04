namespace TimeTrackerBlazorWitkac.Components.Account.Pages.Manage.Components.Models;

public class TimeSelectorModel<TValue>
{
    public string Label { get; set; }
    public TValue Value { get; set; }
    public int[] Values { get; set; }
}
public class TimeSelectorService
{
    public TimeSelectorModel<int> Create(
        string label,
        int selectedValue,
        int[] values
        ) => new TimeSelectorModel<int> { Label = label, Value = selectedValue, Values = values };
}


