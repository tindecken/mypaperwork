using System.Net.Mail;

namespace mypaperwork.Utils;

public class EmailUtils
{
    public bool isEmailValid(string email)
    {
        try
        {
            bool isValidEmail = new MailAddress(email).Address == email;
            if (isValidEmail) return true;
            return false;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}