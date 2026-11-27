using EquipmentBorrowing.Domain;
using Microsoft.EntityFrameworkCore;
namespace EquipmentBorrowing.Infrastructure.Persistence;

public static class AcceptanceDataset {
    public static async Task ResetAsync(EquipmentBorrowingDbContext db) {
        var database=db.Database.GetDbConnection().Database;
        if(!database.EndsWith("_lab5_acceptance",StringComparison.Ordinal)) throw new InvalidOperationException("Reset requires a database name ending in _lab5_acceptance.");
        await db.Database.MigrateAsync();
        await using var tx=await db.Database.BeginTransactionAsync();
        await db.Borrowings.ExecuteDeleteAsync(); await db.Equipment.ExecuteDeleteAsync(); await db.Students.ExecuteDeleteAsync();
        var s=new[]{new Student(0,"ST-100","Ada Cruz"),new Student(0,"ST-200","Ben Diaz",null,false),new Student(0,"ST-300","Cara Lim"),new Student(0,"ST-400","Dani Ong")};
        var names=new[]{"Camera","Laptop","Projector","Tablet","Microphone","Tripod","Speaker"};
        var e=names.Select((name,i)=>new Equipment(0,$"EQ-{i+1:000}",name)).ToArray();
        db.AddRange(s);db.AddRange(e);await db.SaveChangesAsync();
        var today=DateOnly.FromDateTime(DateTime.Today);
        Borrowing Loan(int student,int item,int borrowed,int due,bool returned=false){
            var b=new Borrowing{Student=s[student],Equipment=e[item],DateBorrowed=today.AddDays(borrowed),ExpectedReturnDate=today.AddDays(due)};
            if(returned)b.MarkAsReturned(today.AddDays(-2));else e[item].MarkAsUnavailable(); return b;
        }
        db.Borrowings.AddRange(Loan(0,0,-9,-1),Loan(0,2,-8,2,true),Loan(2,3,-7,2),Loan(2,4,-7,2));
        await db.SaveChangesAsync();await tx.CommitAsync();
    }
}
