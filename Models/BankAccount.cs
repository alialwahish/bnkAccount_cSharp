using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;


namespace LoginReg
{

    public class BankAccount
    {
        [Key]
        public int BankId { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public int Ballance { get; set; }

       

    }









}