using System.ComponentModel.DataAnnotations;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Contracts.Responses;

public class AbsenceResponse
{
    public int Id { get; set; }
    public string UserId { get; set; }
    [Required(ErrorMessage = "Pole jest wymagane.")]
    public int AbsenceTypeId { get; set; }

    public ConfirmationStatus StatusOfType { get; set; }
    public ConfirmationStatus StatusOfDates { get; set; }
    public ConfirmationStatus ConfirmationStatus { get; set; }

    [Required(ErrorMessage = "Data rozpoczęcia jest wymagana.")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "Data zakończenie jest wymagana.")]
    public DateTime EndDate { get; set; }
    public bool IsFullDate { get; set; }

    [StringLength(500, ErrorMessage = "Powód nie może być dłuższy niż 500 znaków.")]
    public string? Reason { get; set; }
    public UserResponse? User { get; set; }
    public AbsenceTypeResponse? AbsenceType { get; set; }
}

