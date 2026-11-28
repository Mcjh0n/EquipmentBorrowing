# Activity 5 execution record

Executed against equipment_borrowing_lab5_acceptance using server-local calendar dates recorded in HTTP captures. The original coursework database was not reset. The dataset uses the reference's relative date offsets.

## Results and scope

- T01: independent HTTP reads, empty/missing paths, wrong method, OpenAPI, and unchanged database verified.
- T02/T03: management/eligibility HTTP cases and before/after data captured. Per-test process restart confirmation remains outstanding.
- T04: borrowing, invalid requests, return, repeated-return conflict, same-row retention and restored availability verified. Dedicated restart evidence remains outstanding.
- T05: success and exact three-loan boundaries, invalid requests with unchanged snapshots, and injected after-save/before-commit rollback verified. T05-rollback.json records HTTP 500 and equal independent database snapshots.
- T06: histories/search/filter/sort requests executed; semantic checks confirm search membership and stable ordering. Full history selection walkthrough remains outstanding.
- T07: API ran independently; Desktop code now uses HTTP and has no Infrastructure/EF Core reference. Desktop launch attempted, but computer inspection timed out; UI interactions and external-change refresh demonstration are NOT RUN.
- T08: reachable API with invalid database connection returned safe HTTP 500. Transport failure, UI feedback/recovery and dedicated restart evidence remain outstanding.

102 HTTP status checks completed. 38 additional semantic assertions passed. These counts do not imply every T01–T08 requirement passed. Requests/responses are in evidence/http-execution.json; semantic results in evidence/semantic-checks.json; independent database reads in the T01–T06 JSON snapshots. Build output is evidence/build.txt.

## Failures, corrections and retests

1. Independent psql reader received no password from the generic connection-string parser. Corrected the harness to require PGPASSWORD set locally. Reader then succeeded; no secret is stored in the script or evidence.
2. PowerShell compared snapshot lines as arrays and falsely reported a changed database. Corrected comparison to join the complete snapshot text. Read-only checks then passed.
3. NuGet flagged older Microsoft.OpenApi dependencies. Updated to 2.7.5; subsequent build had no vulnerability warning.
4. A build while diagnostic API hosts were running failed because their DLLs were locked. Stopped those hosts and rebuilt; see build.txt for the final result.
5. Desktop inspection tool timed out. No UI pass or screenshot has been fabricated.

## Remaining completion work

Run the UI portion of T07/T08, capture process restart durability for T02–T04/T08, and finish any unobserved reference substeps. Record expected/observed outcomes, faults and retests here. Only then create lab5-submitted on the completed revision. This revision is not tagged as a completed submission.
