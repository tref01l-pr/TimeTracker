﻿namespace TimeTrackerBlazorWitkac.Options;

public class SmtpOptions
{
    public const string Smtp = "Smtp";

    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Port { get; set; } = 0;
}