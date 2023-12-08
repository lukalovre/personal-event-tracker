public record PersonComboBoxItem(int ID, string FirstName, string LastName, string Nickname)
{
    public override string ToString()
    {
        return string.IsNullOrWhiteSpace(Nickname) ? $"{FirstName} {LastName}" : Nickname;
    }
}
