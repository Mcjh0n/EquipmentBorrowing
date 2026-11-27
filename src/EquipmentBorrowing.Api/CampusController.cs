using EquipmentBorrowing.Application;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Contracts;
using EquipmentBorrowing.Domain;
using Microsoft.AspNetCore.Mvc;
namespace EquipmentBorrowing.Api;

[ApiController, Route("api/v1")]
[ProducesResponseType<ApiError>(400), ProducesResponseType<ApiError>(404), ProducesResponseType<ApiError>(409), ProducesResponseType<ApiError>(500)]
public class CampusController(IStudentRepository students, IEquipmentRepository equipment, IBorrowingRepository borrowings, StudentManagementService studentService, EquipmentManagementService equipmentService, BorrowingLookupService lookup, ApiBorrowingService workflow) : ControllerBase {
    [HttpGet("students"), ProducesResponseType<StudentDto[]>(200)]
    public async Task<IActionResult> Students([FromQuery] string? search, CancellationToken ct) {
        var rows = await students.GetAllAsync(ct); var term=search?.Trim();
        return Ok(rows.Where(s => string.IsNullOrEmpty(term) || s.Name.Contains(term,StringComparison.OrdinalIgnoreCase) || s.StudentNumber.Contains(term,StringComparison.OrdinalIgnoreCase)).Select(s=>s.Dto()));
    }
    [HttpGet("students/{id}"), ProducesResponseType<StudentDto>(200)]
    public async Task<IActionResult> Student(int id, CancellationToken ct) => Ok((await students.GetByIdAsync(id,ct) ?? throw new KeyNotFoundException("Student not found.")).Dto());
    [HttpPost("students"), ProducesResponseType<StudentDto>(201)]
    public async Task<IActionResult> CreateStudent(StudentRequest input,CancellationToken ct) {
        var row=await studentService.RegisterAsync(input.StudentNumber,input.FullName,input.Contact,ct);
        return Created($"/api/v1/students/{row.Id}",row.Dto());
    }
    [HttpPut("students/{id}"), ProducesResponseType<StudentDto>(200)]
    public async Task<IActionResult> UpdateStudent(int id,StudentRequest input,CancellationToken ct) {
        await studentService.UpdateAsync(id,input.StudentNumber,input.FullName,input.Contact,ct); return await Student(id,ct);
    }
    [HttpPatch("students/{id}/eligibility"), ProducesResponseType<StudentDto>(200)]
    public async Task<IActionResult> Eligibility(int id,EligibilityRequest input,CancellationToken ct) {
        await studentService.SetEligibilityAsync(id,input.IsEligible!.Value,ct); return await Student(id,ct);
    }
    [HttpGet("equipment"), ProducesResponseType<EquipmentDto[]>(200)]
    public async Task<IActionResult> Equipment([FromQuery] bool? available,CancellationToken ct) => Ok((available==true ? await equipment.GetAvailableAsync(ct) : await equipment.GetAllAsync(ct)).Select(e=>e.Dto()));
    [HttpGet("equipment/{id}"), ProducesResponseType<EquipmentDto>(200)]
    public async Task<IActionResult> EquipmentItem(int id,CancellationToken ct) => Ok((await equipment.GetByIdAsync(id,ct) ?? throw new KeyNotFoundException("Equipment not found.")).Dto());
    [HttpPost("equipment"), ProducesResponseType<EquipmentDto>(201)]
    public async Task<IActionResult> CreateEquipment(EquipmentRequest input,CancellationToken ct) {
        var row=await equipmentService.RegisterAsync(input.EquipmentCode,input.Name,input.Description,ct);
        return Created($"/api/v1/equipment/{row.Id}",row.Dto());
    }
    [HttpPut("equipment/{id}"), ProducesResponseType<EquipmentDto>(200)]
    public async Task<IActionResult> UpdateEquipment(int id,EquipmentRequest input,CancellationToken ct) {
        await equipmentService.UpdateAsync(id,input.EquipmentCode,input.Name,input.Description,ct); return await EquipmentItem(id,ct);
    }
    [HttpGet("borrowings/{id}"), ProducesResponseType<BorrowingDto>(200)]
    public async Task<IActionResult> Borrowing(int id,CancellationToken ct) => Ok((await borrowings.GetByIdAsync(id,ct) ?? throw new KeyNotFoundException("Borrowing not found.")).Dto());
    [HttpPost("borrowings"), ProducesResponseType<BorrowingDto>(201)]
    public async Task<IActionResult> Borrow(BorrowRequest input,CancellationToken ct) {
        ValidateDate(input.ExpectedReturnDate!.Value);
        var rows=await workflow.CheckoutAsync(input.StudentId!.Value,[input.EquipmentId!.Value],input.ExpectedReturnDate.Value,false,ct);
        return Created($"/api/v1/borrowings/{rows[0].Id}",rows[0].Dto());
    }
    [HttpPost("checkout-requests"), ProducesResponseType<CheckoutResponse>(201)]
    public async Task<IActionResult> Checkout(CheckoutRequest input,CancellationToken ct) {
        if(input.EquipmentIds.Length<2 || input.EquipmentIds.Distinct().Count()!=input.EquipmentIds.Length) throw new ArgumentException("Select at least two distinct equipment items.");
        ValidateDate(input.ExpectedReturnDate!.Value);
        var rows=await workflow.CheckoutAsync(input.StudentId!.Value,input.EquipmentIds,input.ExpectedReturnDate.Value,true,ct);
        return StatusCode(201,new CheckoutResponse(rows.Select(b=>b.Dto()).ToArray()));
    }
    [HttpPost("borrowings/{id}/return"), ProducesResponseType<BorrowingDto>(200)]
    public async Task<IActionResult> Return(int id,ReturnRequest input,CancellationToken ct) {
        return Ok((await workflow.ReturnAsync(id,ct)).Dto());
    }
    [HttpGet("borrowings"), ProducesResponseType<BorrowingDto[]>(200)]
    public async Task<IActionResult> Borrowings([FromQuery] string? search,[FromQuery] string status="All",[FromQuery] string sort="borrowedDate",[FromQuery] string direction="asc",CancellationToken ct=default) {
        if(status is not ("All" or "Active" or "Returned") || sort is not ("borrowedDate" or "expectedReturnDate") || direction is not ("asc" or "desc")) throw new ArgumentException("Unsupported status, sort, or direction.");
        BorrowingStatus? filter=status=="All" ? null : Enum.Parse<BorrowingStatus>(status);
        return Ok((await lookup.SearchBorrowingsAsync(search,filter,sort=="expectedReturnDate" ? "ExpectedReturnDate" : "DateBorrowed",direction=="desc",ct)).Select(b=>b.Dto()));
    }
    [HttpGet("students/{id}/borrowings"), ProducesResponseType<BorrowingDto[]>(200)]
    public async Task<IActionResult> StudentHistory(int id,CancellationToken ct) => Ok((await lookup.GetStudentHistoryAsync(id,ct)).Select(b=>b.Dto()));
    [HttpGet("equipment/{id}/borrowings"), ProducesResponseType<BorrowingDto[]>(200)]
    public async Task<IActionResult> EquipmentHistory(int id,CancellationToken ct) => Ok((await lookup.GetEquipmentHistoryAsync(id,ct)).Select(b=>b.Dto()));
    private static void ValidateDate(DateOnly due) { if(due<=DateOnly.FromDateTime(DateTime.Today)) throw new ArgumentException("Expected return date must be after the server's local date."); }
}

