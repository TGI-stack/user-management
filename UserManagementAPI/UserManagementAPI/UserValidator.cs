namespace UserManagementAPI1;

using System.ComponentModel.DataAnnotations;

public static class UserValidator
{
    private static readonly EmailAddressAttribute EmailValidator = new();

    public static string? Validate(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Name))
        {
            return "Name is required.";
        }

        if (string.IsNullOrWhiteSpace(user.Email))
        {
            return "Email is required.";
        }

        if (!EmailValidator.IsValid(user.Email))
        {
            return "Invalid email format.";
        }

        return null;
    }
}
