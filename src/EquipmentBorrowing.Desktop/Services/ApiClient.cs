using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EquipmentBorrowing.Contracts;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Application;
namespace EquipmentBorrowing.Desktop.Services;

public sealed class ApiClient(HttpClient http) {
    public async Task<T> Send<T>(string path, HttpMethod? method=null,object? body=null) {
        method ??= HttpMethod.Get;
        using var request=new HttpRequestMessage(method,path);
        if(body is not null) request.Content=JsonContent.Create(body);
        try {
            using var response=await http.SendAsync(request);
            if(!response.IsSuccessStatusCode) {
                var error=await response.Content.ReadFromJsonAsync<ApiError>();
                throw new ApiClientException(error?.Code ?? "server_error", $"HTTP {(int)response.StatusCode}: {error?.Message ?? "Request failed."}");
            }
            return await response.Content.ReadFromJsonAsync<T>() ?? throw new ApiClientException("invalid_response","The API returned an empty response.");
        } catch(HttpRequestException) {
            throw new ApiClientException("connection_failure", method==HttpMethod.Get ? "Cannot reach the API. Check that the API is running, then refresh." : "No HTTP response was received. The change outcome is unknown; reconnect and refresh saved data before retrying.");
        } catch(TaskCanceledException) {
            throw new ApiClientException("timeout", method==HttpMethod.Get ? "The API request timed out. Try refreshing." : "The request timed out; the change outcome is unknown. Refresh saved data before retrying.");
        }
    }
    public static Student Model(StudentDto s)=>new(s.Id,s.StudentNumber,s.FullName,s.Contact,s.IsEligible);
    public static Equipment Model(EquipmentDto e){var item=new Equipment(e.Id,e.EquipmentCode,e.Name,e.Description); item.SetAvailable(e.IsAvailable); return item;}
    public static Borrowing Model(BorrowingDto b){var item=new Borrowing{Id=b.Id,Student=Model(b.Student),Equipment=Model(b.Equipment),DateBorrowed=b.BorrowedDate,ExpectedReturnDate=b.ExpectedReturnDate}; if(b.Status=="Returned" && b.ReturnedDate.HasValue)item.MarkAsReturned(b.ReturnedDate.Value);return item;}
}
public sealed class ApiClientException(string code,string message):Exception(message){ public string Code {get;}=code; }
public sealed class BorrowingLookupService(ApiClient api){
    public async Task<IEnumerable<Student>> GetStudentsAsync()=> (await api.Send<StudentDto[]>("students")).Select(ApiClient.Model);
    public async Task<IEnumerable<Equipment>> GetEquipmentAsync()=> (await api.Send<EquipmentDto[]>("equipment")).Select(ApiClient.Model);
    public async Task<IEnumerable<Equipment>> GetAvailableEquipmentAsync()=> (await api.Send<EquipmentDto[]>("equipment?available=true")).Select(ApiClient.Model);
    public async Task<IEnumerable<Borrowing>> GetStudentHistoryAsync(int id)=>(await api.Send<BorrowingDto[]>($"students/{id}/borrowings")).Select(ApiClient.Model);
    public async Task<IEnumerable<Borrowing>> GetEquipmentHistoryAsync(int id)=>(await api.Send<BorrowingDto[]>($"equipment/{id}/borrowings")).Select(ApiClient.Model);
    public async Task<IEnumerable<Borrowing>> SearchBorrowingsAsync(string text,BorrowingStatus? status,string sort,bool descending)=> (await api.Send<BorrowingDto[]>($"borrowings?search={Uri.EscapeDataString(text ?? "")}&status={status?.ToString() ?? "All"}&sort={(sort=="ExpectedReturnDate" ? "expectedReturnDate" : "borrowedDate")}&direction={(descending ? "desc" : "asc")}")).Select(ApiClient.Model);
}
public sealed class StudentManagementService(ApiClient api){
    public Task<StudentDto> RegisterAsync(string number,string name,string? contact)=>api.Send<StudentDto>("students",HttpMethod.Post,new StudentRequest(number,name,contact));
    public Task<StudentDto> UpdateAsync(int id,string number,string name,string? contact)=>api.Send<StudentDto>($"students/{id}",HttpMethod.Put,new StudentRequest(number,name,contact));
    public Task<StudentDto> SetEligibilityAsync(int id,bool allowed)=>api.Send<StudentDto>($"students/{id}/eligibility",HttpMethod.Patch,new EligibilityRequest(allowed));
}
public sealed class EquipmentManagementService(ApiClient api){
    public Task<EquipmentDto> RegisterAsync(string code,string name,string? description)=>api.Send<EquipmentDto>("equipment",HttpMethod.Post,new EquipmentRequest(code,name,description));
    public Task<EquipmentDto> UpdateAsync(int id,string code,string name,string? description)=>api.Send<EquipmentDto>($"equipment/{id}",HttpMethod.Put,new EquipmentRequest(code,name,description));
}
public sealed class BorrowEquipmentService(ApiClient api){
    public async Task<BorrowingResult> ExecuteAsync(int student,int equipment,DateTime due){await api.Send<BorrowingDto>("borrowings",HttpMethod.Post,new BorrowRequest(student,equipment,DateOnly.FromDateTime(due)));return BorrowingResult.Success("Equipment borrowed successfully.");}
}
public sealed class MultiItemCheckoutService(ApiClient api){
    public async Task<IReadOnlyList<BorrowingDto>> ExecuteAsync(int student,int[] items,DateOnly due)=>(await api.Send<CheckoutResponse>("checkout-requests",HttpMethod.Post,new CheckoutRequest(student,items,due))).Borrowings;
}
public sealed class ReturnEquipmentService(ApiClient api){
    public async Task<BorrowingResult> ExecuteAsync(int id){await api.Send<BorrowingDto>($"borrowings/{id}/return",HttpMethod.Post,new ReturnRequest());return BorrowingResult.Success("Equipment returned successfully.");}
}
