# Activity 4 Business Rules

This is the implementation checklist distilled from the supplied Activity 4 specification. The original requirement suite remains included as `activity4-use-case-suite.docx`.

## Student and equipment identity

- Student numbers and physical equipment identifiers are required, trimmed, and unique using case-insensitive normalized values.
- Full names and item names are required. Contact and equipment descriptions are optional and blank values are stored as absent.
- New students are eligible; new equipment is available.
- Descriptive updates preserve stable IDs and borrowing history. Eligibility and item availability are changed only in their dedicated workflows.
- Referenced students and equipment are not hard-deleted.

## Borrowing rules

- Borrowings reference one existing student and one existing physical equipment row.
- Students have at most three active item loans. Expected return must be a later local calendar date than checkout.
- Multi-item checkout requires at least two distinct items. Eligibility, count, references, date and availability are rechecked within one serializable transaction; every row and availability update commits or none does.
- PostgreSQL has a filtered unique index on equipment ID for active rows to prevent more than one active borrowing per item.
- Return records the local calendar return date, preserves history, makes the item available, and rejects repeat return.

## Retrieval, responsiveness and error behavior

- Search is case-insensitive partial matching over student name, student number and equipment name; status filters and stable ascending/descending date sort may be combined.
- Histories include active and returned rows. Read operations are asynchronous and expose loading, empty and failure states distinctly.
- A known failed transaction reports failure and leaves no partial changes. If commit acknowledgement is uncertain, the app must not claim rollback or automatically retry; reconnect and refresh before the next staff decision.
