using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestWebhook
{
    public class InfoValidateHelper
    {
        public bool validateEmail(string email)
        {
            if (email == null) return false;
            if (new EmailAddressAttribute().IsValid(email)) return true;
            else return false;
        }
    }
}
