namespace ColetaJaApi.DTOs
{
    public class AddressCreateRequest
    {
        public string Street { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string Complement { get; set; } = string.Empty;
        public string Neighborhood { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class ExamRequestCreate
    {
        public int ExamTypeId { get; set; }
        public int AddressId { get; set; }
    }

    public class ExamRequestResponse
    {
        public int Id { get; set; }
        public string ExamType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Result { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
