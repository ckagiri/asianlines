using System;

namespace Ligi.Infrastructure.Sql.AspnetSimpleMembership
{
    public class Membership
    {
        public Guid UserId {get;set;}
        public DateTime? CreateDate {get;set;}
        public string ConfirmationToken {get;set;}
        public bool? IsConfirmed {get;set;}
        public DateTime? LastPasswordFailureDate {get;set;}
        public int PasswordFailuresSinceLastSuccess {get;set;}
        public string Password {get;set;}
        public DateTime? PasswordChangedDate {get;set;}
        public string PasswordSalt {get;set;}
        public string PasswordVerificationToken {get;set;}
        public DateTime? PasswordVerificationTokenExpirationDate { get; set; }
    }
}
