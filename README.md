# Equipment Borrowing System

A C# and .NET project that shows how to borrow and return equipment in a school laboratory.
This project does not have a real database or a visual interface yet.
It only shows the basic structure and logic of the system.

---

## 1. Solution Structure

The project is split into 4 parts. Each part has its own job.

### Domain
This is the most important part.
It holds the main ideas of the system — like what a Student, Equipment, and Borrowing are.
It also holds simple rules, like what happens when equipment is borrowed or returned.
This part does not depend on any other part.

### Application
This part holds the actions the system can do — like borrowing or returning equipment.
It talks to the Domain part to use the main ideas.
It also defines "interfaces" — these are like contracts that say what the system needs to do with data, but not how to do it.
This part depends only on the Domain part.

### Infrastructure
This part holds the actual code that stores and reads data.
Right now, it uses a simple list in memory to store data (no real database yet).
Later, this part can be changed to use a real database like SQLite without changing the other parts.
This part depends on both the Domain and Application parts.

### Tests
This part holds the tests that check if the system works correctly.
It depends on the Domain and Application parts.

---

## 2. Dependency Direction

This shows which part depends on which other part:

```
ConsoleDemo (or Future UI)
         |
         v
     Application
      |       |
      v       |
    Domain    |
              |
      Infrastructure
```

- **Domain** does not need any other part.
- **Application** only needs the Domain part.
- **Infrastructure** needs both the Domain and Application parts.
- **ConsoleDemo** needs all three parts to run the program.

---

## 3. Use Case Mapping

This shows how one feature of the system works from start to finish.

```
Actor:                      Student
Use Case:                   Borrow Equipment
Application Service:        BorrowEquipmentService
Domain Objects Used:        Student, Equipment, Borrowing, BorrowingStatus
Repository Interfaces Used: IStudentRepository, IEquipmentRepository, IBorrowingRepository
Infrastructure Used:        InMemoryStudentRepository, InMemoryEquipmentRepository, InMemoryBorrowingRepository
```

---

## 4. Reflection

**1. Why should the application service use an interface instead of connecting directly to a database?**

When the service uses an interface, it does not care where the data comes from.
The data can come from memory, a file, or a database — and the service code stays the same.
This also makes it easier to test the service without needing a real database.

**2. Which parts will stay the same if we add a real database like SQLite later?**

The Domain and Application parts will not change at all.
Only the Infrastructure part will change — we just add new code there to connect to SQLite.
The rest of the system does not need to know about this change.

**3. Which part will hold the Avalonia UI screens in the future?**

A new part called something like `EquipmentBorrowing.Desktop` will hold the Avalonia screens.
This part will be on the outside and will call the Application part to do the work.

**4. Should a button on the screen directly run database code?**

No. A button should only call the application service.
The service will do the checking and the saving.
If we put database code inside a button, the code becomes messy and hard to fix or test later.

**5. Which part of the code represents the actual task the student wants to do?**

The `BorrowEquipmentService.ExecuteAsync` method is the main action.
It checks all the rules — like if the student is allowed to borrow, if the equipment is available, and if the student has not borrowed too many items already.
If all checks pass, it saves the borrowing record.
