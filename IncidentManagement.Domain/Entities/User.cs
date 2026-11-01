namespace IncidentManagement.Domain.Entities
{
    public class User
    {
        public int UserId { get; private set; }
        public string? Username { get; private set; }
        public string? Email { get; private set; }
        public string? PhoneNumber { get; private set; }
        public string? WhatsAppNumber { get; private set; }
        public string? PasswordHash { get; private set; }
        public byte[]? PasswordSalt { get; private set; }
        public int PropertyId { get; private set; }
        public Property? Property { get; private set; }
        public int? RoleId { get; private set; }
        public Roles? Role { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastLogin { get; private set; }
        public bool IsActive { get; private set; }

        protected User() { }

        public User(string username, string email, string phoneNumber, string whatsAppNumber,
            string passwordHash, byte[] passwordSalt, int propertyId, int? roleId = null)
        {
            Username = username;
            Email = email;
            PhoneNumber = phoneNumber;
            WhatsAppNumber = whatsAppNumber;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            PropertyId = propertyId;
            RoleId = roleId;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void SetActive(bool isActive) => IsActive = isActive;
    }
}
