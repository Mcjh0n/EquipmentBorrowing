# Activity 4 Use Case Catalog

Primary actor for the required workflows: **Laboratory Staff / Equipment Custodian**. The complete actor interactions, branches, business-rule references and acceptance criteria are preserved in the supplied suite at `activity4-use-case-suite.docx`.

| ID | Use case | Classification |
|---|---|---|
| UC-01 | View Equipment Catalog | Required continuation |
| UC-02 | Find Available Equipment | Required continuation |
| UC-03 | Borrow Equipment | Required continuation |
| UC-04 | View Active Borrowings | Required continuation |
| UC-05 | Return Equipment | Required continuation |
| UC-06 | Check Out Multiple Equipment Items | Required new |
| UC-07 | View Student Borrowing History | Required new |
| UC-08 | View Equipment Borrowing History | Required new |
| UC-09 | Search, Filter, and Sort Borrowings | Required new |
| UC-10–11 | Overdue Borrowings; Current Equipment Holder | Deferred to Activity 5 |
| UC-12 | Register Student | Required new |
| UC-13 | Update Student Information | Required new |
| UC-14 | Change Student Borrowing Eligibility | Required new |
| UC-15 | Register Equipment | Required new |
| UC-16 | Update Equipment Information | Required new |
| UC-17–27 | Equipment retirement, maintenance, condition, renewals, bulk returns, summaries, categories and voiding | Deferred to Activity 5 |
| UC-28–33 | Reservations, staff accounts, audit and exports/integrations | Future scope |

## Use Case Diagram

```mermaid
flowchart LR
  Staff[Laboratory Staff / Equipment Custodian]
  subgraph System[Campus Equipment Borrowing System]
    UC01[UC-01 View catalog]
    UC02[UC-02 Find available]
    UC03[UC-03 Borrow item]
    UC04[UC-04 View active]
    UC05[UC-05 Return item]
    UC06[UC-06 Multi-item checkout]
    UC07[UC-07 Student history]
    UC08[UC-08 Equipment history]
    UC09[UC-09 Search/filter/sort]
    UC12[UC-12 Register student]
    UC13[UC-13 Update student]
    UC14[UC-14 Change eligibility]
    UC15[UC-15 Register equipment]
    UC16[UC-16 Update equipment]
  end
  Staff --- UC01
  Staff --- UC02
  Staff --- UC03
  Staff --- UC04
  Staff --- UC05
  Staff --- UC06
  Staff --- UC07
  Staff --- UC08
  Staff --- UC09
  Staff --- UC12
  Staff --- UC13
  Staff --- UC14
  Staff --- UC15
  Staff --- UC16
```
