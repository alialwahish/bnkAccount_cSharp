using System.ComponentModel.DataAnnotations;
using System;


namespace LoginReg{

public class Transaction{

[Key]
public int TransactionId {get;set;}


[Required]
public string Type {get;set;}

[Required]
public DateTime Created_At {get;set;}

[Required]
public int Amount {get;set;}

public int UserId{get;set;}



}





}