using System.ComponentModel.DataAnnotations;
using TaskSis.Api.Domain.DTOs.Common;

namespace TaskSis.Api.Domain.Validation;

public static class DtoValidation
{
    public static IReadOnlyList<ValidationErrorDto> Validate(object dto)
    {
        ValidationContext context = new(dto);
        List<ValidationResult> results = new();

        Validator.TryValidateObject(dto, context, results, validateAllProperties: true);

        if (results.Count == 0) return Array.Empty<ValidationErrorDto>();

        List<ValidationErrorDto> errors = new(results.Count);

        foreach (ValidationResult result in results)
        {
            string message = result.ErrorMessage ?? "Error de validación.";

            if (!result.MemberNames.Any())
            {
                errors.Add(new ValidationErrorDto(string.Empty, message));
                continue;
            }

            foreach (string memberName in result.MemberNames)
            {
                errors.Add(new ValidationErrorDto(memberName, message));
            }
        }

        return errors;
    }
}
