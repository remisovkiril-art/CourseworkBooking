using Booking.Application.Interfaces.Services;
using Booking.Domain.Entities;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Booking.Infrastructure.Services;

public class PdfService : IPdfService
{
    private readonly ApplicationDbContext _context;

    public PdfService(
        ApplicationDbContext context)
    {
        _context = context;

        QuestPDF.Settings.License =
            LicenseType.Community;
    }

    public async Task<byte[]> GenerateBookingPdfAsync(
        Guid bookingId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        BookingEntity? booking =
            await _context.Bookings
                .Include(x => x.Room)
                .ThenInclude(x => x.Hotel)
                .Include(x => x.User)
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == bookingId &&
                        x.UserId == userId,
                    cancellationToken);

        if (booking == null)
        {
            throw new Exception(
                "Booking not found.");
        }

        string hotelName =
            booking.Room.Hotel.Name;

        string hotelAddress =
            booking.Room.Hotel.Address;

        string roomTitle =
            booking.Room.Title;

        int nights =
            (booking.CheckOutDate.Date -
             booking.CheckInDate.Date).Days;

        if (nights < 1)
        {
            nights = 1;
        }

        byte[] pdf =
            Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(40);

                    page.Header()
                        .Text("Booking confirmation")
                        .FontSize(22)
                        .Bold();

                    page.Content()
                        .PaddingTop(20)
                        .Column(column =>
                        {
                            column.Spacing(10);

                            column.Item()
                                .Text(
                                    $"Booking ID: {booking.Id}");

                            column.Item()
                                .Text(
                                    $"Guest: {booking.User.Name ?? booking.User.Email}");

                            column.Item()
                                .Text(
                                    $"Email: {booking.User.Email}");

                            column.Item()
                                .Text(
                                    $"Hotel: {hotelName}");

                            column.Item()
                                .Text(
                                    $"Address: {hotelAddress}");

                            column.Item()
                                .Text(
                                    $"Room: {roomTitle}");

                            column.Item()
                                .Text(
                                    $"Check-in: {booking.CheckInDate:dd.MM.yyyy}");

                            column.Item()
                                .Text(
                                    $"Check-out: {booking.CheckOutDate:dd.MM.yyyy}");

                            column.Item()
                                .Text(
                                    $"Nights: {nights}");

                            column.Item()
                                .Text(
                                    $"Adults: {booking.AdultsCount}");

                            column.Item()
                                .Text(
                                    $"Children: {booking.ChildrenCount}");

                            column.Item()
                                .Text(
                                    $"Total price: {booking.TotalPrice:F2}");

                            column.Item()
                                .Text(
                                    $"Payment: {(booking.IsPaid ? "Paid" : "Not paid")}");

                            column.Item()
                                .Text(
                                    $"Status: {booking.Status}");

                            if (!string.IsNullOrWhiteSpace(
                                    booking.TravelDetails))
                            {
                                column.Item()
                                    .Text(
                                        $"Travel details: {booking.TravelDetails}");
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                        });
                });
            })
            .GeneratePdf();

        return pdf;
    }
}