using System.Text.Json.Serialization;

namespace CoachSearch.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRole
{
	Trainer,
	Customer
}