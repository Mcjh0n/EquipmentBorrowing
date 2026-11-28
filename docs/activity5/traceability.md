# Activity 5 traceability

Implementation paths below are relative to the repository root. The original fourteen use-case tables remain in docs/activity4-use-case-suite.docx.

| Use cases | API | Implementation | Evidence |
|---|---|---|---|
| UC-01/02 | 01 | src/EquipmentBorrowing.Api/CampusController.cs: Equipment | T01 HTTP captures and snapshots |
| UC-03 | 10/13 | CampusController.Borrow/Borrowing; Application/Services/ApiBorrowingService.CheckoutAsync | T04/T05 captures and snapshots |
| UC-04/09 | 14 | CampusController.Borrowings; Infrastructure/Repositories/EfBorrowingRepository.SearchAsync | T06 captures; semantic-checks.json |
| UC-05 | 12/13 | CampusController.Return; ApiBorrowingService.ReturnAsync | T04-returned.json and HTTP captures |
| UC-06 | 11 | CampusController.Checkout; ApiBorrowingService.CheckoutAsync; EfBorrowingRepository.CheckoutManyAsync | T05 snapshots and T05-rollback.json |
| UC-07 | 02/03/15 | CampusController.Students/Student/StudentHistory | T01/T06 captures |
| UC-08 | 01/07/16 | CampusController.Equipment/EquipmentItem/EquipmentHistory | T01/T06 captures |
| UC-12/13/14 | 04/05/06 | CampusController.CreateStudent/UpdateStudent/Eligibility; Application/Services/StudentManagementService | T02 captures and snapshots |
| UC-15/16 | 08/09 | CampusController.CreateEquipment/UpdateEquipment; Application/Services/EquipmentManagementService | T03 captures and snapshots |
| HTTP-only desktop | all | src/EquipmentBorrowing.Desktop/Services/ApiClient.cs and App.axaml.cs | Source review; UI demonstration outstanding |
| Failure handling | all | src/EquipmentBorrowing.Api/Program.cs; Desktop/Services/ApiClient.cs | T08-server-error.json; remaining UI/restart evidence outstanding |

All evidence paths are under docs/activity5/evidence. See execution.md for limitations; the existence of a capture is not a claim that every associated use case has passed every branch.
