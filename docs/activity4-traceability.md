# Activity 4 Traceability and Verification

| Scenario | Use case | Verification evidence to record | Acceptance result |
|---|---|---|---|
| A01 | UC-01 | Catalog vs PostgreSQL rows; empty and unavailable-DB UI states | Catalog and availability match stored records; empty and error differ |
| A02 | UC-02 | Available filter vs database; no-match and failed-load states | Unavailable items excluded |
| A03 | UC-03 | Successful checkout plus ineligible/missing/unavailable/limit/date rejections; fresh DB read and restart | One active record and unavailable item on success; rejection makes no changes |
| A04 | UC-04 | Active list with joined identities; empty and connection-error states | Returned rows excluded; list matches database |
| A05 | UC-05 | Return, repeat return, stale selection, fresh DB read and restart | Same record retained as returned with return date; item available; repeat rejected |
| A06 | UC-06 | Two-item success; duplicate, one item, unavailable, missing, invalid date, ineligible and over-limit rejection; induced transaction failure and fresh before/after reads | Full request commits together or nothing changes |
| A07 | UC-07 | Two distinguishable students and one empty student; switch selection and failed read | Correct active/returned histories, empty state and error state; async loading |
| A08 | UC-08 | Two items with histories and one empty item; switch selection and failed read | Only selected equipment history appears; correct related student data |
| A09 | UC-09 | Each search field, status, sort field/direction, combined criteria, clear, no match and connection failure | Results and stable order match criteria; async responsive loading |
| A12 | UC-12 | Register valid student; blank and normalized duplicate rejection; restart readback | Unique student persists, default eligible |
| A13 | UC-13 | Update identity/contact; blank contact; blank and duplicate rejection; inspect borrowing history before/after | Same student identity and borrowing relationships remain |
| A14 | UC-14 | Restrict and permit; attempt new borrowing while restricted; return existing active loan | Restriction blocks new checkout only and persists; active loan can still return |
| A15 | UC-15 | Register valid equipment; blank/duplicate rejection; restart readback | Item persists available without active borrowing |
| A16 | UC-16 | Edit fields and clear description; duplicate/blank rejection; inspect availability and linked history | Descriptive fields persist; availability and history unchanged |
| A17 | UC-17 | Apply migrations to a clean disposable PostgreSQL database; inspect constraints | Schema, keys, unique identifiers and filtered unique active-item index exist |
| A18 | UC-18 | Cause a known failure and an unavailable-database error, restore connection and refresh | No partial data on known failure; unknown commit is reported without claiming rollback and can be reconciled |
| A19 | UC-19 | Architecture/source review, README setup, diagram/catalog/suite, Git history, integration and restart evidence | Layer boundaries and required submission artifacts are present |

For each executed scenario, add **setup, action, expected result, observed result and evidence filename**. Deliberate constraint/failure scenarios must use a disposable PostgreSQL database. Read back through a fresh connection/context after writes; screenshots alone do not establish transaction atomicity or persistence.
