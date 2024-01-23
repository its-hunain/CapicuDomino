using System.Text.RegularExpressions;


public class CheckValidation
{
    public static bool EmailValidaton(string emailStr)
    {
        bool isEmail = Regex.IsMatch(emailStr, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        return isEmail;

    }

   public static bool PasswordValidaton(string paswdStr)
    {
        bool ispassword = Regex.IsMatch(paswdStr, @"^(?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?=.*[^\w\d\s:])([^\s]){8,}$");
        return ispassword;

    }
}
