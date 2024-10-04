namespace TimeTrackerBlazorWitkac.Resources;

public static class Constants
{
    #region Colors Values
    // Default Colors for Calendar
    public const string DefaultColor = "#000000";
    public const string DefaultPendingIconColor = "#000000";
    public const string DefaultPendingBackgroundColor = "#ffc107";
    public const string DefaultPendingBackgroundColorByUser = "#ffc107";
    public const string DefaultAcceptedIconColor = "#000000";
    public const string DefaultAcceptedBackgroundColor = "#29a847";
    public const string DefaultAcceptedBackgroundColorByUser = "#29a848";
    public const string DefaultAttendanceBackgroundColor = "#17a2b8";

    // Default Color for Rendering 
    public const string DefaultBackgroundDisablePastDates = "var(--rz-base-light)";
    public const string DefaultBackgroundHighlightingDates = "var(--rz-scheduler-highlight-background-color, rgba(255,220,40,.2))";

    //Default Color for TooltipOptions
    public const string DefaultToolTipColor = "var(--rz-text-color)";
    public const string DefaultBackgroundToolTipColor = "var(--rz-base-lighter)";
    #endregion

    #region Opacity Values
    public const double DefaultOpacity = 80;
    public const double DefaultOpacityByUser = 100;
    #endregion

    #region Pointer Events && Cursors
    public const string DefaultPointerEvent = "none";
    public const string DefaultCursor = "not-allowed";
    #endregion

    #region Duration ToolTip's 
    public const int DefaultDurationToolTip = 5000;
    #endregion
}

